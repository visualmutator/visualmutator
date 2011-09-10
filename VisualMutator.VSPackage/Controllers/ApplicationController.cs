namespace PiotrTrzpil.VisualMutator_VSPackage.Controllers
{
    #region Usings

    using System;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Threading;

    using EnvDTE;

    using PiotrTrzpil.VisualMutator_VSPackage.Infrastructure.WpfUtils.Messages;
    using PiotrTrzpil.VisualMutator_VSPackage.Model;
    using PiotrTrzpil.VisualMutator_VSPackage.ViewModels;

    using log4net;

    #endregion

    public class ApplicationController
    {
        private readonly ILMutationsController _ilMutationsController;

        private readonly MainWindowViewModel _mainWindowVm;

        private readonly UnitTestsController _unitTestsController;

        private readonly IVisualStudioConnection _visualStudio;

        private readonly IMessageService _messageService;
        private ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public ApplicationController(
            MainWindowViewModel mainWindowVm,
            ILMutationsController ilMutationsController,
            UnitTestsController unitTestsController,
            IVisualStudioConnection visualStudio,
            IMessageService messageService
            )
        {
            _mainWindowVm = mainWindowVm;
            _ilMutationsController = ilMutationsController;
            _unitTestsController = unitTestsController;
            _visualStudio = visualStudio;
            _messageService = messageService;

            _mainWindowVm.ILMutationsView = _ilMutationsController.ILMutationsVm.View;
            _mainWindowVm.UnitTestsView = _unitTestsController.UnitTestsVm.View;

            HookGlobalExceptionHandlers();
            //  _unitTestsController.Mutants = _ilMutationsController.GeneratedMutants;
        }

        public object Shell
        {
            get
            {
                return _mainWindowVm.View;
            }
        }

        public void Initialize()
        {
            

            _visualStudio.SolutionEvents.Opened += ActivateOnSolutionOpened;
            _visualStudio.SolutionEvents.AfterClosing += DeactivateOnSolutionClosed;
            


        }
        public void HookGlobalExceptionHandlers()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            Application.Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;
        }

        private void Current_DispatcherUnhandledException(
            object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            _messageService.ShowError(e.Exception,_log );
            e.Handled = true;
        }

        private void CurrentDomain_UnhandledException(
            object sender, UnhandledExceptionEventArgs e)
        {
            var exception = (Exception)e.ExceptionObject;


            _messageService.ShowError(exception, _log);

        }


        private void ActivateOnSolutionOpened()
        {
            _ilMutationsController.Initialize();
            _unitTestsController.Initialize();
        }
        private void DeactivateOnSolutionClosed()
        {
            _ilMutationsController.Deactivate();
            _unitTestsController.Deactivate();
        }
    }
}