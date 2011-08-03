namespace PiotrTrzpil.VisualMutator_VSPackage.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;

    using EnvDTE;

    using Microsoft.VisualStudio.Shell;

    using Ninject;

    using PiotrTrzpil.VisualMutator_VSPackage.Controllers;
    using PiotrTrzpil.VisualMutator_VSPackage.ViewModels;
    using PiotrTrzpil.VisualMutator_VSPackage.Views;
    using PiotrTrzpil.VisualMutator_VSPackage.Views.Abstract;

    using VisualMutator.Domain;

    public class Bootstrapper
    {
        private ApplicationController _appController;

        private IKernel _kernel;

        public Bootstrapper()
        {
            _kernel = new StandardKernel();

  
            _kernel.Bind<IKernel>().ToConstant(_kernel);

            _kernel.Bind<ApplicationController>().ToSelf().InSingletonScope();
    
            _kernel.Bind<IMainControl>().To<MainControl>().InSingletonScope();
            _kernel.Bind<MainWindowViewModel>().ToSelf().InSingletonScope();


            _kernel.Bind<IILMutationsView>().To<ILMutationsView>().InSingletonScope();
            _kernel.Bind<ILMutationsViewModel>().ToSelf().InSingletonScope();


            _kernel.Bind<UnitTestsController>().ToSelf().InSingletonScope();


            _kernel.Bind<IVisualStudioConnection>().To<VisualStudioConnection>();
            _kernel.Bind<IMutantGenerator>().To<MutantGenerator>();
            _kernel.Bind<ITypesManager>().To<SolutionTypesManager>();
            _kernel.Bind<IOperatorsManager>().To<OperatorsManager>();
            _kernel.Bind<IOperatorLoader>().To<MEFOperatorLoader>();
            _kernel.Bind<IUnitTestsView>().To<UnitTestsView>();
            _kernel.Bind<UnitTestsViewModel>().ToSelf().InSingletonScope();

            _appController = _kernel.Get<ApplicationController>();



            VisualMutator_VSPackagePackage.MainControl = Shell;

        }

        public object Shell
        {
            get
            {
                return _appController.Shell;
            }
        }

        public void InitializePackage(VisualMutator_VSPackagePackage visualMutatorVsPackagePackage)
        {
            _appController.Initialize();


        }

        
    }
}