namespace PiotrTrzpil.VisualMutator_VSPackage.Infrastructure.NinjectModules
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using CommonUtilityInfrastructure;
    using CommonUtilityInfrastructure.FileSystem;
    using CommonUtilityInfrastructure.Threading;
    using CommonUtilityInfrastructure.WpfUtils;

    using Ninject;
    using Ninject.Modules;

    using PiotrTrzpil.VisualMutator_VSPackage.Infra;
    using PiotrTrzpil.VisualMutator_VSPackage.Model;


    using VisualMutator.Controllers;
    using VisualMutator.Infrastructure;
    using VisualMutator.Model;
    using VisualMutator.Model.CodeDifference;
    using VisualMutator.Model.Mutations;
    using VisualMutator.Model.Mutations.Operators;
    using VisualMutator.Model.Mutations.Types;
    using VisualMutator.Model.Tests;
    using VisualMutator.Model.Tests.Services;
    using VisualMutator.ViewModels;
    using VisualMutator.Views;

    public class InfrastructureModule : NinjectModule 
    {
        
        public override void Load()
        {
            Kernel.Bind<IMessageService>().To<MessageService>().InSingletonScope();
          
            Kernel.Bind<IEventService>().To<EventService>().InSingletonScope();
            Kernel.Bind<IThreading>().To<Threading>().InSingletonScope();
            Kernel.Bind<CommonServices>().ToSelf().InSingletonScope();
            Kernel.Bind<IFileSystem>().To<FileSystemService>().InSingletonScope();
            Kernel.Bind<IThreadPoolExecute>().To<ThreadPoolExecute>();

            var exe = new DispatcherExecute();
            exe.InitializeWithDispatcher();
            Kernel.Bind<IDispatcherExecute>().ToConstant(exe);

            Kernel.InjectFuncFactory(() => DateTime.Now);


          
           

        }
    }

    public class ControllersAndViewsModule : NinjectModule
    {
        private readonly VisualStudioConnection _visualStudioConnection;

        public ControllersAndViewsModule(VisualStudioConnection visualStudioConnection)
        {
            _visualStudioConnection = visualStudioConnection;
        }

        public override void Load()
        {
            Kernel.Bind<IVisualStudioConnection>().ToConstant(_visualStudioConnection);
            Kernel.Bind<ISettingsManager>().ToConstant(new VisualStudioSettingsProvider(_visualStudioConnection));

            Kernel.Bind<IOwnerWindowProvider>().To<VisualStudioOwnerWindowProvider>().InSingletonScope();
            Kernel.Bind<IApplicationTitleProvider>().To<VisualMutatorTitleProvider>().InSingletonScope();




            Kernel.Bind<ApplicationController>().ToSelf().InSingletonScope();
            

            Kernel.Bind<MutantsCreationController>().ToSelf().AndFromFactory();
            Kernel.Bind<MutantsCreationViewModel>().ToSelf();
            Kernel.Bind<IMutantsCreationView>().To<MutantsCreationWindow>();


            Kernel.Bind<MutationResultsController>().ToSelf().InSingletonScope();
            Kernel.Bind<MutationResultsViewModel>().ToSelf();
            Kernel.Bind<IMutationResultsView>().To<MutationResultsView>();


    
            Kernel.Bind<MutantDetailsController>().ToSelf();
            Kernel.Bind<MutantDetailsViewModel>().ToSelf();
            Kernel.Bind<IMutantDetailsView>().To<MutantDetailsView>();



            Kernel.Bind<ResultsSavingController>().ToSelf().AndFromFactory();
            Kernel.Bind<ResultsSavingViewModel>().ToSelf();
            Kernel.Bind<IResultsSavingView>().To<ResultsSavingView>();



            Kernel.Bind<SessionController>().To<SessionController>().AndFromFactory();

        }
    }

    public class ModelModule : NinjectModule
    {


        public override void Load()
        {
            MutantsCreation();
            Tests();
            Results();
        }

 
        public void MutantsCreation()
        {

            Kernel.Bind<IMutantsContainer>().To<MutantsContainer>();//.InSingletonScope();
          
            Kernel.Bind<IAssembliesManager>().To<AssembliesManager>().InSingletonScope();

            Kernel.Bind<IAssemblyReaderWriter>().To<AssemblyReaderWriter>().InSingletonScope();


            Kernel.Bind<ITypesManager>().To<SolutionTypesManager>().AndFromFactory();//.InSingletonScope();
            Kernel.Bind<IOperatorsManager>().To<OperatorsManager>().AndFromFactory();//.InSingletonScope();
            Kernel.Bind<IOperatorLoader>().To<MEFOperatorLoader>();//.InSingletonScope();
        }

        public void Tests()
        {

            Kernel.Bind<ITestsContainer>().To<TestsContainer>().AndFromFactory();//.InSingletonScope();
            Kernel.Bind<IMutantsFileManager>().To<MutantsFileManager>();//.InSingletonScope();

            Kernel.Bind<NUnitTestService>().ToSelf();//.InSingletonScope();
            Kernel.Bind<MsTestService>().ToSelf();//.InSingletonScope();

            Kernel.Bind<INUnitWrapper>().To<NUnitWrapper>();//;.InSingletonScope();
            Kernel.Bind<IMsTestWrapper>().To<MsTestWrapper>();//.InSingletonScope();
            Kernel.Bind<IMsTestLoader>().To<MsTestLoader>();//.InSingletonScope();


            Kernel.Bind<IAssemblyVerifier>().To<AssemblyVerifier>();//.InSingletonScope();

            Kernel.Bind<IEnumerable<ITestService>>().ToConstant(CreateTestService(Kernel));


        }

        public void Results()
        {
            Kernel.Bind<ICodeDifferenceCreator>().To<CodeDifferenceCreator>().InSingletonScope();

            Kernel.Bind<XmlResultsGenerator>().ToSelf();
        }

        private IEnumerable<ITestService> CreateTestService(IKernel kernel)
        {
            return new ITestService[]
            {
                kernel.Get<NUnitTestService>(),
                kernel.Get<MsTestService>()
            };

        }


    }
}