namespace VisualMutator.Controllers
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Threading.Tasks;

    using CommonUtilityInfrastructure;
    using CommonUtilityInfrastructure.Threading;
    using CommonUtilityInfrastructure.WpfUtils;
    using CommonUtilityInfrastructure.WpfUtils.Messages;

    using VisualMutator.Controllers.EventMessages;
    using VisualMutator.Infrastructure;
    using VisualMutator.Infrastructure.Factories;
    using VisualMutator.Model.Mutations;
    using VisualMutator.Model.Mutations.Operators;
    using VisualMutator.Model.Mutations.Types;
    using VisualMutator.ViewModels;
    using VisualMutator.Views;

    using log4net;

    #endregion

    public class MutantsCreationController : Controller
    {
      
     
        private readonly IMutantsContainer _mutantsContainer;

        private readonly IOperatorsManager _operatorsManager;

        private readonly Services _services;

        private readonly ITypesManager _typesManager;

        private readonly ILMutationsViewModel _viewModel;

        private ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public MutantsCreationController(
            ILMutationsViewModel viewModel,
            IMutantsContainer mutantsContainer,
            ITypesManager typesManager,
            IOperatorsManager operatorsManager,
            Services services)
        {
            _viewModel = viewModel;
    
            _mutantsContainer = mutantsContainer;
            _typesManager = typesManager;
            _operatorsManager = operatorsManager;
            _services = services;
           
          
            _viewModel.CommandMutate = new BasicCommand(Mutate, CanExecute);
            _viewModel.CommandMutate.UpdateOnCollectionChanged(_viewModel, _viewModel.Assemblies);
            _viewModel.CommandMutate.UpdateOnChanged(_viewModel, () => _viewModel.IsMutationOngoing);
            _viewModel.CommandMutate.UpdateOnChanged(_viewModel, () => _viewModel.AreTypesLoading);

            _viewModel.CommandRefresh = new BasicCommand(Refresh, CanExecute);
            _viewModel.CommandMutate.UpdateOnCollectionChanged(_viewModel, _viewModel.Assemblies);
            _viewModel.CommandMutate.UpdateOnChanged(_viewModel, () => _viewModel.IsMutationOngoing);
            _viewModel.CommandMutate.UpdateOnChanged(_viewModel, () => _viewModel.AreTypesLoading);

            _viewModel.Assemblies = new BetterObservableCollection<AssemblyNode>();
            _viewModel.MutationPackages = _operatorsManager.OperatorPackages;

           
            _viewModel.CommandLoadLastMutant = new BasicCommand(LoadLastMutant);

           
        }

        public void LoadLastMutant()
        {
            _services.EventPassing.Publish(new LoadLastCreatedMutantEventArgs());
            _services.EventPassing.Publish(new SwitchToUnitTestsTabEventArgs());


        }

       

        private bool CanExecute()
        {
            return _viewModel.Assemblies.Count != 0 && !_viewModel.IsMutationOngoing && !_viewModel.AreTypesLoading;
        }

        public void Initialize()
        {
            _operatorsManager.LoadOperators();
            _viewModel.IsVisible = true;
            _mutantsContainer.LoadSessions();
            Refresh();
        }

        public void Deactivate()
        {
            
            _viewModel.IsVisible = false;
            _viewModel.Assemblies.Clear();
            _mutantsContainer.Clear();
           
        }

        public void Mutate()
        {
            
            _viewModel.IsMutationOngoing = true;
            _viewModel.ClearMutationLog();
            _viewModel.MutationLog("Starting mutation.");


            Action<string> mutationLog = _services.Threading.ActionOnGui<string>(text => _viewModel.MutationLog(text));

            _services.Threading.ScheduleAsync(
                () =>
                {
                    return _mutantsContainer.GenerateMutant("Mutant", mutationLog);
                },
                onGui: result =>
                {
                    _mutantsContainer.AddMutant(result);
                    _viewModel.MutationLog("Mutation complete.");
                },
                onFinally: () => _viewModel.IsMutationOngoing = false);

           
        }

        public void Refresh()
        {
            RefreshTypes();
            
        }

        public void RefreshTypes()
        {
            _viewModel.AreTypesLoading = true;
            _viewModel.Assemblies.Clear();

            _services.Threading.ScheduleAsync(
              () =>
              {
                  return _typesManager.GetTypesFromAssemblies();
              },
              onGui: result =>
              {
                  _viewModel.Assemblies.ReplaceRange(result);
              },
              onFinally: () =>_viewModel.AreTypesLoading = false);

        }


        public ILMutationsViewModel ILMutationsVm
        {
            get
            {
                return _viewModel;
            }
        }

    }
}