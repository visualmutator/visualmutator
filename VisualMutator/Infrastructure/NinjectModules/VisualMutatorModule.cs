namespace VisualMutator.Infrastructure.NinjectModules
{
    #region

    using System;
    using Extensibility;
    using Model.Decompilation;
    using Model.Mutations;
    using Model.StoringMutants;
    using Ninject;
    using Ninject.Extensions.ContextPreservation;
    using Ninject.Extensions.NamedScope;
    using Ninject.Modules;
    using UsefulTools.Core;
    using UsefulTools.FileSystem;
    using UsefulTools.Threading;
    using UsefulTools.Wpf;
    using VisualMutator.Controllers;
    using VisualMutator.Model;
    using VisualMutator.Model.Decompilation.CodeDifference;
    using VisualMutator.Model.Mutations.Operators;
    using VisualMutator.Model.Mutations.Types;
    using VisualMutator.Model.Tests;
    using VisualMutator.Model.Tests.Services;
    using VisualMutator.Model.Verification;
    using VisualMutator.ViewModels;
    using VisualMutator.Views;

    #endregion

    public class VisualMutatorModule : NinjectModule
    {
      //  private ChildKernel childKernel;


        public VisualMutatorModule()
        {
          
        }

        public override void Load()
        {
        //    childKernel = new ChildKernel(Kernel);
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
            Bind<IProcesses>().To<Processes>().InSingletonScope();
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

            Bind<MutantsSavingController>().ToSelf().AndFromFactory();
            Bind<MutantsSavingViewModel>().ToSelf();
            Bind<IMutantsSavingView>().To<MutantsSavingView>();

            Bind<CreationViewModel>().ToSelf();
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
            
            
            Bind<NUnitXmlTestService>().ToSelf();
            Bind<NUnitTestService>().To<NUnitXmlTestService>();


            Bind<INUnitWrapper>().To<NUnitWrapper>();
            
            Bind<INUnitExternal>().To<NUnitExternal>().AndFromFactory();

            Kernel.InjectChildFactory<CreationController>(childKernel =>
            {
                childKernel.Bind<IFileSystemManager>().To<FileSystemManager>().InSingletonScope();

                childKernel.Bind<ICciModuleSource>().To<CciModuleSource>().InSingletonScope();
                childKernel.Bind<ITypesManager>().To<SolutionTypesManager>().InSingletonScope();

                childKernel.Bind<IOperatorLoader>().To<MEFOperatorLoader>().InSingletonScope();
                childKernel.Bind<IOperatorsManager>().To<OperatorsManager>().InSingletonScope();

                childKernel.InjectChildFactory<SessionController>(ch =>
                {

                    ch.Bind<SessionController>().ToSelf().InSingletonScope();

                    ch.Bind<TestingProcess>().ToSelf().AndFromFactory();
                    ch.Bind<MutantDetailsController>().ToSelf().AndFromFactory();
                    ch.Bind<ResultsSavingController>().ToSelf().AndFromFactory();
                    ch.Bind<XmlResultsGenerator>().ToSelf().InSingletonScope();

                    ch.Bind<IMutantsContainer>().To<MutantsContainer>().InSingletonScope();
                    ch.Bind<IMutantsFileManager>().To<MutantsFileManager>().InSingletonScope();
                    
                    ch.Bind<IOperatorUtils>().To<OperatorUtils>().InSingletonScope();

                    ch.Bind<IWhiteCache>().To<WhiteCache>().InSingletonScope();
                    ch.Bind<IMutantsCache>().To<MutantsCache>().InSingletonScope();


                    ch.Bind<AstFormatter>().ToSelf();
                    ch.Bind<ITestsContainer>().To<TestsContainer>().AndFromFactory();

                    ch.Bind<IAssemblyVerifier>().To<AssemblyVerifier>().InSingletonScope();

                    ch.Bind<ICodeDifferenceCreator>().To<CodeDifferenceCreator>().InSingletonScope();
                    ch.Bind<ICodeVisualizer>().To<CodeVisualizer>().InSingletonScope();
                });
            });

           
           
       
        }

        public void Tests()
        {

         
           
        }

        public void Results()
        {
           
        }

     

    }
}