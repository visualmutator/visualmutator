namespace VisualMutator.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows.Documents;
    using Controllers;
    using CoverageFinder;
    using Infrastructure;
    using Microsoft.Cci;
    using Mutations.Types;
    using NUnit.Util;
    using StoringMutants;
    using Tests;
    using UsefulTools.DependencyInjection;
    using UsefulTools.ExtensionMethods;
    using UsefulTools.Paths;

    public class SessionConfiguration
    {
        private readonly TestsLoader _testLoader;
        private readonly ITypesManager _typesManager;
        private readonly IFactory<AutoCreationController> _autoCreationControllerFactory;
        private readonly IRootFactory<SessionController> _sessionFactory;
        private readonly IWhiteCache _whiteCache;
        private readonly ProjectFilesClone _originalFilesClone;
        private readonly ProjectFilesClone _testsClone;

        public SessionConfiguration(
            IProjectClonesManager fileManager,
            TestsLoader testLoader,
            ITypesManager typesManager,
            IFactory<AutoCreationController> autoCreationControllerFactory,
            IRootFactory<SessionController> sessionFactory,
            IWhiteCache whiteCache)
        {
            _testLoader = testLoader;
            _typesManager = typesManager;
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
        public Task<object> LoadTests()
        {
            return Task.Run(() => _testLoader.LoadTests(
             _testsClone.Assemblies.AsStrings().ToList()).CastTo<object>());
        }


        public async Task<IObjectRoot<SessionController>> CreateSession(MethodIdentifier methodIdentifier, bool auto)
        {
            _whiteCache.Pause(true);
            try
            {
                AutoCreationController creationController = _autoCreationControllerFactory.Create();
                var choices = await creationController.Run(methodIdentifier, auto);
                return _sessionFactory.CreateWithBindings(choices);
            }
            finally
            {
                _whiteCache.Pause(false);
            }
            

            
        }
    }
}