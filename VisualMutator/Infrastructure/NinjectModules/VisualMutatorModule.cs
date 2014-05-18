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

            Bind<INUnitExternal>().To<NUnitResultsParser>().InSingletonScope();


            Bind<IOptionsManager>().To<OptionsManager>().InSingletonScope();
            Bind<IOperatorLoader>().To<MEFOperatorLoader>().InSingletonScope();
            Bind<IOperatorsManager>().To<OperatorsManager>().InSingletonScope();

            Bind<ContinuousConfigurator>().ToSelf().InSingletonScope();

            Kernel.BindObjectRoot<ContinuousConfiguration>().ToSelf(ch0 =>
            {
                ch0.Bind<IFileSystemManager>().To<FileSystemManager>().InSingletonScope();
                ch0.Bind<WhiteCache>().ToSelf().AndFromFactory();
                ch0.Bind<DisabledWhiteCache>().ToSelf().AndFromFactory();

                ch0.BindObjectRoot<SessionConfiguration>().ToSelf(ch1 =>
                {
                    ch1.Bind<CreationController>().ToSelf().AndFromFactory();
                    ch1.Bind<SessionCreator>().ToSelf().AndFromFactory();
                    ch1.Bind<AutoCreationController>().ToSelf().AndFromFactory();

                    ch1.Bind<ICciModuleSource>().To<CciModuleSource>().InSingletonScope();
                    ch1.Bind<ITypesManager>().To<SolutionTypesManager>().InSingletonScope();


                    ch1.BindObjectRoot<SessionController>().ToSelf(ch2 =>
                    {
                        ch2.Bind<TestingProcess>().ToSelf().AndFromFactory();
                        ch2.Bind<TestingMutant>().ToSelf().AndFromFactory();
                        ch2.Bind<MutantDetailsController>().ToSelf().AndFromFactory();
                        ch2.Bind<ResultsSavingController>().ToSelf().AndFromFactory();
                        ch2.Bind<XmlResultsGenerator>().ToSelf().InSingletonScope();

                        ch2.Bind<IMutantsContainer>().To<MutantsContainer>().InSingletonScope();
                        ch2.Bind<IMutantsFileManager>().To<MutantsFileManager>().InSingletonScope();
                    
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