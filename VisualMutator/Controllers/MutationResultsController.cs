namespace VisualMutator.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;
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

    using log4net;

    enum RequestedHaltState
    {
        Pause, Stop 
    }

    public class MutationResultsController : Controller
    {
        private ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly MutationResultsViewModel _viewModel;

        private readonly IFactory<MutantsCreationController> _mutantsCreationFactory;

        private readonly IFactory<ResultsSavingController> _resultsSavingFactory;

        private readonly IMutantsContainer _mutantsContainer;

        private readonly ITestsContainer _testsContainer;

        private readonly MutantDetailsController _mutantDetailsController;

        private readonly CommonServices _commonServices;

        private MutationTestingSession _currentSession;

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


            _viewModel.CommandCreateNewMutants = new BasicCommand(RunMutationSession,
                () => _viewModel.OperationsState.IsIn(OperationsState.None, OperationsState.Finished))
                .UpdateOnChanged(_viewModel, () => _viewModel.OperationsState);

            _viewModel.CommandPause = new BasicCommand(PauseOperations, 
                () => _viewModel.OperationsState.IsIn(OperationsState.Testing))
                .UpdateOnChanged(_viewModel, () => _viewModel.OperationsState);

            _viewModel.CommandStop = new BasicCommand(StopOperations,
                () => _viewModel.OperationsState.IsIn(
                    OperationsState.Testing, OperationsState.TestingPaused, OperationsState.Pausing))
                .UpdateOnChanged(_viewModel, () => _viewModel.OperationsState);

            _viewModel.CommandContinue = new BasicCommand(ResumeOperations,
                () => _viewModel.OperationsState == OperationsState.TestingPaused)
                .UpdateOnChanged(_viewModel, () => _viewModel.OperationsState);

            _viewModel.Operators = new BetterObservableCollection<ExecutedOperator>();


            _viewModel.RegisterPropertyChanged(_ => _.SelectedMutationTreeItem).OfType<Mutant>()
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
                    .Case(OperationsState.PreCheck, "Running pre-check...")
                    .Case(OperationsState.Mutating, "Creating mutants...")
                    .Case(OperationsState.Pausing, "Pausing...")
                    .Case(OperationsState.Stopping, "Stopping...")
                    .Case(OperationsState.Testing, () => "Running tests... ({0}/{1})"
                        .Formatted(_currentSession.TestedMutants.Count,
                            _currentSession.MutantsToTest.Count + _currentSession.TestedMutants.Count))
                    .GetResult();
            });

        }


        public void RunMutationSession()
        {
            var mutantsCreationController = _mutantsCreationFactory.Create();
            mutantsCreationController.Run();
            
            if (mutantsCreationController.HasResults)
            {
                MutationSessionChoices choices = mutantsCreationController.Result;
        

                SetState(OperationsState.PreCheck);

                _commonServices.Threading.ScheduleAsync(() =>
                {
                    _currentSession  = _mutantsContainer.PrepareSession(choices);
                    var changelessMutant = _mutantsContainer.CreateChangelessMutant(_currentSession);

                    _currentSession.TestEnvironment = _testsContainer.InitTestEnvironment();
                    _testsContainer.RunTestsForMutant(_currentSession,_currentSession.TestEnvironment, changelessMutant);


                    if (changelessMutant.State == MutantResultState.Error)
                    {
                        if (changelessMutant.TestSession.Exception is AssemblyVerificationException)
                        {
                            _commonServices.Logging.ShowWarning("Some of source project assemblies failed veryfication. "+
                                "Mutant verification will be disabled, also errors may occur during testing and "+
                                "while decompiling code for preview." + changelessMutant.TestSession.Exception, _log);
                            _currentSession.Options.IsMutantVerificationEnabled = false;

                        }
                        else
                        {
                            _commonServices.Logging.ShowError("Error occurred while pre-testing source assemblies. "+
                            "Mutation session cannot continue.\n\nException:\n" + changelessMutant.TestSession.Exception, _log);
                            SetState(OperationsState.Finished);
                            Finish();
                            return false;
                        }
                    }
                    else if (changelessMutant.State == MutantResultState.Killed)
                    {
                        _commonServices.Logging.ShowError("One or more tests failed on unmodified source assemblies. " 
                            + "All tests must pass before starting mutation testing process. Mutation session cannot continue." );
                        SetState(OperationsState.Finished);
                        Finish();
                        return false;
                    }

                    return true;
                },
                (cont) =>
                {

                    if (cont)
                    {
                        CreateMutants(choices);
                    }
                    
                });
            }
        }


        public void CreateMutants(MutationSessionChoices choices)
        {
            SetState(OperationsState.Mutating);

 
            _commonServices.Threading.ScheduleAsync(() =>
            {
                _mutantsContainer.GenerateMutantsForOperators(_currentSession);
            },
            () =>
            {
                _viewModel.Operators.ReplaceRange(_currentSession.MutantsGroupedByOperators);
                RunTests();
            });
            
        }


        public void RunTests()
        {
            var allMutants = _currentSession.MutantsGroupedByOperators.SelectMany(op => op.Mutants);
            _currentSession.MutantsToTest = new Queue<Mutant>(allMutants);
            _viewModel.InitTestingProgress(_currentSession.MutantsToTest.Count);
            
            _commonServices.Threading.ScheduleAsync(() =>
            {
                _currentSession.TestedMutants = new List<Mutant>();
                RunTestsInternal();
            });
            
        }

        private void RunTestsInternal()
        {

            while (_currentSession.MutantsToTest.Count != 0 && _requestedHaltState == null)
            {
                SetState(OperationsState.Testing);

                Mutant mutant = _currentSession.MutantsToTest.Dequeue();
                _testsContainer.RunTestsForMutant(_currentSession,_currentSession.TestEnvironment, mutant);
                _currentSession.TestedMutants.Add(mutant);
                _viewModel.UpdateTestingProgress();

                int mutantsKilled = _currentSession.TestedMutants.Count(m => m.State == MutantResultState.Killed);

                _currentSession.MutationScore = ((double)mutantsKilled) / _currentSession.TestedMutants.Count;
                _viewModel.MutantsRatio = string.Format("Mutants killed: {0}/{1}", mutantsKilled, _currentSession.TestedMutants.Count);
                _viewModel.MutationScore = string.Format("Mutation score: {0}", _currentSession.MutationScore);

            }
            if (_requestedHaltState != null)
            {

                Functional.Switch(_requestedHaltState)
                    .Case(RequestedHaltState.Pause, () => SetState( OperationsState.TestingPaused))
                    .Case(RequestedHaltState.Stop, () =>
                    {
                        SetState(OperationsState.Finished);
                        Finish();
                    }).Do();
                _requestedHaltState = null;
            }
            else
            {
                SetState(OperationsState.Finished);
                Finish();
            }

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
                RunTestsInternal();
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