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
    using UsefulTools.Core;

    #endregion

    public class ApplicationController
    {
        private readonly MainController _mainController;
        private readonly IOptionsManager _optionsManager;

        private readonly IHostEnviromentConnection _hostEnviroment;

        private readonly ISettingsManager _settingsManager;


        private readonly IEventService _eventService;

        private readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public MainController MainController
        {
            get { return _mainController; }
        }

        public IOptionsManager OptionsManager
        {
            get { return _optionsManager; }
        }

        public ApplicationController(
            IBindingFactory<MainController> mainControllerFactory,
            IOptionsManager optionsManager,
            IHostEnviromentConnection hostEnviroment,
            ISettingsManager settingsManager,
            IEventService eventService)
        {
            _optionsManager = optionsManager;
            _hostEnviroment = hostEnviroment;
            _settingsManager = settingsManager;
            _eventService = eventService;

            HookGlobalExceptionHandlers();

            _eventService.Subscribe(this);

            _mainController = mainControllerFactory.CreateWithBindings(_hostEnviroment.Events);
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

            Trace.Listeners.Add(new CustomTraceListener());

            _hostEnviroment.Initialize();
            _settingsManager.Initialize();

            LocateNUnitConsole();
        }

        private void LocateNUnitConsole()
        {
            const string key = "NUnitConsoleDirPath";
            if (!_settingsManager.ContainsKey(key) || !Directory.Exists(_settingsManager[key]))
            {
                var localDir = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);
                Debug.Assert(localDir != null, "localDir != null");
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

        public void HookGlobalExceptionHandlers()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            if (Application.Current != null)
            {
                Application.Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;
            }
        }

        public void RunMutationSessionForCurrentPosition()
        {
            MainController.RunMutationSessionForCurrentPosition();
        }

        private void Current_DispatcherUnhandledException(
            object sender, DispatcherUnhandledExceptionEventArgs e)
        {
          //  _messageService.ShowError(e.Exception.ToString());
          //  e.Handled = true;
            _log.Warn("Current_DispatcherUnhandledException: "+e.Exception.Message);
        }

        private void CurrentDomain_UnhandledException(
            object sender, UnhandledExceptionEventArgs e)
        {
            var exception = (Exception)e.ExceptionObject;
            _log.Warn("CurrentDomain_UnhandledException: " + exception.Message);
            // _messageService.ShowError(exception.ToString());

        }

    }
}