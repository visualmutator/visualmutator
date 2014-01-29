namespace VisualMutator.GUI
{
    #region Usings

    using System;
    using System.Configuration;
    using System.Diagnostics;
    using System.Reflection;
    using System.Windows;
    using Ninject;
    using Ninject.Activation.Strategies;
    using Ninject.Modules;
    using VisualMutator.Controllers;
    using VisualMutator.Infrastructure;
    using VisualMutator.Infrastructure.NinjectModules;

    #endregion

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable")]
    public class Bootstrapper
    {
    
        private readonly ApplicationController _appController;

        private IKernel _kernel;

        private static log4net.ILog _log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);


        static Bootstrapper()
        {
            Log4NetConfig.Execute();
            EnsureApplication();

        }
 

        public Bootstrapper()
        {
         
            _log.Info("Starting bootstrapper.");

   
            try
            {

                _log.Info("Configuring dependency container.");
                SetupDependencyInjection();

                _log.Info("Executing dependency injection.");
                _appController = _kernel.Get<ApplicationController>();

              //  System.Configuration.Se

ConfigurationManager.AppSettings.
                VisualMutator_VSPackagePackage.MainControl = Shell;



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
                    MessageBox.Show(e.ToString());
                }
            }


            
        }
        public static void EnsureApplication()
        {
            if (Application.Current == null)
            {
                new Application();
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        public void SetupDependencyInjection()
        {

            var modules = new INinjectModule[]
            {
                new VisualMutatorModule(), 
                new GuiNinjectModule(new VisualStudioConnection(_package)), 
            };
           


            _kernel = new StandardKernel();
            _kernel.Components.Add<IActivationStrategy, MyMonitorActivationStrategy>();




            _kernel.Load(modules);

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