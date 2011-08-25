namespace PiotrTrzpil.VisualMutator_VSPackage.Infrastructure
{
    #region Usings

    using System;
    using System.IO;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Threading;

    using FileUtils;
    using FileUtils.Impl;

    using Ninject;
    using Ninject.Modules;

    using PiotrTrzpil.VisualMutator_VSPackage.Controllers;
    using PiotrTrzpil.VisualMutator_VSPackage.Infrastructure.FileSystem;
    using PiotrTrzpil.VisualMutator_VSPackage.Infrastructure.NinjectModules;
    using PiotrTrzpil.VisualMutator_VSPackage.Infrastructure.WpfUtils;
    using PiotrTrzpil.VisualMutator_VSPackage.Infrastructure.WpfUtils.Messages;
    using PiotrTrzpil.VisualMutator_VSPackage.Model;
    using PiotrTrzpil.VisualMutator_VSPackage.Model.Mutations;
    using PiotrTrzpil.VisualMutator_VSPackage.ViewModels;
    using PiotrTrzpil.VisualMutator_VSPackage.Views;
    using PiotrTrzpil.VisualMutator_VSPackage.Views.Abstract;

    #endregion

    public class Bootstrapper
    {
        private readonly ApplicationController _appController;

        private IKernel _kernel;

       
        public Bootstrapper()
        {
             SetupAssemblyResolve();
            HookGlobalExceptionHandlers();

            SetupDependencyInjection();
            
            _appController = _kernel.Get<ApplicationController>();

            VisualMutator_VSPackagePackage.MainControl = Shell;
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
        public void HookGlobalExceptionHandlers()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            Application.Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;
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

        private static void Current_DispatcherUnhandledException(
            object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show(e.ToString());
        }

        private static void CurrentDomain_UnhandledException(
            object sender, UnhandledExceptionEventArgs e)
        {
            MessageBox.Show(e.ToString());
        }

        public void InitializePackage(VisualMutator_VSPackagePackage visualMutatorVsPackagePackage)
        {
            _appController.Initialize();
        }
    }
}