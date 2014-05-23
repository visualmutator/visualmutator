namespace VisualMutator.Console
{
    using System.Collections.Generic;
    using System.Linq;

    public class CommandLineParser
    {
        private readonly List<string> _assembliesPaths;
        private readonly string _methodIdentifier;
        private string _resultsPath;

        public CommandLineParser(string[] args)
        {
            _assembliesPaths = args[0].Split(';').ToList();
            _methodIdentifier = args[1];
            _resultsPath = args[2];
        }

        public string MethodIdentifier
        {
            get
            {
                return _methodIdentifier;
            }
        }

        public string ResultsPath
        {
            get { return _resultsPath; }
        }

        public List<string> AssembliesPaths
        {
            get
            {
                return _assembliesPaths;
            }
        }
    }
}