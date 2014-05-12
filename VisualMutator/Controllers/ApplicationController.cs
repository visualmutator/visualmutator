namespace VisualMutator.Controllers
{
    #region

    using System;
    using System.Diagnostics;
    using System.IO;
    using System.IO.Compression;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Threading;
    using Infrastructure;
    using log4net;
    using Model.Mutations.Operators;
    using UsefulTools.Core;
    using UsefulTools.Switches;
    using Switch = UsefulTools.Switches.Switch;

    #endregion

    public class ApplicationController
    {
        private readonly MainController _mutationResultsController;
        private readonly IOperatorsManager _operatorsManager;

        private readonly IHostEnviromentConnection _hostEnviroment;

        private readonly ISettingsManager _settingsManager;

        private readonly IMessageService _messageService;

        private readonly IEventService _eventService;

        private readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private IDisposable _disp;

        public ApplicationController(
            MainController mutationResultsController,
            IOperatorsManager operatorsManager,
            IHostEnviromentConnection hostEnviroment,
            ISettingsManager settingsManager,
            IMessageService messageService,
            IEventService eventService)
        {
            _mutationResultsController = mutationResultsController;
            _operatorsManager = operatorsManager;
            _hostEnviroment = hostEnviroment;
            _settingsManager = settingsManager;
            _messageService = messageService;
            _eventService = eventService;


            HookGlobalExceptionHandlers();

            _eventService.Subscribe(this);
         
        }

        public UserControl MainView
        {
            get
            {
                return (UserControl) _mutationResultsController.ViewModel.View;
            }
        }

        public void Initialize()
        {
            _log.Info("Initializing package VisualMutator...");
            _log.Debug("Debug Test...");
            
        
            _disp = _hostEnviroment.Events.Subscribe(type =>
                Switch.On(type)
                .Case(EventType.HostOpened, ActivateOnSolutionOpened)
                .Case(EventType.HostClosed, DeactivateOnSolutionClosed)
                .ThrowIfNoMatch());

            Trace.Listeners.Add(new CustomTraceListener());

            _hostEnviroment.Initialize();
            _settingsManager.Initialize();

            LocateNUnitConsole();

            _operatorsManager.GetOperators();
        }

        private void LocateNUnitConsole()
        {
            const string key = "NUnitConsoleDirPath";
            if (!_settingsManager.ContainsKey(key) || !Directory.Exists(_settingsManager[key]))
            {
                var localDir = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);
                var nUnitConsoleZipPath = Path.Combine(localDir, "nunitconsole.zip");
                var nUnitConsoleDirPath = Path.Combine(localDir, "nunitconsole");
                if (Directory.Exists(nUnitConsoleDirPath))
                {
                    Directory.Delete(nUnitConsoleDirPath, recursive:true);
                }
                ZipFile.ExtractToDirectory(nUnitConsoleZipPath, localDir);
                _settingsManager[key] = nUnitConsoleDirPath;
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
            _messageService.ShowError(e.Exception.ToString());
            e.Handled = true;
        }

        private void CurrentDomain_UnhandledException(
            object sender, UnhandledExceptionEventArgs e)
        {
            var exception = (Exception)e.ExceptionObject;

            _messageService.ShowError(exception.ToString());

        }


        private void ActivateOnSolutionOpened()
        {
            _mutationResultsController.Initialize();
        }
        private void DeactivateOnSolutionClosed()
        {
            _mutationResultsController.Deactivate();
        }


    }
}