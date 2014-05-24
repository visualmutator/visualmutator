namespace VisualMutator.Infrastructure.NinjectModules
{
    #region

    using Extensibility;
    using Model.Decompilation;
    using Model.Mutations;
    using Model.StoringMutants;
    using Ninject.Modules;
    using Controllers;
    using Model;
    using Model.Decompilation.CodeDifference;
    using Model.Mutations.Operators;
    using Model.Mutations.Types;
    using Model.Tests;
    using Model.Tests.Services;
    using Model.Verification;
    using ViewModels;

    #endregion

    public class VisualMutatorModule : NinjectModule
    {

        public override void Load()
        {
            Views();
            MutantsCreation();
        }
       
        private void Views()
        {

            //VIEWS
            Bind<MainViewModel>().ToSelf();
            Bind<MutantsSavingController>().ToSelf().AndFromFactory();
            Bind<MutantsSavingViewModel>().ToSelf();
            Bind<CreationViewModel>().ToSelf();
            Bind<ChooseTestingExtensionViewModel>().ToSelf().AndFromFactory();
            Bind<MutantDetailsViewModel>().ToSelf();
            Bind<ResultsSavingViewModel>().ToSelf();
            Bind<MutantsCreationOptionsViewModel>().ToSelf();
            Bind<MutantsTestingOptionsViewModel>().ToSelf();
            Bind<TypesTreeViewModel>().ToSelf();
            Bind<MutationsTreeViewModel>().ToSelf();
            Bind<TestsSelectableTreeViewModel>().ToSelf();
            Bind<OptionsViewModel>().ToSelf();
        }


        public void MutantsCreation()
        {

          //  Kernel.Load(new NamedScopeModule());
         //   Kernel.Load(new ContextPreservationModule());

            Bind<ApplicationController>().ToSelf().InSingletonScope();
            Bind<MainController>().ToSelf().InSingletonScope();
            Bind<OptionsController>().ToSelf().AndFromFactory();
            Bind<NUnitXmlTestService>().ToSelf();
            Bind<NUnitTestService>().To<NUnitXmlTestService>();
            Bind<NUnitTester>().ToSelf().AndFromFactory();
            Bind<INUnitWrapper>().To<NUnitWrapper>();
            Bind<INUnitExternal>().To<NUnitResultsParser>();
            Bind<IOptionsManager>().To<OptionsManager>().InSingletonScope();
            Bind<ContinuousConfigurator>().ToSelf().InSingletonScope();

            Kernel.BindObjectRoot<ContinuousConfiguration>().ToSelf(ch0 => // on solution opened / rebuild
            {
                ch0.Bind<IOperatorsManager>().To<OperatorsManager>().InSingletonScope();
                ch0.Bind<IOperatorLoader>().To<MEFOperatorLoader>();
                ch0.Bind<IProjectClonesManager>().To<ProjectClonesManager>().InSingletonScope();
                ch0.Bind<ProjectFilesClone>().ToSelf().AndFromFactory();
                ch0.Bind<FilesManager>().ToSelf().InSingletonScope();
                ch0.Bind<WhiteCache>().ToSelf().AndFromFactory();
                ch0.Bind<DisabledWhiteCache>().ToSelf().AndFromFactory();

                ch0.BindObjectRoot<SessionConfiguration>().ToSelf(ch1 => // on session creation
                {   
                    ch1.Bind<SessionCreator>().ToSelf().AndFromFactory();
                    ch1.Bind<AutoCreationController>().ToSelf().AndFromFactory();
                    //ch1.Bind<ICciModuleSource>().To<CciModuleSource>().InSingletonScope();
                    ch1.Bind<ITypesManager>().To<SolutionTypesManager>().InSingletonScope();

                    ch1.BindObjectRoot<SessionController>().ToSelf(ch2 => // on session starting
                    {
                        ch2.Bind<TestingProcess>().ToSelf().AndFromFactory();
                        ch2.Bind<TestingMutant>().ToSelf().AndFromFactory();
                        ch2.Bind<MutantDetailsController>().ToSelf().AndFromFactory();
                        ch2.Bind<ResultsSavingController>().ToSelf().AndFromFactory();
                        ch2.Bind<XmlResultsGenerator>().ToSelf().InSingletonScope();
                        ch2.Bind<IMutantsContainer>().To<MutantsContainer>().InSingletonScope();
                        ch2.Bind<IOperatorUtils>().To<OperatorUtils>().InSingletonScope();
                        ch2.Bind<IMutantsCache>().To<MutantsCache>().InSingletonScope();
                        ch2.Bind<AstFormatter>().ToSelf();
                        ch2.Bind<ITestsContainer>().To<TestsContainer>().AndFromFactory();
                        ch2.Bind<IAssemblyVerifier>().To<AssemblyVerifier>().InSingletonScope();
                        ch2.Bind<ICodeDifferenceCreator>().To<CodeDifferenceCreator>().InSingletonScope();
                        ch2.Bind<ICodeVisualizer>().To<CodeVisualizer>().InSingletonScope();
                    });
                });
            });



        }

       
        
     

    }
}