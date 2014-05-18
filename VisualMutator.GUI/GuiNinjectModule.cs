namespace VisualMutator.GUI
{
    using Ninject.Modules;
    using UsefulTools.Core;
    using UsefulTools.Wpf;
    using VisualMutator.Infrastructure;

    public class GuiNinjectModule : NinjectModule
    {

        public GuiNinjectModule()
        {
          
        }

        public override void Load()
        {
            //Bind<IOwnerWindowProvider>().To<WindowProvider>().InSingletonScope();
           // Bind<IHostEnviromentConnection>().To<EnvironmentConnection>().InSingletonScope();
            Bind<ISettingsManager>().To<AppSettingsManager>().InSingletonScope();


        }

    }
}