namespace PiotrTrzpil.VisualMutator_VSPackage.Infrastructure.NinjectModules
{
    #region

    using CommonUtilityInfrastructure;
    using Model;
    using Ninject.Modules;
    using VisualMutator.Infrastructure;

    #endregion

    public class VSNinjectModule : NinjectModule
    {
        private readonly VisualStudioConnection _hostEnviromentConnection;

        public VSNinjectModule(VisualStudioConnection hostEnviromentConnection)
        {
            _hostEnviromentConnection = hostEnviromentConnection;
        }

        public override void Load()
        {
            Bind<IOwnerWindowProvider>().To<VisualStudioOwnerWindowProvider>().InSingletonScope();
            Bind<IHostEnviromentConnection>().ToConstant(_hostEnviromentConnection);
            Bind<ISettingsManager>().ToConstant(new VisualStudioSettingsProvider(_hostEnviromentConnection));


        }

    }
}