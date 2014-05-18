namespace VisualMutator.Console
{
    using Infrastructure;
    using Ninject.Modules;
    using UsefulTools.Core;
    using UsefulTools.Wpf;

    public class ConsoleNinjectModule : NinjectModule
    {
        private readonly EnvironmentConnection _connection;

        public ConsoleNinjectModule(EnvironmentConnection connection)
        {
            _connection = connection;
        }

        public override void Load()
        {
            Bind<IOwnerWindowProvider>().To<FakeOwnerWindowProvider>().InSingletonScope();
            Bind<IHostEnviromentConnection>().ToConstant(_connection);
            Bind<ISettingsManager>().To<AppSettingsManager>().InSingletonScope();


        }
    }
}