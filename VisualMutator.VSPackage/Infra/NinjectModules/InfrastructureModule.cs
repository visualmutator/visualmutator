namespace PiotrTrzpil.VisualMutator_VSPackage.Infrastructure.NinjectModules
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using CommonUtilityInfrastructure.FileSystem;
    using CommonUtilityInfrastructure.WpfUtils;
    using CommonUtilityInfrastructure.WpfUtils.Messages;

    using Ninject;
    using Ninject.Modules;

    using PiotrTrzpil.VisualMutator_VSPackage.Model;
    using PiotrTrzpil.VisualMutator_VSPackage.Views;

    using VisualMutator.Controllers;
    using VisualMutator.Infrastructure;
    using VisualMutator.ViewModels;
    using VisualMutator.Views.Abstract;

    public class InfrastructureModule : NinjectModule 
    {
        
        public override void Load()
        {
            Kernel.Bind<IMessageService>().To<MessageService>().InSingletonScope();
            Kernel.Bind<IEventService>().To<EventService>().InSingletonScope();

            Kernel.Bind<IVisualStudioConnection>().To<VisualStudioConnection>().InSingletonScope();
            Kernel.Bind<IFileSystem>().To<FileSystemService>().InSingletonScope();
       
            Kernel.Bind<ApplicationController>().ToSelf().InSingletonScope();
            Kernel.Bind<IMainControl>().To<MainControl>().InSingletonScope();
            Kernel.Bind<MainWindowViewModel>().ToSelf().InSingletonScope();
     


            var exe = new Execute();
            exe.InitializeWithDispatcher();
            Kernel.Bind<IExecute>().ToConstant(exe);

        }
    }
}