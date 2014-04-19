namespace VisualMutator.Controllers
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Threading.Tasks;
    using Infrastructure;
    using log4net;
    using Model;
    using Model.Mutations.MutantsTree;
    using UsefulTools.Core;
    using UsefulTools.DependencyInjection;
    using UsefulTools.ExtensionMethods;
    using UsefulTools.Switches;
    using UsefulTools.Wpf;
    using ViewModels;

    #endregion

    public class MainController : Controller
    {
        private ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IFactory<OptionsController> _optionsController;
        private readonly MainViewModel _viewModel;
        private readonly ContinuousConfigurator _continuousConfigurator;
        private readonly IHostEnviromentConnection _host;

        private readonly CommonServices _svc;

        private List<IDisposable> _subscriptions;
        private readonly Subject<ControlEvent> _controlSource;
        private IObjectRoot<SessionController> _currentSessionController;
        private IObjectRoot<ContinuousConfiguration> _continuousConfiguration;
        private SessionConfiguration _sessionConfiguration;


        public MainController(
            IFactory<OptionsController> optionsController,
            MainViewModel viewModel,
            ContinuousConfigurator continuousConfigurator,
            IHostEnviromentConnection host,
            CommonServices svc)
        {
            _optionsController = optionsController;
            _viewModel = viewModel;
            _continuousConfigurator = continuousConfigurator;
            _host = host;

            _controlSource = new Subject<ControlEvent>();
            _svc = svc;

            _viewModel.CommandCreateNewMutants = new SmartCommand(() => RunMutationSession(),
                () => _viewModel.OperationsState.IsIn(OperationsState.None, OperationsState.Finished, OperationsState.Error))
                .UpdateOnChanged(_viewModel, () => _viewModel.OperationsState);

            _viewModel.CommandPause = new SmartCommand(PauseOperations, 
                () => _viewModel.OperationsState.IsIn(OperationsState.Testing))
                .UpdateOnChanged(_viewModel, () => _viewModel.OperationsState);

            _viewModel.CommandStop = new SmartCommand(StopOperations,
                () => _viewModel.OperationsState.IsIn(OperationsState.Mutating, OperationsState.PreCheck,
                    OperationsState.Testing, OperationsState.TestingPaused, OperationsState.Pausing))
                .UpdateOnChanged(_viewModel, () => _viewModel.OperationsState);

            _viewModel.CommandContinue = new SmartCommand(ResumeOperations,
                () => _viewModel.OperationsState == OperationsState.TestingPaused)
                .UpdateOnChanged(_viewModel, () => _viewModel.OperationsState);

            _viewModel.CommandSaveResults = new SmartCommand(SaveResults, () =>
                _viewModel.OperationsState == OperationsState.Finished)
                .UpdateOnChanged(_viewModel, () => _viewModel.OperationsState);

            _viewModel.CommandOptions = new SmartCommand(ShowOptions);
            _viewModel.CommandTest = new SmartCommand(Test);

        }

        private void ShowOptions()
        {
            _optionsController.Create().Run();

        }

        public void RunMutationSessionForCurrentPosition()
        {
            MethodIdentifier methodIdentifier;
            if (_host.GetCurrentClassAndMethod(out methodIdentifier) && methodIdentifier.MethodName != null)
            {
                _log.Info("Showing mutation session window for: " + methodIdentifier);

                RunMutationSession(methodIdentifier);

            }
        }

        public async void RunMutationSession(MethodIdentifier methodIdentifier = null)
        {
            _host.Build();
            _log.Info("Showing mutation session window.");

            _continuousConfiguration = _continuousConfigurator.GetConfiguration();
            _sessionConfiguration = _continuousConfiguration.Get.CreateSessionConfiguration();

            try
            {
                IObjectRoot<SessionController> sessionController = 
                    await _sessionConfiguration.CreateSession(methodIdentifier);

                Clean();
                _currentSessionController = sessionController;

                _viewModel.MutantDetailsViewModel = _currentSessionController.Get.MutantDetailsController.ViewModel;

                Subscribe(_currentSessionController.Get);

                _log.Info("Starting mutation session...");
                _currentSessionController.Get.RunMutationSession(_controlSource);
            }
            catch (TaskCanceledException)
            {
                // cancelled by user
            }
       
        }
    
     

        public void PauseOperations()
        {
            SetState(OperationsState.Pausing);
            _controlSource.OnNext(new ControlEvent(ControlEventType.Pause));
        }
        public void ResumeOperations()
        {
            _controlSource.OnNext(new ControlEvent(ControlEventType.Resume));
        }

        public void StopOperations()
        {
            _controlSource.OnNext(new ControlEvent(ControlEventType.Stop));

        }
        public void SaveResults()
        {
            _currentSessionController.Get.SaveResults();
            //_controlSource.OnNext(new ControlEvent(ControlEventType.SaveResults));
        }

        public void Initialize()
        {
            _viewModel.IsVisible = true;
        }

        public void Deactivate()
        {
            _controlSource.OnNext(new ControlEvent(ControlEventType.Stop));
            Clean();
            _viewModel.IsVisible = false;
        }

        private void SessionFinished()
        {
            _continuousConfiguration.Get.Dispose();
        }

        private void Clean()
        {
            _viewModel.Clean();
            if (_currentSessionController != null)
            {
                _currentSessionController.Get.MutantDetailsController.Clean();
            }
            if (_subscriptions!=null)
            {
                foreach (var subscription in _subscriptions)
                {
                    subscription.Dispose();
                }
            }
            _currentSessionController = null;
            SetState(OperationsState.None);
        }
        
        private void Test()
        {
            _host.Test();
        }


        private void SetState(OperationsState state)
        {
            _svc.Threading.InvokeOnGui(() =>
            {
                if (_viewModel.OperationsState != state)
                {
                    _viewModel.OperationsState = state;
                    _viewModel.OperationsStateDescription = FunctionalExt.ValuedSwitch<OperationsState, string>(state)
                        .Case(OperationsState.None, "")
                        .Case(OperationsState.TestingPaused, "Paused")
                        .Case(OperationsState.Finished, "Finished")
                        .Case(OperationsState.PreCheck, "Pre-check...")
                        .Case(OperationsState.Mutating, "Creating mutants...")
                        .Case(OperationsState.Pausing, "Pausing...")
                        .Case(OperationsState.Stopping, "Stopping...")
                        .Case(OperationsState.SavingMutants, "Saving mutants...")
                        .Case(OperationsState.Error, "Error occurred.")
                        .GetResult();
                }
            });
        }

        public void Subscribe(SessionController sessionController)
        {
            _subscriptions = new List<IDisposable>
            {
                sessionController.SessionEventsObservable
                    .OfType<MinorSessionUpdateEventArgs>()
                    .Where(e => e.EventType == OperationsState.Finished 
                            || e.EventType == OperationsState.Error)
                    .Subscribe(args =>
                    {
                        SessionFinished();
                    }),
                sessionController.SessionEventsObservable
                    .OfType<MinorSessionUpdateEventArgs>()
                    .Subscribe(args =>
                    {
                        SetState(args.EventType);
                        Switch.On(args.ProgressUpdateMode)
                            .Case(ProgressUpdateMode.SetValue, () =>
                            {
                                _viewModel.IsProgressIndeterminate = false;
                                _viewModel.Progress = args.PercentCompleted;
                            })
                            .Case(ProgressUpdateMode.Indeterminate, () =>
                            {
                                _viewModel.IsProgressIndeterminate = true;
                            })
                            .Case(ProgressUpdateMode.PreserveValue, ()=>
                            {
                               
                            });
                        
                    }),

                sessionController.SessionEventsObservable
                 .OfType<MutationFinishedEventArgs>()
                 .Subscribe(args => _viewModel.MutantAssemblies.ReplaceRange(args.MutantsGrouped)),

                sessionController.SessionEventsObservable
                 .OfType<TestingProgressEventArgs>()
                 .Subscribe(args =>
                 {
                     _svc.Threading.InvokeOnGui(()=>
                     {
                         _viewModel.OperationsState = OperationsState.Testing;
                         _viewModel.OperationsStateDescription = "Running tests... ({0}/{1})"
                             .Formatted(args.NumberOfAllMutantsTested + 1,
                                 args.NumberOfAllMutants);

                         _viewModel.MutantsRatio = string.Format("Mutants killed: {0}/{1}", args.NumberOfMutantsKilled, args.NumberOfAllMutantsTested);
                         _viewModel.MutationScore = string.Format(@"Mutation score: {0}%", args.MutationScore.AsPercentageOf(1.0d));
                         _viewModel.Progress = args.NumberOfAllMutantsTested.AsPercentageOf(args.NumberOfAllMutants);
                     });
                 }),

               _viewModel.WhenPropertyChanged(vm => vm.SelectedMutationTreeItem).OfType<Mutant>()
                   .Subscribe(x =>
                   {
                       sessionController.LoadDetails(x);
                       sessionController.TestWithHighPriority(x);
                   } ),
               _viewModel.WhenPropertyChanged(vm => vm.SelectedMutationTreeItem).Where(i => !(i is Mutant))
                   .Subscribe(x => sessionController.CleanDetails()),
               _viewModel.WhenPropertyChanged(vm => vm.SelectedMutationTreeItem).Where(i => !(i is Mutant))
                   .Subscribe(x => sessionController.CleanDetails())
            };

        }

        public MainViewModel ViewModel
        {
            get
            {
                return _viewModel;
            }
        }

       
    }
}