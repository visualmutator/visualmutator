namespace PiotrTrzpil.VisualMutator_VSPackage.Infrastructure
{
    #region Usings

    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Threading;

    using Ninject;
    using Ninject.Modules;

    using PiotrTrzpil.VisualMutator_VSPackage.Infrastructure.NinjectModules;
    using PiotrTrzpil.VisualMutator_VSPackage.Model;


    using VisualMutator;
    using VisualMutator.Controllers;
    using VisualMutator.Infrastructure;

    #endregion

    public class Bootstrapper
    {
        private readonly ApplicationController _appController;

        private IKernel _kernel;

        private static log4net.ILog _log;



        static Bootstrapper()
        {
            Log4NetConfig.Execute();
            _log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            EnsureApplication();
        }
        public static void EnsureApplication()
        {
            if (Application.Current == null)
            {
                new Application();
            }
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
        public void SetupDependencyInjection()
        {

            var modules = new INinjectModule[]
            {
                new InfrastructureModule(), 
                new ControllersAndViewsModule(), 
                new ModelModule(), 
            };
           


            _kernel = new StandardKernel(modules);


        }
   

        public object Shell
        {
            get
            {
                return _appController.Shell;
            }
        }

        public void InitializePackage(VisualMutator_VSPackagePackage visualMutatorVsPackagePackage)
        {
            _appController.Initialize();
        }
    }
}