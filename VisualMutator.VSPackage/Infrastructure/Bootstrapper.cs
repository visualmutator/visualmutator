namespace PiotrTrzpil.VisualMutator_VSPackage.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Windows;

    using EnvDTE;

    using Microsoft.VisualStudio.Shell;

    using Ninject;

    using PiotrTrzpil.VisualMutator_VSPackage.Controllers;
    using PiotrTrzpil.VisualMutator_VSPackage.ViewModels;
    using PiotrTrzpil.VisualMutator_VSPackage.Views;
    using PiotrTrzpil.VisualMutator_VSPackage.Views.Abstract;

    using VisualMutator.Domain;

    public class Bootstrapper
    {
        private ApplicationController _appController;

        private IKernel _kernel;

        static Bootstrapper()
        {
            AppDomain.CurrentDomain.AssemblyResolve += (sender,  e) =>
            {
                AssemblyName requestedName = new AssemblyName(e.Name);

                if (requestedName.Name == "VisualMutator.Extensibility")
                {
                    var p = new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath;
                    var path = Path.Combine(Path.GetDirectoryName(p), 
                        "Extensions", "VisualMutator.Extensibility.dll");

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



            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            MessageBox.Show(e.ToString());
        }

        public Bootstrapper()
        {
            _kernel = new StandardKernel();

  
            _kernel.Bind<IKernel>().ToConstant(_kernel);

            _kernel.Bind<ApplicationController>().ToSelf().InSingletonScope();
    
            _kernel.Bind<IMainControl>().To<MainControl>().InSingletonScope();
            _kernel.Bind<MainWindowViewModel>().ToSelf().InSingletonScope();


            _kernel.Bind<IILMutationsView>().To<ILMutationsView>().InSingletonScope();
            _kernel.Bind<ILMutationsViewModel>().ToSelf().InSingletonScope();


            _kernel.Bind<UnitTestsController>().ToSelf().InSingletonScope();


            _kernel.Bind<IVisualStudioConnection>().To<VisualStudioConnection>().InSingletonScope();
            _kernel.Bind<IMutantGenerator>().To<MutantGenerator>().InSingletonScope();
            _kernel.Bind<ITypesManager>().To<SolutionTypesManager>().InSingletonScope();
            _kernel.Bind<IOperatorsManager>().To<OperatorsManager>().InSingletonScope();
            _kernel.Bind<IOperatorLoader>().To<MEFOperatorLoader>().InSingletonScope();
            _kernel.Bind<IUnitTestsView>().To<UnitTestsView>().InSingletonScope();
            _kernel.Bind<UnitTestsViewModel>().ToSelf().InSingletonScope();

            _appController = _kernel.Get<ApplicationController>();



            VisualMutator_VSPackagePackage.MainControl = Shell;

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