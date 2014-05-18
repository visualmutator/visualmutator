namespace VisualMutator.Console
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using log4net;
    using Model;
    using Moq;
    using Ninject.Modules;
    using PiotrTrzpil.VisualMutator_VSPackage.Infrastructure;
    using UsefulTools.Core;
    using UsefulTools.FileSystem;
    using UsefulTools.Threading;
    using UsefulTools.Wpf;
    using Views;
    using VisualMutator.Infrastructure;
    using VisualMutator.Infrastructure.NinjectModules;

    #endregion

    public class FakeExecute : IDispatcherExecute
    {
        public void OnUIThread(Action action)
        {
            action();
        }

        public void PostOnUIThread(Action action)
        {
            action();
        }

        public TaskScheduler GuiScheduler { get { return TaskScheduler.Default; } }
        public SynchronizationContext GuiSyncContext { get {return SynchronizationContext.Current;} }
    }
    public class ConsoleInfrastructureModule : NinjectModule
    {
        public override void Load()
        {
            Infrastructure();

        }
        private void Infrastructure()
        {
            Bind<IMessageService>().To<MessageService>().InSingletonScope();
            Bind<IEventService>().To<EventService>().InSingletonScope();
            Bind<IThreading>().To<Threading>().InSingletonScope();
            Bind<CommonServices>().ToSelf().InSingletonScope();
            Bind<IFileSystem>().To<FileSystemService>().InSingletonScope();
            Bind<IProcesses>().To<Processes>().InSingletonScope();
            Bind<IThreadPoolExecute>().To<ThreadPoolExecute>();

            Bind<IDispatcherExecute>().ToConstant(new FakeExecute());
            Kernel.InjectFuncFactory(() => DateTime.Now);
        }
    }
    public class FakeViewsModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IMutationResultsView>().ToConstant(new Mock<IMutationResultsView>().Object);
            Bind<IMutantsSavingView>().ToConstant(new Mock<IMutantsSavingView>().Object);
            Bind<ISessionCreationView>().ToConstant(new Mock<ISessionCreationView>().Object);
            Bind<IChooseTestingExtensionView>().ToConstant(new Mock<IChooseTestingExtensionView>().Object);
            Bind<IMutantDetailsView>().ToConstant(new Mock<IMutantDetailsView>().Object);
            Bind<IResultsSavingView>().ToConstant(new Mock<IResultsSavingView>().Object);
            Bind<ITestsSelectableTree>().ToConstant(new Mock<ITestsSelectableTree>().Object);
            Bind<IMutantsCreationOptionsView>().ToConstant(new Mock<IMutantsCreationOptionsView>().Object);
            Bind<IMutantsTestingOptionsView>().ToConstant(new Mock<IMutantsTestingOptionsView>().Object);
            Bind<IMutationsTreeView>().ToConstant(new Mock<IMutationsTreeView>().Object);
            Bind<ITypesTreeView>().ToConstant(new Mock<ITypesTreeView>().Object);
            Bind<IOptionsView>().ToConstant(new Mock<IOptionsView>().Object);

        }
    }
    public class ConsoleBootstrapper
    {
        private readonly EnvironmentConnection _connection;
        private static ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly Bootstrapper _boot;

 

        public ConsoleBootstrapper(EnvironmentConnection connection)
        {
            _connection = connection;
            _boot = new Bootstrapper(new List<INinjectModule>() {
                new VisualMutatorModule(),
                new ConsoleInfrastructureModule(),
                new FakeViewsModule(),
                new ConsoleNinjectModule(connection)});
        }


        public object Shell
        {
            get
            {
                return _boot.Shell;
            }
        }


        public void Initialize()
        {
            try
            {
                _boot.Initialize();
                MethodIdentifier methodIdentifier;
                _connection.GetCurrentClassAndMethod(out methodIdentifier);
                OptionsModel optionsModel = _boot.AppController.OptionsManager.ReadOptions();
                optionsModel.WhiteCacheThreadsCount = 0;
                optionsModel.ProcessingThreadsCount = 2;
                _boot.AppController.OptionsManager.WriteOptions(optionsModel);
                _boot.AppController.MainController.RunMutationSessionAuto(methodIdentifier);
                Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}