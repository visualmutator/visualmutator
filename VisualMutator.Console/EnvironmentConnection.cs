namespace VisualMutator.Console
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Infrastructure;
    using Model;
    using Model.CoverageFinder;
    using UsefulTools.Paths;

    public class EnvironmentConnection : IHostEnviromentConnection
    {
        private readonly CommandLineParser _parser;
        private Subject<EventType> _events;

        public EnvironmentConnection(CommandLineParser parser)
        {
            _parser = parser;
            _events = new Subject<EventType>();
        }

        public void Initialize()
        {
        }

        public IObservable<EventType> Events
        {
            get { return _events; }
        }

        public IEnumerable<FilePathAbsolute> GetProjectAssemblyPaths()
        {
           return _parser.AssembliesPathsList.Select(a => a.ToFilePathAbs()).Concat(_parser.TestAssembliesList.Select(a => a.ToFilePathAbs()));
        }

        public string GetTempPath()
        {
            return Path.GetTempPath();
        }

        public void Test()
        {
        }

        public bool GetCurrentClassAndMethod(out MethodIdentifier methodIdentifier)
        {
            if (_parser.MethodIdentifier != "null")
            {
                methodIdentifier = new MethodIdentifier(_parser.MethodIdentifier);
            }
            else
            {
                methodIdentifier = null;
            }
            return true;
        }

        public void Build()
        {
            _events.OnNext(EventType.HostOpened);
        }

        internal void End()
        {
            _events.OnNext(EventType.HostClosed);
        }
    }
}