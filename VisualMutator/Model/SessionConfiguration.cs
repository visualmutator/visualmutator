namespace VisualMutator.Model
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using System.Windows.Documents;
    using Controllers;
    using CoverageFinder;
    using Infrastructure;
    using log4net;
    using Microsoft.Cci;
    using Mutations;
    using Mutations.Types;
    using NUnit.Util;
    using StoringMutants;
    using Tests;
    using Tests.TestsTree;
    using UsefulTools.DependencyInjection;
    using UsefulTools.ExtensionMethods;
    using UsefulTools.Paths;

    public class SessionConfiguration
    {

        private readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);


        private readonly TestsLoader _testLoader;
        private readonly IFactory<AutoCreationController> _autoCreationControllerFactory;
        private readonly IRootFactory<SessionController> _sessionFactory;
        private readonly IWhiteSource _whiteCache;
        private readonly ProjectFilesClone _originalFilesClone;
        private readonly ProjectFilesClone _testsClone;

        public SessionConfiguration(
            IProjectClonesManager fileManager,
            TestsLoader testLoader,
            IFactory<AutoCreationController> autoCreationControllerFactory,
            IRootFactory<SessionController> sessionFactory,
            IWhiteSource whiteCache)
        {
            _testLoader = testLoader;
            _autoCreationControllerFactory = autoCreationControllerFactory;
            _sessionFactory = sessionFactory;
            _whiteCache = whiteCache;

            

            _originalFilesClone = fileManager.CreateClone("Mutants");

            _testsClone = fileManager.CreateClone("Tests");
            if (_originalFilesClone.IsIncomplete || _testsClone.IsIncomplete
                || _testsClone.Assemblies.Count == 0)
            {
                AssemblyLoadProblem = true;
            }
        }

        public bool AssemblyLoadProblem { get; set; }

        public async Task<List<CciModuleSource>> LoadAssemblies()
        {
            try
            {
                return await _whiteCache.GetWhiteModulesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        public async Task<TestsRootNode> LoadTests()
        {
            return await _testLoader.LoadTests(
             _testsClone.Assemblies.AsStrings().ToList());
        }


        public async Task<IObjectRoot<SessionController>> CreateSession(MethodIdentifier methodIdentifier, List<string> testAssemblies, bool auto)
        {
            _whiteCache.Pause(true);
            try
            {
                AutoCreationController creationController = _autoCreationControllerFactory.Create();
                var choices = await creationController.Run(methodIdentifier, testAssemblies, auto);
                var original = new OriginalCodebase(LoadAssemblies().Result, testAssemblies.ToEmptyIfNull().ToList());
                _log.Info("Created original codebase with assemblies to mutate: "+ original.ModulesToMutate.Select(m => m.Module.Name).MakeString());
                return _sessionFactory.CreateWithBindings(choices, original);
            }
            finally
            {
                _whiteCache.Pause(false);
            }
            

            
        }
    }
}