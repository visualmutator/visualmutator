namespace VisualMutator.Controllers
{
    #region Usings

    using System;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Threading;

    using CommonUtilityInfrastructure.WpfUtils;
    using CommonUtilityInfrastructure.WpfUtils.Messages;

    using VisualMutator.Controllers.EventMessages;
    using VisualMutator.Infrastructure;
    using VisualMutator.ViewModels;

    using log4net;

    #endregion

    public class ApplicationController : IHandler<SwitchToUnitTestsTabEventArgs>
    {
        private readonly MutantsCreationController _mutantsCreationController;

        private readonly MainWindowViewModel _viewModel;

        private readonly UnitTestsController _unitTestsController;

        private readonly IVisualStudioConnection _visualStudio;

        private readonly IMessageService _messageService;

        private readonly IEventService _eventService;

        private ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public ApplicationController(
            MainWindowViewModel viewModel,
            MutantsCreationController mutantsCreationController,
            UnitTestsController unitTestsController,
            IVisualStudioConnection visualStudio,
            IMessageService messageService,
            IEventService eventService)
        {
            _viewModel = viewModel;
            _mutantsCreationController = mutantsCreationController;
            _unitTestsController = unitTestsController;
            _visualStudio = visualStudio;
            _messageService = messageService;
            _eventService = eventService;

            _viewModel.ILMutationsView = _mutantsCreationController.ILMutationsVm.View;
            _viewModel.UnitTestsView = _unitTestsController.UnitTestsVm.View;

            HookGlobalExceptionHandlers();

            _eventService.Subscribe(this);
         
        }

        public object Shell
        {
            get
            {
                return _viewModel.View;
            }
        }

        public void Initialize()
        {
            _visualStudio.Initialize();

            _visualStudio.SolutionOpened += ActivateOnSolutionOpened;
            _visualStudio.SolutionAfterClosing += DeactivateOnSolutionClosed;
            _visualStudio.OnBuildBegin += BuildEvents_OnBuildBegin;
            _visualStudio.OnBuildDone += BuildEvents_OnBuildDone;
            _viewModel.SelectedTab = 0;

        }

       

        private void BuildEvents_OnBuildBegin()
        {
         // _unitTestsController.
        }
        private void BuildEvents_OnBuildDone()
        {
            _unitTestsController.RefreshTestList();
        }
        public void HookGlobalExceptionHandlers()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            Application.Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;
        }

        private void Current_DispatcherUnhandledException(
            object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            _messageService.ShowFatalError(e.Exception,_log );
            e.Handled = true;
        }

        private void CurrentDomain_UnhandledException(
            object sender, UnhandledExceptionEventArgs e)
        {
            var exception = (Exception)e.ExceptionObject;


            _messageService.ShowFatalError(exception, _log);

        }


        private void ActivateOnSolutionOpened()
        {
            _mutantsCreationController.Initialize();
            _unitTestsController.Initialize();
        }
        private void DeactivateOnSolutionClosed()
        {
            _mutantsCreationController.Deactivate();
            _unitTestsController.Deactivate();
        }

        public void Handle(SwitchToUnitTestsTabEventArgs message)
        {
            _viewModel.SelectedTab = 1;
        }
    }
}