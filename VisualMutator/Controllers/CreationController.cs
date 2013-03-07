namespace VisualMutator.Controllers
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using System.Windows.Forms;

    using CommonUtilityInfrastructure;
    using CommonUtilityInfrastructure.Threading;
    using CommonUtilityInfrastructure.WpfUtils;
    using Model;
    using VisualMutator.Infrastructure;
    using VisualMutator.Model.Mutations;
    using VisualMutator.Model.Mutations.Operators;
    using VisualMutator.Model.Mutations.Types;
    using VisualMutator.ViewModels;
    using VisualMutator.Views;

    using log4net;

    #endregion

    public abstract class CreationController<TViewModel, TView> : Controller
        where TViewModel : CreationViewModel<TView> where TView : class, IWindow
    {
        protected readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);



        protected readonly IOperatorsManager _operatorsManager;

        protected readonly CommonServices _svc;

        protected readonly ITypesManager _typesManager;

        protected readonly TViewModel _viewModel;


        public MutationSessionChoices Result { get; protected set; }

        protected CreationController(
            TViewModel viewModel,
            ITypesManager typesManager,
            IOperatorsManager operatorsManager,
            CommonServices svc)
        {
            _viewModel = viewModel;

            _typesManager = typesManager;
            _operatorsManager = operatorsManager;
            _svc = svc;


            _viewModel.CommandCreateMutants = new BasicCommand(AcceptChoices,
                () => _viewModel.TypesTreeMutate.Assemblies != null 
                    && _viewModel.MutationsTree.MutationPackages != null
                    && _viewModel.TypesTreeMutate.Assemblies.Count != 0
                  //  && _viewModel.TypesTreeToTest.Assemblies.Count != 0
                    && _viewModel.MutationsTree.MutationPackages.Count != 0)
                    .UpdateOnChanged(_viewModel.TypesTreeMutate, _ => _.Assemblies)
                   // .UpdateOnChanged(_viewModel.TypesTreeToTest, _ => _.Assemblies)
                    .UpdateOnChanged(_viewModel.MutationsTree, _ => _.MutationPackages);

   
           
        }

        public async void Run()
        {
            
            _svc.Threading.ScheduleAsync(()=> _operatorsManager.LoadOperators(),
                packages => _viewModel.MutationsTree.MutationPackages = new ReadOnlyCollection<PackageNode>(packages));

            _svc.Threading.ScheduleAsync(() => _typesManager.GetTypesFromAssemblies(),
                assemblies =>
                {
                    _viewModel.TypesTreeMutate.Assemblies =  new ReadOnlyCollection<AssemblyNode>(assemblies);
                    _viewModel.TypesTreeToTest.Assemblies =  new ReadOnlyCollection<AssemblyNode>(assemblies);
                    
                    if (_typesManager.IsAssemblyLoadError)
                    {
                        
                        _svc.Logging.ShowWarning(UserMessages.WarningAssemblyNotLoaded(), _log, _viewModel.View);
                    }
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