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
    using Tests.Mutations;
    using ViewModels;

    #endregion
    public class VisualMutatorViewsModule : NinjectModule
    {
        public override void Load()
        {
            Bind<MainViewModel>().ToSelf();
            Bind<MutantsSavingController>().ToSelf().AndFromFactory();
            Bind<MutantsSavingViewModel>().ToSelf();
            Bind<CreationViewModel>().ToSelf();
            Bind<MutantDetailsViewModel>().ToSelf();
            Bind<ResultsSavingViewModel>().ToSelf();
            Bind<MutantsCreationOptionsViewModel>().ToSelf();
            Bind<MutantsTestingOptionsViewModel>().ToSelf();
            Bind<TypesTreeViewModel>().ToSelf();
            Bind<MutationsTreeViewModel>().ToSelf();
            Bind<TestsSelectableTreeViewModel>().ToSelf();
            Bind<OptionsViewModel>().ToSelf();
        }
    }


    public class VisualMutatorModule : NinjectModule
    {

        public override void Load()
        {

            Bind<ApplicationController>().ToSelf().InSingletonScope();

            Bind<OptionsController>().ToSelf().AndFromFactory();


            Bind<ITestsService>().To<NUnitXmlTestService>().InSingletonScope();
            Bind<NUnitTestLoader>().ToSelf().InSingletonScope();
            Bind<INUnitWrapper>().To<NUnitWrapper>().InSingletonScope();

            Bind<IOptionsManager>().To<OptionsManager>().InSingletonScope();
            Bind<ContinuousConfigurator>().ToSelf().InSingletonScope();
            Bind<MainController>().ToSelf().AndFromFactory();
            Bind<WhiteCache>().ToSelf().AndFromFactory();

            Bind<IProjectClonesManager>().To<ProjectClonesManager>().AndFromFactory();

            Bind<ProjectFilesClone>().ToSelf().AndFromFactory();
            Bind<FilesManager>().ToSelf().InSingletonScope();

            Kernel.BindObjectRoot<ContinuousConfiguration>().ToSelf(ch0 => // on solution opened / rebuilt
            {
                ch0.Bind<IOperatorsManager>().To<OperatorsManager>().InSingletonScope();
                ch0.Bind<IOperatorLoader>().To<MEFOperatorLoader>().InSingletonScope();
                

                ch0.BindObjectRoot<SessionConfiguration>().ToSelf(ch1 => // on session creation
                {
                    ch1.Bind<SessionCreator>().ToSelf().AndFromFactory();
                    ch1.Bind<AutoCreationController>().ToSelf().AndFromFactory();
                    ch1.Bind<ITypesManager>().To<SolutionTypesManager>().InSingletonScope();

                    ch1.BindObjectRoot<SessionController>().ToSelf(ch2 => // on session starting
                    {
                        ch2.Bind<TestingProcess>().ToSelf().AndFromFactory();

                        ch2.BindObjectRoot<TestingMutant>().ToSelf(ch3 => // on mutant testing
                        {
                            ch3.Bind<TestsRunContext>().ToSelf().AndFromFactory();
                            ch3.Bind<INUnitExternal>().To<NUnitResultsParser>().InSingletonScope();
                        });


                        ch2.Bind<MutantDetailsController>().ToSelf().AndFromFactory();
                        ch2.Bind<ResultsSavingController>().ToSelf().AndFromFactory();
                        ch2.Bind<XmlResultsGenerator>().ToSelf().InSingletonScope();
                        ch2.Bind<IMutantsContainer>().To<MutantsContainer>().InSingletonScope();
                        ch2.Bind<IMutationExecutor>().To<MutationExecutor>().InSingletonScope();
                        ch2.Bind<IOperatorUtils>().To<OperatorUtils>().InSingletonScope();
                        ch2.Bind<IMutantsCache>().To<MutantsCache>().InSingletonScope();
                        ch2.Bind<AstFormatter>().ToSelf();
                        ch2.Bind<ITestsContainer>().To<TestsContainer>().InSingletonScope();
                        ch2.Bind<IAssemblyVerifier>().To<AssemblyVerifier>().InSingletonScope();
                        ch2.Bind<ICodeDifferenceCreator>().To<CodeDifferenceCreator>().InSingletonScope();
                        ch2.Bind<ICodeVisualizer>().To<CodeVisualizer>().InSingletonScope();
                        ch2.Bind<MutantMaterializer>().ToSelf().InSingletonScope();
                    });
                });
            });


        }


    }
}