namespace PiotrTrzpil.VisualMutator_VSPackage.Infrastructure.NinjectModules
{
    using CommonUtilityInfrastructure;
    using CommonUtilityInfrastructure.WpfUtils;
    using Model;
    using Ninject.Modules;
    using VisualMutator.Infrastructure;


    public class GuiNinjectModule : NinjectModule
    {

        public GuiNinjectModule()
        {
          
        }

        public override void Load()
        {
            Bind<IOwnerWindowProvider>().To<VisualStudioOwnerWindowProvider>().InSingletonScope();
            Bind<IHostEnviromentConnection>().ToConstant(_hostEnviromentConnection);
            Bind<ISettingsManager>().ToConstant(new VisualStudioSettingsProvider(_hostEnviromentConnection));


        }

    }
}