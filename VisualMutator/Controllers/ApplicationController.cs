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
    using Model;
    using Model.Mutations.Operators;
    using UsefulTools.Core;
    using UsefulTools.Switches;
    using Switch = UsefulTools.Switches.Switch;

    #endregion

    public class ApplicationController
    {
        private readonly MainController _mainController;
        private readonly IOptionsManager _optionsManager;
        private readonly IOperatorsManager _operatorsManager;

        private readonly IHostEnviromentConnection _hostEnviroment;

        private readonly ISettingsManager _settingsManager;

        private readonly IMessageService _messageService;

        private readonly IEventService _eventService;

        private readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private IDisposable _disp;

        public MainController MainController
        {
            get { return _mainController; }
        }

        public IOptionsManager OptionsManager
        {
            get { return _optionsManager; }
        }

        public ApplicationController(
            MainController mainController,
            IOptionsManager optionsManager,
            IOperatorsManager operatorsManager,
            IHostEnviromentConnection hostEnviroment,
            ISettingsManager settingsManager,
            IMessageService messageService,
            IEventService eventService)
        {
            _mainController = mainController;
            _optionsManager = optionsManager;
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
                return (UserControl) _mainController.ViewModel.View;
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
            if (Application.Current != null)
            {
                Application.Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;
            }
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
            _mainController.Initialize();
        }
        private void DeactivateOnSolutionClosed()
        {
            _mainController.Deactivate();
        }


    }
}