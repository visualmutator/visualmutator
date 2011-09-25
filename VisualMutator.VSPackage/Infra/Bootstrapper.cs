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
    using PiotrTrzpil.VisualMutator_VSPackage.Views;

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
            EnsureApplicationResources();
        }
        public static void EnsureApplicationResources()
        {
            if (Application.Current == null)
            {
                // create the Application object
                new App();
            }
        }

        public Bootstrapper()
        {
            _log.Info("Starting bootstrapper.");

            _log.Error("Error Test");

            try
            {
             //   _log.Info("Configuring assembly resolve.");
               // SetupAssemblyResolve();
               

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
                new MutantsModule(), 
                new UnitTestsModule(), 
            };
           


            _kernel = new StandardKernel(modules);


        }
      

        public void SetupAssemblyResolve()
        {
            AppDomain.CurrentDomain.AssemblyResolve += (sender, e) =>
            {
                var requestedName = new AssemblyName(e.Name);

                if (requestedName.Name == "VisualMutator.Extensibility")
                {
                    string p = new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath;
                    string path = Path.Combine(
                        Path.GetDirectoryName(p),
                        "Extensions",
                        "VisualMutator.Extensibility.dll");

                    return Assembly.LoadFrom(path);
                }
                else
                {
                    //   string[] Parts = e.Name.Split(',');
                    //   string File = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\" + Parts[0].Trim() + ".dll";

                    //    return System.Reflection.Assembly.LoadFrom(File);
                    return null;
                }
            };
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