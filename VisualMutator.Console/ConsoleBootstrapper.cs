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
            Bind<IMessageService>().To<ConsoleMessageService>().InSingletonScope();
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
            Bind<IMutationResultsView>().ToMethod(c => new Mock<IMutationResultsView>().Object);
            Bind<IMutantsSavingView>().ToMethod(c => new Mock<IMutantsSavingView>().Object);
            Bind<ISessionCreationView>().ToMethod(c => new Mock<ISessionCreationView>().Object);
            Bind<IChooseTestingExtensionView>().ToMethod(c => new Mock<IChooseTestingExtensionView>().Object);
            Bind<IMutantDetailsView>().ToMethod(c => new Mock<IMutantDetailsView>().Object);
            Bind<IResultsSavingView>().ToMethod(c => new Mock<IResultsSavingView>().Object);
            Bind<ITestsSelectableTree>().ToMethod(c => new Mock<ITestsSelectableTree>().Object);
            Bind<IMutantsCreationOptionsView>().ToMethod(c => new Mock<IMutantsCreationOptionsView>().Object);
            Bind<IMutantsTestingOptionsView>().ToMethod(c => new Mock<IMutantsTestingOptionsView>().Object);
            Bind<IMutationsTreeView>().ToMethod(c => new Mock<IMutationsTreeView>().Object);
            Bind<ITypesTreeView>().ToMethod(c => new Mock<ITypesTreeView>().Object);
            Bind<IOptionsView>().ToMethod(c => new Mock<IOptionsView>().Object);

        }
    }
    public class ConsoleBootstrapper
    {
        private readonly EnvironmentConnection _connection;
        private readonly CommandLineParser _parser;
        private static ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly Bootstrapper _boot;

 

        public ConsoleBootstrapper(EnvironmentConnection connection, CommandLineParser parser)
        {
            _connection = connection;
            _parser = parser;
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
                for (int i = 0; i < 1000; i++)
                {
                    _boot.AppController.MainController.RunMutationSessionAuto2(methodIdentifier);
                }
               

                _boot.AppController.MainController.SessionFinishedEvents.Subscribe(_ =>
                {
                    _boot.AppController.MainController.SaveResultsAuto(_parser.ResultsPath);
                });
                Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}