namespace VisualMutator.Infrastructure
{
    using System;
    using System.IO;
    using System.Reflection;
    using CommonUtilityInfrastructure.Paths;
    using log4net;
    using log4net.Appender;
    using log4net.Core;
    using log4net.Layout;
    using log4net.Repository.Hierarchy;

    public class Log4NetConfig
    {

        public static void Execute()
        {
            ConfigureLog();
  
        }

        private static void ConfigureLog()
        {
            var root = ((Hierarchy)LogManager.GetRepository()).Root;
        //    root.AddAppender(GetConsoleAppender());
            FilePathAbsolute currentAssembly = new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath.ToFilePathAbs();
            File.Create(currentAssembly.GetBrotherFileWithName("visual-mutator-debug.log").Path);
            root.AddAppender(GetFileAppender(currentAssembly.GetBrotherFileWithName("visual-mutator-debug.log").Path, Level.Debug));
            root.AddAppender(GetFileAppender(currentAssembly.GetBrotherFileWithName("visual-mutator-info.log").Path, Level.Info));
            root.AddAppender(GetFileAppender(currentAssembly.GetBrotherFileWithName("visual-mutator-error.log").Path, Level.Error));
            root.AddAppender(GetConsoleAppender());
            root.Repository.Configured = true;
        }

        private static FileAppender GetFileAppender(string filePath, Level threshold)
        {
            var appender = new FileAppender
            {
                Name = "File",
                AppendToFile = true,
                File = filePath,
                Layout = new PatternLayout("%-5level [%thread] - %date %5rms %-35.35logger{2} %-25.25method: %message%newline"),
                Threshold = threshold
            };

            appender.ActivateOptions();
            return appender;
        }

        private static ConsoleAppender GetConsoleAppender()
        {
            var appender = new ConsoleAppender
            {
                Name = "Console",
                Layout = new SimpleLayout(),
                Threshold = Level.Debug
            };

            appender.ActivateOptions();
            return appender;
        }

    }
}