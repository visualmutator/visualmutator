namespace PiotrTrzpil.VisualMutator_VSPackage.Infrastructure.NinjectModules
{
    using CommonUtilityInfrastructure;
    using CommonUtilityInfrastructure.WpfUtils;
    using Model;
    using Ninject.Modules;
    using VisualMutator.Infrastructure;


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