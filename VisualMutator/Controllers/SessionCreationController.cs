namespace VisualMutator.Controllers
{
    #region Usings

    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;

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

    public class SessionCreationController : Controller
    {
      
     

        private readonly IOperatorsManager _operatorsManager;

        private readonly CommonServices _commonServices;

        private readonly ITypesManager _typesManager;

        private readonly MutantsCreationViewModel _viewModel;

        private ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

    //    private string[] paths;

        public MutationSessionChoices Result { get; set; }

        public bool HasResults
        {
            get
            {
                return Result != null;
            }
        }

        public SessionCreationController(
            MutantsCreationViewModel viewModel,
            ITypesManager typesManager,
            IOperatorsManager operatorsManager,
            CommonServices commonServices)
        {
            _viewModel = viewModel;

            _typesManager = typesManager;
            _operatorsManager = operatorsManager;
            _commonServices = commonServices;


            _viewModel.CommandCreateMutants = new BasicCommand(StoreChoicesResults, () => _viewModel.Assemblies != null
                && _viewModel.MutationPackages != null);
            _viewModel.CommandCreateMutants.UpdateOnChanged(_viewModel, () => _viewModel.Assemblies);
            _viewModel.CommandCreateMutants.UpdateOnChanged(_viewModel, () => _viewModel.MutationPackages);

            _viewModel.CommandAdditionalFileToCopy = new BasicCommand(ChooseFiles);

            _viewModel.AdditionalFileToCopy = new BetterObservableCollection<string>();

            _viewModel.TimeoutSeconds = 10;
        }



        public void ChooseFiles()
        {
            var dlg = new Microsoft.Win32.OpenFileDialog();

            bool? result = dlg.ShowDialog();

            if (result == true)
            {

                _viewModel.AdditionalFileToCopy.ReplaceRange(dlg.FileNames);
            }
        }


        public SessionCreationController Run()
        {
            
            _commonServices.Threading.ScheduleAsync(()=> _operatorsManager.LoadOperators(),
                packages => _viewModel.MutationPackages = new ReadOnlyCollection<PackageNode>(packages));

            _commonServices.Threading.ScheduleAsync(() => _typesManager.GetTypesFromAssemblies(),
                assemblies => _viewModel.Assemblies = new ReadOnlyCollection<AssemblyNode>(assemblies));

            _viewModel.ShowDialog();
            return this;
        }
        public void StoreChoicesResults()
        {
            Result = new MutationSessionChoices
            {
                SelectedOperators = _viewModel.MutationPackages.SelectMany(pack => pack.Operators)
                                 .Where(oper => oper.IsLeafIncluded).Select(n=>n.Operator).ToList(),
                Assemblies = _viewModel.Assemblies.Select(a => a.AssemblyDefinition).ToList(),
                SelectedTypes = _typesManager.GetIncludedTypes(_viewModel.Assemblies),
                AdditionalFilesToCopy = _viewModel.AdditionalFileToCopy.ToList(),
                CreateMoreMutants = _viewModel.CreateMoreMutants,
                TestingTimeoutSeconds = _viewModel.TimeoutSeconds
            };
            _viewModel.Close();
        }


       
    }
}