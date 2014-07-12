namespace VisualMutator.Console
{
    using Moq;
    using Ninject.Modules;
    using Views;

    public class FakeViewsModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IMutationResultsView>().ToMethod(c => new Mock<IMutationResultsView>().Object);
            Bind<ISessionCreationView>().ToMethod(c => new Mock<ISessionCreationView>().Object);
            Bind<IMutantDetailsView>().ToMethod(c => new Mock<IMutantDetailsView>().Object);
            Bind<IResultsSavingView>().ToMethod(c => new Mock<IResultsSavingView>().Object);
            Bind<ITestsSelectableTree>().ToMethod(c => new Mock<ITestsSelectableTree>().Object);
            Bind<IMutationsTreeView>().ToMethod(c => new Mock<IMutationsTreeView>().Object);
            Bind<ITypesTreeView>().ToMethod(c => new Mock<ITypesTreeView>().Object);
            Bind<IOptionsView>().ToMethod(c => new Mock<IOptionsView>().Object);

        }
    }
}