namespace VisualMutator.Infrastructure
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;
    using log4net;
    using Ninject;
    using Ninject.Activation.Strategies;
    using Ninject.Modules;
    using UsefulTools.Core;
    using VisualMutator.Controllers;
    using VisualMutator.Infrastructure.NinjectModules;

    #endregion

    public class Bootstrapper
    {
        private readonly IList<INinjectModule> _dependentModules;
        private readonly ApplicationController _appController;

        private IKernel _kernel;

        public IKernel Kernel
        {
            get { return _kernel; }
        }

        public ApplicationController AppController
        {
            get { return _appController; }
        }

        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);


        static Bootstrapper()
        {
            Log4NetConfig.Execute();
        }
 

        public Bootstrapper(IList<INinjectModule> dependentModules)
        {
            _dependentModules = dependentModules;
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            string version = fvi.FileVersion;
            _log.Info("Starting VisualMutator version: " + version);
            _log.Info("Starting bootstrapper.");
            try
            {

                _log.Info("Configuring dependency container.");
                SetupDependencyInjection();

                _log.Info("Executing dependency injection.");
                _appController = _kernel.Get<ApplicationController>();

            }
            catch (Exception e)
            {
                _log.Error("Error during addin initialization",e);
                if (Debugger.IsAttached)
                {
                    Debugger.Break();
                }
                else
                {
                    if(_kernel != null)
                    {
                        _kernel.Get<IMessageService>().ShowFatalError(e);
                    }
                   // MessageBox.Show(e.ToString());
                }
            }
        }
       
        public void SetupDependencyInjection()
        {
            _kernel = new StandardKernel();
            _kernel.Components.Add<IActivationStrategy, MyMonitorActivationStrategy>();
            _kernel.Load(_dependentModules);
        }
   

        public object Shell
        {
            get
            {
                return _appController.MainView;
            }
        }

        public void Initialize()
        {
            _appController.Initialize();
        }
    }
}