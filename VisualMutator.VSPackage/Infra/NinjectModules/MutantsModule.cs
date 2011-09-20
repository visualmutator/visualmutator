namespace PiotrTrzpil.VisualMutator_VSPackage.Infrastructure.NinjectModules
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Ninject.Modules;

    using PiotrTrzpil.VisualMutator_VSPackage.Controllers;
    using PiotrTrzpil.VisualMutator_VSPackage.Infrastructure.WpfUtils;
    using PiotrTrzpil.VisualMutator_VSPackage.Model.Mutations;
    using PiotrTrzpil.VisualMutator_VSPackage.ViewModels;
    using PiotrTrzpil.VisualMutator_VSPackage.Views;

    public class MutantsModule : NinjectModule 
    {
        public override void Load()
        {

            Kernel.Bind<IILMutationsView>().To<ILMutationsView>().InSingletonScope();
            Kernel.Bind<ILMutationsController>().ToSelf().InSingletonScope();



            Kernel.Bind<IMutantsContainer>().To<MutantsContainer>().InSingletonScope();
            Kernel.Bind<IMutantsFileManager>().To<MutantsFileManager>().InSingletonScope();

           
           // Kernel.Bind<ILMutationsViewModel>().ToSelf().InSingletonScope();


            Kernel.Bind<IAssemblyReaderWriter>().To<AssemblyReaderWriter>().InSingletonScope();
            Kernel.Bind<ITypesManager>().To<SolutionTypesManager>().InSingletonScope();
            Kernel.Bind<IOperatorsManager>().To<OperatorsManager>().InSingletonScope();
            Kernel.Bind<IOperatorLoader>().To<MEFOperatorLoader>().InSingletonScope();

            Kernel.InjectFuncFactory<DateTime>(() => DateTime.Now);

        }
    }
}