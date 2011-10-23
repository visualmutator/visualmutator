namespace VisualMutator.Controllers
{
    #region Usings

    using System;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Threading;

    using CommonUtilityInfrastructure.WpfUtils;
    using CommonUtilityInfrastructure.WpfUtils.Messages;


    using VisualMutator.Infrastructure;
    using VisualMutator.ViewModels;

    using log4net;

    #endregion

    public class ApplicationController
    {
        private readonly MutationResultsController _mutationResultsController;

        private readonly IVisualStudioConnection _visualStudio;

        private readonly IMessageService _messageService;

        private readonly IEventService _eventService;

        private ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public ApplicationController(
            MutationResultsController mutationResultsController,
            IVisualStudioConnection visualStudio,
            IMessageService messageService,
            IEventService eventService)
        {
            _mutationResultsController = mutationResultsController;
            _visualStudio = visualStudio;
            _messageService = messageService;
            _eventService = eventService;


            HookGlobalExceptionHandlers();

            _eventService.Subscribe(this);
         
        }

        public object Shell
        {
            get
            {
                return _mutationResultsController.ViewModel.View;
            }
        }

        public void Initialize()
        {
            _log.Info("Initializing package...");
            _visualStudio.Initialize();

            _visualStudio.SolutionOpened += ActivateOnSolutionOpened;
            _visualStudio.SolutionAfterClosing += DeactivateOnSolutionClosed;
            _visualStudio.OnBuildBegin += BuildEvents_OnBuildBegin;
            _visualStudio.OnBuildDone += BuildEvents_OnBuildDone;
        

            if (_visualStudio.IsSolutionOpen)
            {
                ActivateOnSolutionOpened();
            }

        }

       

        private void BuildEvents_OnBuildBegin()
        {
         // _unitTestsController.
        }
        private void BuildEvents_OnBuildDone()
        {
           // _mutantsCreationController.RefreshTypes();
            
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
            _mutationResultsController.Initialize();
           //// _testingAndManagingController.Initialize();
        }
        private void DeactivateOnSolutionClosed()
        {
            _mutationResultsController.Deactivate();
           // _testingAndManagingController.Deactivate();
        }


    }
}