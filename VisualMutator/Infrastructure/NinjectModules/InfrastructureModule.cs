namespace VisualMutator.Infrastructure.NinjectModules
{
    using System;
    using Ninject.Modules;
    using UsefulTools.Core;
    using UsefulTools.FileSystem;
    using UsefulTools.Threading;
    using UsefulTools.Wpf;
    using Views;

    public class InfrastructureModule : NinjectModule
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

            var exe = new DispatcherExecute();
            exe.InitializeWithDispatcher();
            Bind<IDispatcherExecute>().ToConstant(exe);
            Kernel.InjectFuncFactory(() => DateTime.Now);
        }
    }
}