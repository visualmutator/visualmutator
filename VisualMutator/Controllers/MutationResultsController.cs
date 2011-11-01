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


    enum RequestedHaltState
    {
        Pause, Stop 
    }

    public class MutationResultsController : Controller
    {
        private readonly MutationResultsViewModel _viewModel;

        private readonly IFactory<MutantsCreationController> _mutantsCreationFactory;

        private readonly IFactory<ResultsSavingController> _resultsSavingFactory;

        private readonly IMutantsContainer _mutantsContainer;

        private readonly ITestsContainer _testsContainer;

        private readonly MutantDetailsController _mutantDetailsController;

        private readonly CommonServices _commonServices;

        private MutationTestingSession _currentSession;

    //    private volatile bool _isPauseRequested;

   //     private bool _isStopRequested;

        private RequestedHaltState? _requestedHaltState;

        public MutationResultsController(
            MutationResultsViewModel viewModel,
            IFactory<MutantsCreationController> mutantsCreationFactory,
            IFactory<ResultsSavingController> resultsSavingFactory,
            IMutantsContainer mutantsContainer,
            ITestsContainer testsContainer,
            MutantDetailsController mutantDetailsController,
            CommonServices commonServices)
        {
            _viewModel = viewModel;
            _mutantsCreationFactory = mutantsCreationFactory;
            _resultsSavingFactory = resultsSavingFactory;
            _mutantsContainer = mutantsContainer;
            _testsContainer = testsContainer;
            _mutantDetailsController = mutantDetailsController;
            _commonServices = commonServices;


            _viewModel.CommandSaveResults = new BasicCommand(SaveResults, () => 
                _viewModel.OperationsState == OperationsState.Finished)
                .UpdateOnChanged(_viewModel, () => _viewModel.OperationsState);


            _viewModel.CommandCreateNewMutants = new BasicCommand(CreateMutants,
                () => _viewModel.OperationsState.IsIn(OperationsState.None, OperationsState.Finished))
                .UpdateOnChanged(_viewModel, () => _viewModel.OperationsState);

            _viewModel.CommandPause = new BasicCommand(PauseOperations, 
                () => _viewModel.OperationsState.IsIn(OperationsState.Mutating, OperationsState.Testing))
                .UpdateOnChanged(_viewModel, () => _viewModel.OperationsState);

            _viewModel.CommandStop = new BasicCommand(StopOperations,
                () => _viewModel.OperationsState.IsIn(OperationsState.Mutating,
                    OperationsState.Testing, OperationsState.TestingPaused, OperationsState.Pausing))
                .UpdateOnChanged(_viewModel, () => _viewModel.OperationsState);

            _viewModel.CommandContinue = new BasicCommand(ResumeOperations,
                () => _viewModel.OperationsState == OperationsState.TestingPaused)
                .UpdateOnChanged(_viewModel, () => _viewModel.OperationsState);

            _viewModel.Operators = new BetterObservableCollection<ExecutedOperator>();


            _viewModel.RegisterPropertyChanged(() => _viewModel.SelectedMutationTreeItem).OfType<Mutant>()
                .Subscribe(mutant => _mutantDetailsController.LoadDetails(mutant, _currentSession.OriginalAssemblies));

            _viewModel.MutantDetailsViewModel = _mutantDetailsController.ViewModel;

        }

   
        private void SetState(OperationsState state)
        {
            _commonServices.Threading.InvokeOnGui(() =>
            {
                _viewModel.OperationsState = state;
                _viewModel.OperationsStateDescription = Functional.ValuedSwitch<OperationsState, string>(state)
                    .Case(OperationsState.None, "")
                    .Case(OperationsState.TestingPaused, "Paused")
                    .Case(OperationsState.Finished, "Finished")
                    .Case(OperationsState.Mutating, "Creating mutants...")
                    .Case(OperationsState.Pausing, "Pausing...")
                    .Case(OperationsState.Stopping, "Stopping...")
                    .Case(OperationsState.Testing, () => "Running tests... ({0}/{1})"
                        .Formatted(_currentSession.TestedMutants.Count,
                            _currentSession.MutantsToTest.Count + _currentSession.TestedMutants.Count))
                    .GetResult();
            });

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

        private void RunTestsInternal(MutationTestingSession currentSession)
        {

            while (currentSession.MutantsToTest.Count != 0 && _requestedHaltState == null)
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
            if (_requestedHaltState != null)
            {

                if (_requestedHaltState == RequestedHaltState.Pause)
                {
                    SetState( OperationsState.TestingPaused);
                }
                else
                {
                    SetState(OperationsState.Finished);
                    Finish();
                }
                _requestedHaltState = null;
            }
          //  SetState(_requestedHaltState == RequestedHaltState.Pause ? OperationsState.TestingPaused 
            //    : OperationsState.Finished);

           // _isPauseRequested = false;

        }

        private void Finish()
        {
            
        }

        public void PauseOperations()
        {
            _requestedHaltState = RequestedHaltState.Pause;
            SetState(OperationsState.Pausing);

        }
        public void ResumeOperations()
        {
            _commonServices.Threading.ScheduleAsync(() =>
            {
                RunTestsInternal(_currentSession);
            });
        }

        public void StopOperations()
        {
            if (_viewModel.OperationsState == OperationsState.TestingPaused)
            {
                Finish();
            }
            else
            {
                _requestedHaltState = RequestedHaltState.Stop;
                SetState(OperationsState.Stopping);
            }
            
        }
        public void SaveResults()
        {
            var resultsSavingController = _resultsSavingFactory.Create();


            resultsSavingController.Run(_currentSession);

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