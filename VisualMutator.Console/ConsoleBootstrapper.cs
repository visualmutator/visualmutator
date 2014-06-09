namespace VisualMutator.Console
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using log4net;
    using Model;
    using Ninject.Modules;
    using VisualMutator.Infrastructure;
    using VisualMutator.Infrastructure.NinjectModules;

    #endregion

    public class ConsoleBootstrapper
    {
        private readonly EnvironmentConnection _connection;
        private readonly CommandLineParser _parser;
        private static ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly Bootstrapper _boot;

 

        public ConsoleBootstrapper(EnvironmentConnection connection, CommandLineParser parser)
        {
            _connection = connection;
            _parser = parser;
            _boot = new Bootstrapper(new List<INinjectModule>() {
                new VisualMutatorModule(),
                new ConsoleInfrastructureModule(),
                new FakeViewsModule(),
                new ConsoleNinjectModule(connection)});
        }


        public object Shell
        {
            get
            {
                return _boot.Shell;
            }
        }


        public async void Initialize()
        {
            try
            {
                _boot.Initialize();
                OptionsModel optionsModel = _boot.AppController.OptionsManager.ReadOptions();
                optionsModel.WhiteCacheThreadsCount = 1;
                optionsModel.ProcessingThreadsCount = 2;
                _boot.AppController.OptionsManager.WriteOptions(optionsModel);

                _connection.Build();
                MethodIdentifier methodIdentifier;
                _connection.GetCurrentClassAndMethod(out methodIdentifier);
                

                _boot.AppController.MainController.RunMutationSession(methodIdentifier, true);
                //                for (int i = 0; i < 1000; i++)
                //                {
                //                    _boot.AppController.MainController.RunMutationSessionAuto2(methodIdentifier);
                //                }

                _boot.AppController.MainController.SessionFinishedEvents.Subscribe(_ =>
                {
                    _boot.AppController.MainController.SaveResultsAuto(_parser.ResultsPath);
                });
                Console.ReadLine();
            }
            catch (Exception e)
            {
                _log.Error(e);
            }
            Console.ReadLine();
        }
    }
}