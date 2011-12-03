namespace VisualMutator.Controllers
{
    #region Usings

    using System;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using System.Windows.Forms;

    using CommonUtilityInfrastructure;
    using CommonUtilityInfrastructure.Threading;
    using CommonUtilityInfrastructure.WpfUtils;

    using VisualMutator.Infrastructure;
    using VisualMutator.Model.Mutations;
    using VisualMutator.Model.Mutations.Operators;
    using VisualMutator.Model.Mutations.Types;
    using VisualMutator.ViewModels;
    using VisualMutator.Views;

    using log4net;

    #endregion

    public class OnlyMutantsCreationController : Controller
    {
        private ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

     

        private readonly IOperatorsManager _operatorsManager;

        private readonly CommonServices _svc;

        private readonly ITypesManager _typesManager;

        private readonly OnlyMutantsCreationViewModel _viewModel;


        public MutationSessionChoices Result { get; set; }


        public OnlyMutantsCreationController(
            OnlyMutantsCreationViewModel viewModel,
            ITypesManager typesManager,
            IOperatorsManager operatorsManager,
            CommonServices svc)
        {
            _viewModel = viewModel;

            _typesManager = typesManager;
            _operatorsManager = operatorsManager;
            _svc = svc;


            _viewModel.CommandCreateMutants = new BasicCommand(AcceptChoices,
                () => _viewModel.TypesTree.Assemblies != null && _viewModel.MutationsTree.MutationPackages != null)
                    .UpdateOnChanged(_viewModel.TypesTree, _ => _.Assemblies)
                    .UpdateOnChanged(_viewModel.MutationsTree, _ => _.MutationPackages);

   
            _viewModel.CommandBrowseForMutantFolder = new BasicCommand(BrowseForMutantsFolder);
        }

        public void BrowseForMutantsFolder()
        {
            var dlg = new FolderBrowserDialog();
            dlg.ShowNewFolderButton = true;
            DialogResult result = dlg.ShowDialog();

            if (result == DialogResult.OK)
            {
                _viewModel.MutantsFolderPath = dlg.SelectedPath;

            }
        }

        public OnlyMutantsCreationController Run()
        {
            
            _svc.Threading.ScheduleAsync(()=> _operatorsManager.LoadOperators(),
                packages => _viewModel.MutationsTree.MutationPackages = new ReadOnlyCollection<PackageNode>(packages));

            _svc.Threading.ScheduleAsync(() => _typesManager.GetTypesFromAssemblies(),
                assemblies =>
                {
                    _viewModel.TypesTree.Assemblies = new ReadOnlyCollection<AssemblyNode>(assemblies);
                    if (_typesManager.IsAssemblyLoadError)
                    {
                        _svc.Logging.ShowWarning(UserMessages.WarningAssemblyNotLoaded(), _log);

                    }
                });

            _viewModel.ShowDialog();
            return this;
        }
        public void AcceptChoices()
        {

            if (!string.IsNullOrEmpty(_viewModel.MutantsFolderPath) 
                && Path.IsPathRooted(_viewModel.MutantsFolderPath))
            {

                if(!_svc.FileSystem.Directory.Exists(_viewModel.MutantsFolderPath))
                {
                    try
                    {
                        _svc.FileSystem.Directory.CreateDirectory(_viewModel.MutantsFolderPath);
                    }
                    catch (Exception)
                    {
                        _svc.Logging.ShowError("Could not create directory.");
                        return;
                    }
                    
                }
                

                if (_svc.FileSystem.Directory.GetDirectories(_viewModel.MutantsFolderPath).Length == 0
                    && _svc.FileSystem.Directory.GetFiles(_viewModel.MutantsFolderPath).Length == 0)
                {
                    Result = new MutationSessionChoices
                    {
                        SelectedOperators = _viewModel.MutationsTree.MutationPackages.SelectMany(pack => pack.Operators)
                                         .Where(oper => oper.IsLeafIncluded).Select(n => n.Operator).ToList(),
                        Assemblies = _viewModel.TypesTree.Assemblies.Select(a => a.AssemblyDefinition).ToList(),
                        SelectedTypes = _typesManager.GetIncludedTypes(_viewModel.TypesTree.Assemblies),
                        MutantsCreationOptions = _viewModel.MutantsCreation.Options,
                        MutantsCreationFolderPath = _viewModel.MutantsFolderPath,
                    };
                    _viewModel.Close();
                }
                else
                {
                    _svc.Logging.ShowError("Selected directory is not empty. Select empty directory.");
                }
            }
            else
            {
                _svc.Logging.ShowError("Selected path is invalid");
            }
        }


        public bool HasResults
        {
            get
            {
                return Result != null;
            }
        }

    }
}