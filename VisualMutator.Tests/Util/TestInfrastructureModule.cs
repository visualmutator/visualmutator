namespace VisualMutator.Tests.Util
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Ninject.Modules;
    using UsefulTools.Core;
    using UsefulTools.FileSystem;
    using UsefulTools.Threading;
    using VisualMutator.Infrastructure;
    using VisualMutator.Infrastructure.NinjectModules;

    public class ImmediateExecute : IDispatcherExecute
    {
        public TaskScheduler GuiScheduler
        {
            get
            {
                return TaskScheduler.Default;
            }
        }
        public SynchronizationContext GuiSyncContext
        {
            get
            {
                return SynchronizationContext.Current;
            }
        }
    }

    public class IntegrationTestModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IMessageService>().To<ConsoleMessageService>().InSingletonScope();
            Bind<IEventService>().To<EventService>().InSingletonScope();
            Bind<CommonServices>().ToSelf().InSingletonScope();
            Bind<IFileSystem>().To<FileSystemService>().InSingletonScope();
            Bind<IProcesses>().To<Processes>().InSingletonScope();
            Bind<IThreadPoolExecute>().To<ThreadPoolExecute>();

            var exe = new ImmediateExecute();
            Bind<IDispatcherExecute>().ToConstant(exe);
            Kernel.InjectFuncFactory(() => DateTime.Now);
        }
    }
    public class UnitTestModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IMessageService>().To<ConsoleMessageService>().InSingletonScope();
            Bind<IEventService>().To<EventService>().InSingletonScope();
            Bind<CommonServices>().ToSelf().InSingletonScope();
           // Bind<IFileSystem>().To<FileSystemService>().InSingletonScope();
            //Bind<IProcesses>().To<Processes>().InSingletonScope();
            Bind<IThreadPoolExecute>().To<ThreadPoolExecute>();

            var exe = new ImmediateExecute();
            Bind<IDispatcherExecute>().ToConstant(exe);
            //Kernel.InjectFuncFactory(() => DateTime.Now);
        }
      
    }
}