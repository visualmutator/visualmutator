namespace PiotrTrzpil.VisualMutator_VSPackage.Infrastructure.NinjectModules
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Ninject.Modules;

    using PiotrTrzpil.VisualMutator_VSPackage.Controllers;
    using PiotrTrzpil.VisualMutator_VSPackage.Model.Tests;
    using PiotrTrzpil.VisualMutator_VSPackage.ViewModels;
    using PiotrTrzpil.VisualMutator_VSPackage.Views;
    using PiotrTrzpil.VisualMutator_VSPackage.Views.Abstract;

    public class UnitTestsModule : NinjectModule 
    {
        public override void Load()
        {
            Kernel.Bind<UnitTestsController>().ToSelf().InSingletonScope();



            Kernel.Bind<IUnitTestsView>().To<UnitTestsView>().InSingletonScope();
            Kernel.Bind<UnitTestsViewModel>().ToSelf().InSingletonScope();
            Kernel.Bind<ITestsContainer>().To<TestsContainer>().InSingletonScope();


        }
    }
}