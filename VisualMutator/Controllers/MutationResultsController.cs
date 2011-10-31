namespace VisualMutator.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using CommonUtilityInfrastructure;
    using CommonUtilityInfrastructure.DependencyInjection;
    using CommonUtilityInfrastructure.WpfUtils;

    using VisualMutator.Infrastructure;
    using VisualMutator.Model;
    using VisualMutator.Model.Mutations;
    using VisualMutator.Model.Mutations.Structure;
    using VisualMutator.Model.Mutations.Types;
    using VisualMutator.Model.Tests;
    using VisualMutator.Model.Tests.TestsTree;
    using VisualMutator.ViewModels;

    public class MutationResultsController : Controller
    {
        private readonly MutationResultsViewModel _viewModel;

        private readonly IFactory<MutantsCreationController> _mutantsCreationFactory;

        private readonly IMutantsContainer _mutantsContainer;

        private readonly ITestsContainer _testsContainer;

        private readonly MutantDetailsController _mutantDetailsController;

        private readonly CommonServices _commonServices;

        private MutationTestingSession _currentSession;

        private volatile bool _isPauseRequested;

        public MutationResultsController(
            MutationResultsViewModel viewModel,
            IFactory<MutantsCreationController> mutantsCreationFactory,
            IMutantsContainer mutantsContainer,
            ITestsContainer testsContainer,
            MutantDetailsController mutantDetailsController,
            CommonServices commonServices)
        {
            _viewModel = viewModel;
            _mutantsCreationFactory = mutantsCreationFactory;
            _mutantsContainer = mutantsContainer;
            _testsContainer = testsContainer;
            _mutantDetailsController = mutantDetailsController;
            _commonServices = commonServices;


            

            _viewModel.CommandCreateNewMutants = new BasicCommand(CreateMutants,
                () => _viewModel.OperationsState.IsIn(OperationsState.None, OperationsState.Finished));
            _viewModel.CommandCreateNewMutants.UpdateOnChanged(_viewModel, () => _viewModel.OperationsState);

            _viewModel.CommandStop = new BasicCommand(PauseOperations, 
                () => _viewModel.OperationsState.IsIn(OperationsState.Mutating, OperationsState.Testing));
            _viewModel.CommandStop.UpdateOnChanged(_viewModel, () => _viewModel.OperationsState);

            _viewModel.CommandContinue = new BasicCommand(ResumeOperations,
                () => _viewModel.OperationsState == OperationsState.TestingPaused);
            _viewModel.CommandContinue.UpdateOnChanged(_viewModel, () => _viewModel.OperationsState);

            _viewModel.Operators = new BetterObservableCollection<ExecutedOperator>();


            _viewModel.RegisterPropertyChanged(() => _viewModel.SelectedMutationTreeItem).OfType<Mutant>()
                .Subscribe(mutant => _mutantDetailsController.LoadDetails(mutant, _currentSession.OriginalAssemblies));

            _viewModel.MutantDetailsViewModel = _mutantDetailsController.ViewModel;

        }

        public void PauseOperations()
        {
            _isPauseRequested = true;
            _viewModel.OperationsStateDescription = "Pausing...";
        }

        public void CreateMutants()
        {
            var mutantsCreationController = _mutantsCreationFactory.Create();
            mutantsCreationController.Run();
            
            if (mutantsCreationController.HasResults)
            {
                MutationSessionChoices choices = mutantsCreationController.Result;

                SetState(OperationsState.Mutating);

                _currentSession = new MutationTestingSession();
               
                _commonServices.Threading.ScheduleAsync(() =>
                {
                    _currentSession =  _mutantsContainer.GenerateMutantsForOperators(choices);
                },
                () =>
                {
                    _viewModel.Operators.ReplaceRange(_currentSession.MutantsGroupedByOperators);
                    RunTests(_currentSession);
                });

                
            }

           
        }

        private void SetState(OperationsState state)
        {
           _commonServices.Threading.InvokeOnGui(()=>
           {
               _viewModel.OperationsState = state;
               _viewModel.OperationsStateDescription = Functional.ValuedSwitch<OperationsState, string>(state)
                   .Case(OperationsState.None, "")
                   .Case(OperationsState.TestingPaused, "Paused")
                   .Case(OperationsState.Finished, "Finished")
                   .Case(OperationsState.Mutating, "Creating mutants...")
                   .Case(OperationsState.Testing, () => "Running tests... ({0}/{1})"
                       .Formatted(_currentSession.TestedMutants.Count,
                           _currentSession.MutantsToTest.Count + _currentSession.TestedMutants.Count))
                   .GetResult();
           });
           
        }

        private void RunTestsInternal(MutationTestingSession currentSession)
        {

            while (currentSession.MutantsToTest.Count != 0 && !_isPauseRequested)
            {
                SetState(OperationsState.Testing);
    
                Mutant mutant = currentSession.MutantsToTest.Dequeue();
                _testsContainer.RunTestsForMutant(currentSession.TestEnvironment, mutant);
                currentSession.TestedMutants.Add(mutant);
                _viewModel.UpdateTestingProgress();

                int mutantsKilled = currentSession.TestedMutants.Count(m => m.State == MutantResultState.Killed);

                currentSession.MutationScore = ((double)mutantsKilled) / currentSession.TestedMutants.Count;
                _viewModel.MutantsRatio = string.Format("Mutants killed: {0}/{1}", mutantsKilled, currentSession.TestedMutants.Count);
                _viewModel.MutationScore = string.Format("Mutation score: {0}", currentSession.MutationScore);

            }
            SetState(_isPauseRequested ? OperationsState.TestingPaused : OperationsState.Finished);

            _isPauseRequested = false;

        }

        public void RunTests(MutationTestingSession currentSession)
        {
            var allMutants = currentSession.MutantsGroupedByOperators.SelectMany(op => op.Mutants);
            currentSession.MutantsToTest = new Queue<Mutant>(allMutants);
            _viewModel.InitTestingProgress(currentSession.MutantsToTest.Count);
            
            _commonServices.Threading.ScheduleAsync(() =>
            {
                currentSession.TestEnvironment = _testsContainer.InitTestEnvironment();
                currentSession.TestedMutants = new List<Mutant>();
                RunTestsInternal(currentSession);
            });
            
        }
        public void ResumeOperations()
        {
            _commonServices.Threading.ScheduleAsync(() =>
            {
                RunTestsInternal(_currentSession);
            });
        }
        public void Stop()
        {

        }

        public void Initialize()
        {
            _viewModel.IsVisible = true;
        }

        public void Deactivate()
        {
            Stop();
            Clean();
            _viewModel.IsVisible = false;
        }

        private void Clean()
        {
            _viewModel.Operators.Clear();
        }

        public MutationResultsViewModel ViewModel
        {
            get
            {
                return _viewModel;
            }
        }
    }
}