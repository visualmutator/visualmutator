namespace PiotrTrzpil.VisualMutator_VSPackage.Infrastructure.NinjectModules
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using CommonUtilityInfrastructure;
    using CommonUtilityInfrastructure.DependencyInjection;
    using CommonUtilityInfrastructure.FileSystem;
    using CommonUtilityInfrastructure.Threading;
    using CommonUtilityInfrastructure.WpfUtils;

    using Ninject;
    using Ninject.Activation;
    using Ninject.Activation.Strategies;
    using Ninject.Extensions.ContextPreservation;
    using Ninject.Extensions.NamedScope;
    using Ninject.Modules;

 
  

    using VisualMutator.Controllers;
    using VisualMutator.Extensibility;
    using VisualMutator.Infrastructure;
    using VisualMutator.Model;
    using VisualMutator.Model.Decompilation;
    using VisualMutator.Model.Decompilation.CodeDifference;
    using VisualMutator.Model.Mutations;
    using VisualMutator.Model.Mutations.Operators;
    using VisualMutator.Model.Mutations.Types;
    using VisualMutator.Model.StoringMutants;
    using VisualMutator.Model.Tests;
    using VisualMutator.Model.Tests.Services;
    using VisualMutator.Model.Verification;
    using VisualMutator.ViewModels;
    using VisualMutator.Views;

   
    public class VisualMutatorModule : NinjectModule
    {


        public VisualMutatorModule()
        {
          
        }

        public override void Load()
        {
            Views();
            Infrastructure();
            MutantsCreation();
            Tests();
            Results();
        }
        private void Infrastructure()
        {
            Bind<IMessageService>().To<MessageService>().InSingletonScope();

            Bind<IEventService>().To<EventService>().InSingletonScope();
            Bind<IThreading>().To<Threading>().InSingletonScope();
            Bind<CommonServices>().ToSelf().InSingletonScope();
            Bind<IFileSystem>().To<FileSystemService>().InSingletonScope();
            Bind<IThreadPoolExecute>().To<ThreadPoolExecute>();

            var exe = new DispatcherExecute();
            exe.InitializeWithDispatcher();
            Bind<IDispatcherExecute>().ToConstant(exe);

            Kernel.InjectFuncFactory(() => DateTime.Now);


        }
        private void Views()
        {

            //VIEWS

            Bind<IMutationResultsView>().To<MutationResultsView>();
            Bind<MainViewModel>().ToSelf();


            Bind<OnlyMutantsCreationViewModel>().ToSelf();
            Bind<IOnlyMutantsCreationView>().To<OnlyMutantsCreationView>();

            Bind<SessionCreationViewModel>().ToSelf();
            Bind<ISessionCreationView>().To<SessionCreationView>();


            Bind<ChooseTestingExtensionViewModel>().ToSelf().AndFromFactory();
            Bind<IChooseTestingExtensionView>().To<ChooseTestingExtensionView>();


            Bind<MutantDetailsViewModel>().ToSelf();
            Bind<IMutantDetailsView>().To<MutantDetailsView>();

            Bind<IResultsSavingView>().To<ResultsSavingView>();
            Bind<ResultsSavingViewModel>().ToSelf();

            Bind<IMutantsCreationOptionsView>().To<MutantsCreationOptionsView>();
            Bind<MutantsCreationOptionsViewModel>().ToSelf();

            Bind<IMutantsTestingOptionsView>().To<MutantsTestingOptionsView>();
            Bind<MutantsTestingOptionsViewModel>().ToSelf();

            Bind<ITypesTreeView>().To<TypesTree>();
            Bind<TypesTreeViewModel>().ToSelf();

            Bind<IMutationsTreeView>().To<MutationsTree>();
            Bind<MutationsTreeViewModel>().ToSelf();

            Bind<ITestsSelectableTree>().To<TestsSelectableTree>();
            Bind<TestsSelectableTreeViewModel>().ToSelf();

        }


        public void MutantsCreation()
        {

            Kernel.Load(new NamedScopeModule());
            Kernel.Load(new ContextPreservationModule());
       

            Bind<ApplicationController>().ToSelf().InSingletonScope();
            Bind<MainController>().ToSelf().InSingletonScope();
            
            Bind<SessionCreationController>().ToSelf().AndFromFactory().DefinesNamedScope("Session");
            Bind<OnlyMutantsCreationController>().ToSelf().AndFromFactory().DefinesNamedScope("Session");
            Bind<SessionController>().ToSelf().AndFromFactory().InNamedScope("Session").DefinesNamedScope("InSession");
            Bind<MutantDetailsController>().ToSelf().AndFromFactory().InNamedScope("Session");
            Bind<ResultsSavingController>().ToSelf().AndFromFactory();
            
          //  Bind<IFactory<SessionController>>().ToConstant(new NinjectFactory<SessionController>(Kernel));
            //.


            Bind<IMutantsContainer>().To<MutantsContainer>().InNamedScope("Session");
            Bind<IMutantsFileManager>().To<MutantsFileManager>().InNamedScope("Session");


            Bind<ITypesManager>().To<SolutionTypesManager>().InNamedScope("Session");
            Bind<IOperatorsManager>().To<OperatorsManager>().InNamedScope("Session");
            Bind<IOperatorLoader>().To<MEFOperatorLoader>().InNamedScope("Session");
            Bind<ICommonCompilerAssemblies>().To<CommonCompilerAssemblies>().InNamedScope("Session");
            Bind<IOperatorUtils>().To<OperatorUtils>().InNamedScope("Session");

            Bind<IMutantsCache>().To<MutantsCache>().InNamedScope("Session");  
        }

        public void Tests()
        {

            Bind<ITestsContainer>().To<TestsContainer>().AndFromFactory().InNamedScope("Session");//.InNamedScope("Session");  

            Bind<NUnitTestService>().ToSelf();

            Bind<MsTestService>().ToSelf();

            Bind<INUnitWrapper>().To<NUnitWrapper>().InSingletonScope();
            Bind<IMsTestWrapper>().To<MsTestWrapper>();
            Bind<IMsTestLoader>().To<MsTestLoader>();


            Bind<IAssemblyVerifier>().To<AssemblyVerifier>().InNamedScope("Session");  

           
        }

        public void Results()
        {
            Bind<ICodeDifferenceCreator>().To<CodeDifferenceCreator>().InNamedScope("Session");
            Bind<ICodeVisualizer>().To<CodeVisualizer>().InNamedScope("Session");  

            Bind<XmlResultsGenerator>().ToSelf().InSingletonScope();
        }

     

    }
}