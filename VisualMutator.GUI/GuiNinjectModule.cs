namespace VisualMutator.GUI
{
    using CommonUtilityInfrastructure;
    using CommonUtilityInfrastructure.WpfUtils;
    using Ninject.Modules;
    using VisualMutator.Infrastructure;

    public class GuiNinjectModule : NinjectModule
    {

        public GuiNinjectModule()
        {
          
        }

        public override void Load()
        {
            Bind<IOwnerWindowProvider>().To<WindowProvider>().InSingletonScope();
            Bind<IHostEnviromentConnection>().To<EnvironmentConnection>().InSingletonScope();
            Bind<ISettingsManager>().To<AppSettingsManager>().InSingletonScope();


        }

    }
}