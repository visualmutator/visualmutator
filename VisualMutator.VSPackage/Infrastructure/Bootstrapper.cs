namespace PiotrTrzpil.VisualMutator_VSPackage.Infrastructure
{
    #region Usings

    using System;
    using System.IO;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Threading;

    using Ninject;

    using PiotrTrzpil.VisualMutator_VSPackage.Controllers;
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

        static Bootstrapper()
        {

           

        }

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
            _kernel = new StandardKernel();

            _kernel.Bind<IMessageService>().To<MessageService>();
            _kernel.Bind<IVisualStudioConnection>().To<VisualStudioConnection>().InSingletonScope();
            //_kernel.Bind<IKernel>().ToConstant(_kernel);

            _kernel.Bind<ApplicationController>().ToSelf().InSingletonScope();

            _kernel.Bind<IMainControl>().To<MainControl>().InSingletonScope();
            _kernel.Bind<MainWindowViewModel>().ToSelf().InSingletonScope();

            _kernel.Bind<IILMutationsView>().To<ILMutationsView>().InSingletonScope();
            _kernel.Bind<ILMutationsViewModel>().ToSelf().InSingletonScope();

            _kernel.Bind<UnitTestsController>().ToSelf().InSingletonScope();

            
            _kernel.Bind<IMutantGenerator>().To<MutantGenerator>().InSingletonScope();
            _kernel.Bind<ITypesManager>().To<SolutionTypesManager>().InSingletonScope();
            _kernel.Bind<IOperatorsManager>().To<OperatorsManager>().InSingletonScope();
            _kernel.Bind<IOperatorLoader>().To<MEFOperatorLoader>().InSingletonScope();
            _kernel.Bind<IUnitTestsView>().To<UnitTestsView>().InSingletonScope();
            _kernel.Bind<UnitTestsViewModel>().ToSelf().InSingletonScope();

            

            var exe = new Execute();
            exe.InitializeWithDispatcher();
            _kernel.Bind<IExecute>().ToConstant(exe);

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