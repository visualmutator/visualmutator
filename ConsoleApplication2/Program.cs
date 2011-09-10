namespace ConsoleApplication2
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Xml.Linq;

    using FluentLog4Net;
    using FluentLog4Net.Layouts;

    using Mono.Cecil;
 
    using log4net;
    using log4net.Appender;
    using log4net.Core;
    using log4net.Layout;
    using log4net.Repository.Hierarchy;

    #endregion

    internal class Program
    {
        private static ILog _log;

        private static void Main(string[] args)
        {
            log4net.Util.LogLog.InternalDebugging = true;

            ConfigureLog();

            _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);


            _log.Debug("sadadsaada");

            Console.ReadLine();
        }


        private static void ConfigureLog()
        {
            var root = ((Hierarchy)LogManager.GetRepository()).Root;
            root.AddAppender(GetConsoleAppender());
            root.AddAppender(GetFileAppender(@"", "standard.log", Level.Debug));
           // root.AddAppender(GetFileAppender(@"d:\dev\huddle\log\Huddle.Sync", "error.log", Level.Warn));
            root.Repository.Configured = true;
        }

        private static FileAppender GetFileAppender(string directory, string fileName, Level threshold)
        {
            var appender = new FileAppender
            {
                Name = "File",
                AppendToFile = true,
                File = Path.Combine(directory, fileName),
                Layout = new PatternLayout(),
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