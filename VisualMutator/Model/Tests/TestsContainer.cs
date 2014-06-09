namespace VisualMutator.Model.Tests
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using Exceptions;
    using Infrastructure;
    using LinqLib.Operators;
    using log4net;
    using Mutations;
    using Mutations.MutantsTree;
    using Services;
    using StoringMutants;
    using Strilanc.Value;
    using TestsTree;
    using UsefulTools.CheckboxedTree;
    using UsefulTools.Core;
    using UsefulTools.ExtensionMethods;
    using Verification;

    #endregion

    public interface ITestsContainer
    {

        ProjectFilesClone InitTestEnvironment();

        void CancelAllTesting();

        bool VerifyMutant( StoredMutantInfo storedMutantInfo, Mutant mutant);

        Task<StoredMutantInfo> StoreMutant(Mutant changelessMutant);

   
        void CreateTestSelections(IList<TestNodeAssembly> testAssemblies);
        IEnumerable<TestNodeNamespace> CreateMutantTestTree(Mutant mutant);
    }

    public class TestsContainer : ITestsContainer
    {
        private readonly IMutantsCache _mutantsCache;
        private readonly IProjectClonesManager _fileManager;
        private readonly MutationSessionChoices _choices;

        private readonly IAssemblyVerifier _assemblyVerifier;
        private readonly CommonServices _svc;


        private ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);


        private readonly TestResultTreeCreator _testResultTreeCreator;

        public TestsContainer(
            IMutantsCache mutantsCache,
            IProjectClonesManager fileManager,
            MutationSessionChoices choices,
            IAssemblyVerifier assemblyVerifier,
            CommonServices svc)
        {
            _mutantsCache = mutantsCache;
            _fileManager = fileManager;
            _choices = choices;
            _assemblyVerifier = assemblyVerifier;
            _svc = svc;
            _testResultTreeCreator = new TestResultTreeCreator();
        }
       

        public void CreateTestSelections(IList<TestNodeAssembly> testAssemblies)
        {
            var testsSelector = new TestsSelector();
            foreach (var testNodeAssembly in testAssemblies)
            {
                testNodeAssembly.TestsLoadContext.SelectedTests = 
                    testsSelector.GetIncludedTests(testNodeAssembly);

                _log.Debug("Created tests to run: "+ testNodeAssembly.TestsLoadContext.SelectedTests.TestsDescription);
            }
        }


        public void VerifyAssemblies(List<string> assembliesPaths)
        {
            foreach (var assemblyPath in assembliesPaths)
            {
                _assemblyVerifier.Verify(assemblyPath);
            }
  
        }
        public ProjectFilesClone InitTestEnvironment()
        {
            return _fileManager.CreateClone("InitTestEnvironment");
        }



        public bool VerifyMutant( StoredMutantInfo storedMutantInfo, Mutant mutant)
        {

            try
            {
                VerifyAssemblies(storedMutantInfo.AssembliesPaths);
            }
            catch (AssemblyVerificationException e)
            {

                mutant.MutantTestSession.ErrorDescription = "Mutant assembly failed verification";
                mutant.MutantTestSession.ErrorMessage = e.Message;
                mutant.MutantTestSession.Exception = e;
                mutant.State = MutantResultState.Error;
                return false;
            }
            return true;
                

        }

        public async Task<StoredMutantInfo> StoreMutant( Mutant mutant)
        {
            var clone = await _fileManager.CreateCloneAsync("InitTestEnvironment");
            var info = new StoredMutantInfo(clone);
            var mutationResult = await _mutantsCache.GetMutatedModulesAsync(mutant);

            var singleMutated = mutationResult.MutatedModules.Modules.SingleOrDefault();
            if (singleMutated != null)
            {
                await WriteModule(singleMutated, info, mutationResult.WhiteModules);
            }

            var otherModules = _choices.WhiteSource.ModulesInfo
                .Where(_ => singleMutated == null || _.Name != singleMutated.Name);

            foreach (var otherModule in otherModules)
            {
                string file = Path.Combine(info.Directory, otherModule.Name + ".dll");
                info.AssembliesPaths.Add(file);
            }

            return info;
        }

        public async Task WriteModule(IModuleInfo module, StoredMutantInfo info, ICciModuleSource cci)
        {
            //TODO: remove: assemblyDefinition.Name.Name + ".dll", use factual original file name
            string file = Path.Combine(info.Directory, module.Name + ".dll");
            await Task.Run(() => cci.WriteToFile(module, file));
            info.AssembliesPaths.Add(file);

        }
    
        public void CancelAllTesting()
        {
        }

       

        public IEnumerable<TestNodeNamespace> CreateMutantTestTree(Mutant mutant)
        {
            List<TmpTestNodeMethod> nodeMethods = mutant.TestRunContexts
                .SelectMany(c => c.TestResults.ResultMethods).ToList();

            return _testResultTreeCreator.CreateMutantTestTree(nodeMethods);
        }
    }
}