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

        private readonly CommonServices _svc;

        private readonly ITypesManager _typesManager;

        private readonly SessionCreationViewModel _viewModel;

        private ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

    //    private string[] paths;

        public MutationSessionChoices Result { get; set; }

  
        public SessionCreationController(
            SessionCreationViewModel viewModel,
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

   
        }



        public SessionCreationController Run()
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
            Result = new MutationSessionChoices
            {
                SelectedOperators = _viewModel.MutationsTree.MutationPackages.SelectMany(pack => pack.Operators)
                                 .Where(oper => oper.IsLeafIncluded).Select(n=>n.Operator).ToList(),
                Assemblies = _viewModel.TypesTree.Assemblies.Select(a => a.AssemblyDefinition).ToList(),
                SelectedTypes = _typesManager.GetIncludedTypes(_viewModel.TypesTree.Assemblies),
                MutantsCreationOptions = _viewModel.MutantsCreation.Options,
                MutantsTestingOptions = _viewModel.MutantsTesting.Options,
            };
            _viewModel.Close();
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