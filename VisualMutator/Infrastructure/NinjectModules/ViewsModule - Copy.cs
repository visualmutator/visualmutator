namespace VisualMutator.Infrastructure.NinjectModules
{
    using Ninject.Modules;
    using Views;

    public class ViewsModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IMutationResultsView>().To<MutationResultsView>();
            Bind<ISessionCreationView>().To<SessionCreationView>();
            Bind<IMutantDetailsView>().To<MutantDetailsView>();
            Bind<IResultsSavingView>().To<ResultsSavingView>();
            Bind<ITestsSelectableTree>().To<TestsSelectableTree>();
            Bind<IMutationsTreeView>().To<MutationsTree>();
            Bind<ITypesTreeView>().To<TypesTree>();
            Bind<IOptionsView>().To<OptionsView>();

        }
    }
}