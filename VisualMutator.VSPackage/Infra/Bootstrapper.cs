namespace PiotrTrzpil.VisualMutator_VSPackage.Infrastructure
{
    #region

    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;
    using System.Windows;
    using log4net;
    using Microsoft.VisualStudio.Shell;
    using Model;
    using Ninject;
    using Ninject.Activation.Strategies;
    using Ninject.Modules;
    using NinjectModules;
    using VisualMutator.Controllers;
    using VisualMutator.Infrastructure;
    using VisualMutator.Infrastructure.NinjectModules;

    #endregion

    [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable")]
    public class Bootstrapper
    {
        private readonly Package _package;

        private readonly ApplicationController _appController;

        private IKernel _kernel;

        private static ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);


        static Bootstrapper()
        {
            Log4NetConfig.Execute();
            EnsureApplication();

        }
 

        public Bootstrapper(Package package)
        {
            _package = package;
            

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

        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        public void SetupDependencyInjection()
        {

            var modules = new INinjectModule[]
            {
                new VisualMutatorModule(), 
                new VSNinjectModule(new VisualStudioConnection(_package)), 
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

        public void InitializePackage(VisualMutator_VSPackagePackage visualMutatorVsPackagePackage)
        {
           
            _appController.Initialize();
           
        }
    }
}