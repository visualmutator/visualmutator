namespace VisualMutator.Console
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Infrastructure;
    using Model;
    using UsefulTools.Paths;

    public class EnvironmentConnection : IHostEnviromentConnection
    {
        private readonly CommandLineParser _parser;

        public EnvironmentConnection(CommandLineParser parser)
        {
            _parser = parser;
            Events = Observable.Never<EventType>();
        }

        public void Initialize()
        {
        }

        public IObservable<EventType> Events { get; private set; }

        public IEnumerable<FilePathAbsolute> GetProjectAssemblyPaths()
        {
           return _parser.AssembliesPaths.Select(a => a.ToFilePathAbs());
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
            methodIdentifier = new MethodIdentifier(_parser.MethodIdentifier);
            return true;
        }

        public void Build()
        {
        }
    }
}