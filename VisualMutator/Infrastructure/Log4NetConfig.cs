namespace VisualMutator.Infrastructure
{
    using System;
    using System.IO;
    using System.Reflection;

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
            string p = new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath;

            root.AddAppender(GetFileAppender(Path.GetDirectoryName(p), "Info.log", Level.Info));
            root.AddAppender(GetFileAppender(Path.GetDirectoryName(p), "Error.log", Level.Error));
            root.Repository.Configured = true;
        }

        private static FileAppender GetFileAppender(string directory, string fileName, Level threshold)
        {
            var appender = new FileAppender
            {
                Name = "File",
                AppendToFile = true,
                File = Path.Combine(directory,fileName),
                Layout = new PatternLayout("%-5level [%thread] - %date %5rms %-35.35logger{2} %-25.25method: %newline%message%newline%newline%newline%newline%newline"),
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