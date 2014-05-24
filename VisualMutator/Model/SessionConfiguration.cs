namespace VisualMutator.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows.Documents;
    using Controllers;
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

            fileManager.Initialize();

            _originalFilesClone = fileManager.CreateClone("Mutants");

            whiteCache.Initialize();

            _testsClone = fileManager.CreateClone("Tests");
            if (_originalFilesClone.IsIncomplete || _testsClone.IsIncomplete
                || _testsClone.Assemblies.Count == 0)
            {
                AssemblyLoadProblem = true;
            }
        }

        public bool AssemblyLoadProblem { get; set; }

        public Task<IModuleSource> LoadAssemblies()
        {
            return Task.Run(() => _typesManager.LoadAssemblies(
                    _originalFilesClone.Assemblies));
        }
        public Task<object> LoadTests()
        {
            return Task.Run(() => _testLoader.LoadTests(
             _testsClone.Assemblies.AsStrings().ToList()).CastTo<object>());
        }


        public async Task<IObjectRoot<SessionController>> CreateSession(MethodIdentifier methodIdentifier = null)
        {
            AutoCreationController creationController = _autoCreationControllerFactory.Create();
            var choices = await creationController.Run(methodIdentifier) ;
            return _sessionFactory.CreateWithBindings(choices);

        }
        public async Task<IObjectRoot<SessionController>> CreateSessionAuto(MethodIdentifier methodIdentifier)
        {

            AutoCreationController creationController = _autoCreationControllerFactory.Create();
            var choices = await creationController.Run(methodIdentifier, true);
            return _sessionFactory.CreateWithBindings(choices);
           

        }

        
    }
}