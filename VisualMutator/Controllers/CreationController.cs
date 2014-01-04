namespace VisualMutator.Controllers
{
    #region

    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Security.Cryptography;
    using System.Threading.Tasks;
    using Infrastructure;
    using log4net;
    using Model;
    using Model.Exceptions;
    using Model.Mutations.Operators;
    using Model.Mutations.Types;
    using Model.StoringMutants;
    using Model.Tests;
    using Model.Tests.TestsTree;
    using UsefulTools.CheckboxedTree;
    using UsefulTools.Core;
    using UsefulTools.ExtensionMethods;
    using UsefulTools.Paths;
    using UsefulTools.Wpf;
    using ViewModels;

    #endregion

    public abstract class CreationController<TViewModel, TView> : Controller
        where TViewModel : CreationViewModel<TView> where TView : class, IWindow
    {
        protected readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);



        protected readonly IOperatorsManager _operatorsManager;
        private readonly IHostEnviromentConnection _hostEnviroment;
        protected readonly ITestsContainer _testsContainer;
        private readonly IFileManager _fileManager;

        protected readonly CommonServices _svc;

        protected readonly ITypesManager _typesManager;

        protected readonly TViewModel _viewModel;


        public MutationSessionChoices Result { get; protected set; }

        protected CreationController(
            TViewModel viewModel,
            ITypesManager typesManager,
            IOperatorsManager operatorsManager,
            IHostEnviromentConnection hostEnviroment,
            ITestsContainer testsContainer,
            IFileManager fileManager,
            CommonServices svc)
        {
            _viewModel = viewModel;

            _typesManager = typesManager;
            _operatorsManager = operatorsManager;
            _hostEnviroment = hostEnviroment;
            _testsContainer = testsContainer;
            _fileManager = fileManager;
            _svc = svc;


            _viewModel.CommandCreateMutants = new SmartCommand(AcceptChoices,
                () => _viewModel.TypesTreeMutate.Assemblies != null && _viewModel.TypesTreeMutate.Assemblies.Count != 0
                      && _viewModel.TypesTreeToTest.Namespaces!=null && _viewModel.TypesTreeToTest.Namespaces.Count != 0
                      && _viewModel.MutationsTree.MutationPackages.Count != 0)
                .UpdateOnChanged(_viewModel.TypesTreeMutate, _ => _.Assemblies)
                .UpdateOnChanged(_viewModel.TypesTreeToTest, _ => _.Namespaces)
                .UpdateOnChanged(_viewModel.MutationsTree, _ => _.MutationPackages);



        }

        public void SetMutationConstraints(ClassAndMethod classAndMethod)
        {

        }

        public void Run2(ClassAndMethod classAndMethod)
        {
            bool loadError;
            var originalFilesList = _fileManager.CopyOriginalFiles(out loadError);
            if (loadError)
            {
                _svc.Logging.ShowWarning(UserMessages.WarningAssemblyNotLoaded(), null);
            }

            _viewModel.ProjectPaths = _hostEnviroment.GetProjectPaths().ToList();

            _svc.Threading.ScheduleAsync(() => _operatorsManager.LoadOperators(),
                packages => _viewModel.MutationsTree.MutationPackages
                    = new ReadOnlyCollection<PackageNode>(packages));

            
            List<ClassAndMethod> coveredTests = null;
            var t = Task.Run(() => _typesManager.GetTypesFromAssemblies(originalFilesList, classAndMethod,
                out coveredTests).CastTo<object>());
            var task = Task.Run(() => _testsContainer.LoadTests(
                originalFilesList.AsStrings()).ToList().CastTo<object>());

            Task.WhenAll(t, task).ContinueWith( 
                (Task<object[]> result) =>
                {
                    if(result.Exception!= null)
                    {
                        if(result.Exception.Flatten().InnerException is TestWasSelectedToMutateException)
                        {
                            _svc.Logging.ShowError(UserMessages.ErrorBadMethodSelected(), _viewModel.View);
                        }
                        else
                        {
                            _svc.Logging.ShowError(result.Exception, _viewModel.View);
                        }
                        _viewModel.Close();
                    }
                    else
                    {
                        var assemblies = (IList<AssemblyNode>)result.Result[0];
                        var tests = (IList<TestNodeNamespace>)result.Result[1];

                        assemblies = assemblies.Where(a => a.Children.Count > 0).ToList();

                        SelectOnlyCovered(tests, coveredTests);
                        _svc.Threading.PostOnGui(() =>
                        {
                            if (_typesManager.IsAssemblyLoadError)
                            {
                                _svc.Logging.ShowWarning(UserMessages.WarningAssemblyNotLoaded(), _viewModel.View);
                            }
                            _viewModel.TypesTreeMutate.Assemblies = new ReadOnlyCollection<AssemblyNode>(assemblies);

                            _viewModel.TypesTreeToTest.Namespaces = new ReadOnlyCollection<TestNodeNamespace>(tests);

                        });
                    }
                    
                    
                    
                });

            _viewModel.ShowDialog();
        }

        private void SelectOnlyCovered(IList<TestNodeNamespace> tests, List<ClassAndMethod> coveredTests)
        {
            foreach (var testNodeNamespace in tests)
            {
                testNodeNamespace.IsIncluded = false;
            }
            var toSelect = tests.Cast<CheckedNode>().SelectManyRecursive(n => n.Children, leafsOnly: true)
                .OfType<TestNodeMethod>()
                .Where(t => coveredTests.Select(_=>_.ClassName).Contains(t.ContainingClassFullName)
                &&  coveredTests.Select(_=>_.MethodName).Contains(t.Name));
            foreach (var testNodeMethod in toSelect)
            {
                testNodeMethod.IsIncluded = true;
            }
        }


        public void Run()
        {
           /* _svc.Threading.PostOnGui(async () =>
                {


                  //  var assemblies = await Task.Run(() => _typesManager.GetTypesFromAssemblies());


                 //   _viewModel.TypesTreeMutate.Assemblies = assemblies.ToReadonly();



                    var taskGetAssemblies = Task.Run<object>(() => _typesManager.GetTypesFromAssemblies());
                    var taskLoadTests = Task.Run<object>(() => _testsContainer.LoadTests(
                                _hostEnviroment.GetProjectAssemblyPaths().Select(p => (string) p).ToList()));
                    var loadOperators = Task.Run<object>(() => _operatorsManager.LoadOperators());


                    var tAll = await Task.WhenAll(taskGetAssemblies, taskLoadTests, loadOperators);

                  //  t.
                    if (_typesManager.IsAssemblyLoadError)
                    {

                        _svc.Logging.ShowWarning(UserMessages.WarningAssemblyNotLoaded(), _log, _viewModel.View);
                    }

                    _viewModel.TypesTreeMutate.Assemblies = tAll[0].CastTo<IList<AssemblyNode>>().ToReadonly();
                    _viewModel.TypesTreeToTest.Namespaces = tAll[1].CastTo<IEnumerable<TestNodeNamespace>>().ToReadonly();
                

                   // var packages = await Task.Run(() => _operatorsManager.LoadOperators());

                    _viewModel.MutationsTree.MutationPackages = tAll[2].CastTo<IList<PackageNode>>().ToReadonly();

                });
            */


            bool loadError;
            var originalFilesList = _fileManager.CopyOriginalFiles(out loadError);
            if (loadError)
            {
                _svc.Logging.ShowWarning(UserMessages.WarningAssemblyNotLoaded(), null);
            }

            _svc.Threading.ScheduleAsync(()=> _operatorsManager.LoadOperators(),
                packages => _viewModel.MutationsTree.MutationPackages 
                    = new ReadOnlyCollection<PackageNode>(packages));

            _svc.Threading.ScheduleAsync(() => _typesManager.GetTypesFromAssemblies(originalFilesList),
                assemblies =>
                {
                    _viewModel.TypesTreeMutate.Assemblies =  new ReadOnlyCollection<AssemblyNode>(assemblies);

                    if (_typesManager.IsAssemblyLoadError)
                    {
                        _svc.Logging.ShowWarning(UserMessages.WarningAssemblyNotLoaded(),  _viewModel.View);
                    }
                });
            _svc.Threading.ScheduleAsync(() => _testsContainer.LoadTests(
                originalFilesList.AsStrings()),
               tests =>
               {
                   _viewModel.TypesTreeToTest.Namespaces = new ReadOnlyCollection<TestNodeNamespace>(tests.ToList());

                
               });
            _viewModel.ShowDialog();
    
        }

        protected abstract void AcceptChoices();


        public bool HasResults
        {
            get
            {
                return Result != null;
            }
        }

    }
}