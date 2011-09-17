namespace PiotrTrzpil.VisualMutator_VSPackage.Infrastructure.NinjectModules
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using FileUtils;
    using FileUtils.Impl;

    using Ninject;
    using Ninject.Modules;

    using PiotrTrzpil.VisualMutator_VSPackage.Controllers;
    using PiotrTrzpil.VisualMutator_VSPackage.Infrastructure.FileSystem;
    using PiotrTrzpil.VisualMutator_VSPackage.Infrastructure.WpfUtils;
    using PiotrTrzpil.VisualMutator_VSPackage.Infrastructure.WpfUtils.Messages;
    using PiotrTrzpil.VisualMutator_VSPackage.Model;
    using PiotrTrzpil.VisualMutator_VSPackage.ViewModels;
    using PiotrTrzpil.VisualMutator_VSPackage.Views;
    using PiotrTrzpil.VisualMutator_VSPackage.Views.Abstract;

    using VisualMutator.Infrastructure;

    public class InfrastructureModule : NinjectModule 
    {
        
        public override void Load()
        {
            Kernel.Bind<IMessageService>().To<MessageService>();
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