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
        private readonly IOptionsManager _optionsManager;
        private readonly IFileSystemManager _fileManager;
        private readonly TestsLoader _testLoader;
        private readonly IWhiteCache _whiteCache;
        private readonly ITypesManager _typesManager;
        private readonly IFactory<CreationController> _creationControllerFactory;
        private readonly IBindingFactory<SessionController> _sessionFactory;
        private readonly List<FilePathAbsolute> _assembliesPaths;
        private ProjectFilesClone _originalFilesClone;
        private ProjectFilesClone _testsClone;

        public SessionConfiguration(
            IOptionsManager optionsManager,
            IFileSystemManager fileManager,
            TestsLoader testLoader,
            ITypesManager typesManager,
            IFactory<CreationController> creationControllerFactory,
            IBindingFactory<SessionController> sessionFactory,
            IWhiteCache whiteCache)
        {
            _optionsManager = optionsManager;
            _fileManager = fileManager;
            _testLoader = testLoader;
            _whiteCache = whiteCache;
            _typesManager = typesManager;
            _creationControllerFactory = creationControllerFactory;
            _sessionFactory = sessionFactory;

            _fileManager.Initialize();

            _originalFilesClone = _fileManager.CreateClone("Mutants");

            _whiteCache.Initialize();
              //  _originalFilesClone.Assemblies.AsStrings().ToList());

            _testsClone = _fileManager.CreateClone("Tests");
            if (_originalFilesClone.IsIncomplete || _testsClone.IsIncomplete
                || _testsClone.Assemblies.Count == 0)
            {
                AssemblyLoadProblem = true;
            }
        }

        public bool AssemblyLoadProblem { get; set; }

        public Task<IList<IModule>> LoadAssemblies()
        {
            return Task.Run(() => _typesManager.LoadAssemblies(
                    _originalFilesClone.Assemblies));
        }
        public Task<object> LoadTests()
        {
            return Task.Run(() => _testLoader.LoadTests(
             _testsClone.Assemblies.AsStrings().ToList()).CastTo<object>());

        }


        public Task<SessionController> CreateSession(MethodIdentifier methodIdentifier = null)
        {
            var tcs = new TaskCompletionSource<SessionController>();

            CreationController creationController = _creationControllerFactory.Create();
            creationController.Run(methodIdentifier);
            if (creationController.HasResults)
            {
                SessionController sessionController = _sessionFactory
                    .CreateWithBindings(creationController.Result);
                tcs.TrySetResult(sessionController);
            }
            else
            {
                tcs.TrySetCanceled();
            }

            return tcs.Task;
        }
    }
}