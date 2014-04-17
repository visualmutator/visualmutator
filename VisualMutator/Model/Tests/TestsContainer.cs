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
    }

    public class TestsContainer : ITestsContainer
    {
        private readonly IMutantsFileManager _mutantsFileManager;
        private readonly IFileSystemManager _fileManager;

        private readonly IAssemblyVerifier _assemblyVerifier;


        private ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private bool _testsLoaded;

        private readonly TestResultTreeCreator _testResultTreeCreator;

        public TestsContainer(
            IMutantsFileManager mutantsFileManager,
            IFileSystemManager fileManager,
            IAssemblyVerifier assemblyVerifier)
        {
            _mutantsFileManager = mutantsFileManager;
            _fileManager = fileManager;
            _assemblyVerifier = assemblyVerifier;
            _testResultTreeCreator = new TestResultTreeCreator();
        }
       

        public void CreateTestSelections(IList<TestNodeAssembly> testAssemblies)
        {
            var testsSelector = new TestsSelector();
            foreach (var testNodeAssembly in testAssemblies)
            {
                testNodeAssembly.TestsLoadContext.SelectedTests = 
                    testsSelector.GetIncludedTests(testNodeAssembly);
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
            var result = new StoredMutantInfo(clone);
            _mutantsFileManager.StoreMutant(result, mutant);
            return result;
        }


        public IEnumerable<TestsRunContext> CreateTestContexts(
            List<string> mutatedPaths,
            IList<TestNodeAssembly> testAssemblies)
        {


            foreach (var testNodeAssembly in testAssemblies)
            {
                //todo: get rid of this ungly thing
                var mutatedPath = mutatedPaths.Single(p => Path.GetFileName(p) ==
                    Path.GetFileName(testNodeAssembly.AssemblyPath));

                var originalContext = testNodeAssembly.TestsLoadContext;
                var context = new TestsRunContext();
                context.SelectedTests = originalContext.SelectedTests;
                context.AssemblyPath = mutatedPath;

                yield return context;
            }
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