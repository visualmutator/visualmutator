namespace VisualMutator.Console
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Infrastructure;
    using Ninject.Modules;
    using UsefulTools.Core;
    using UsefulTools.FileSystem;
    using UsefulTools.Threading;
    public class ImmediateExecute : IDispatcherExecute
    {
        public TaskScheduler GuiScheduler { get { return TaskScheduler.Default; } }
        public SynchronizationContext GuiSyncContext { get { return SynchronizationContext.Current; } }
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
            Bind<CommonServices>().ToSelf().InSingletonScope();
            Bind<IFileSystem>().To<FileSystemService>().InSingletonScope();
            Bind<IProcesses>().To<Processes>().InSingletonScope();
            Bind<IThreadPoolExecute>().To<ThreadPoolExecute>();

            Bind<IDispatcherExecute>().ToConstant(new ImmediateExecute());
            Kernel.InjectFuncFactory(() => DateTime.Now);
        }
    }
}