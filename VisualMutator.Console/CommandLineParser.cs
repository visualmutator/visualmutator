namespace VisualMutator.Console
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class CommandLineParser
    {
        private readonly List<string> _assembliesPaths;
        private readonly string _methodIdentifier;
        private string _resultsPath;
        private int _whiteThreads;
        private int _mutantThreads;
        private string _otherParams;

        public CommandLineParser(string[] args)
        {
            _whiteThreads = Convert.ToInt32(args[0]);
            _mutantThreads = Convert.ToInt32(args[1]);
            _methodIdentifier = args[2];
            _resultsPath = args[3];
            _assembliesPaths = args[4].Split(';').ToList();
            _otherParams = args.Skip(5).Aggregate("",(a, b) => a + " "+b);
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

        public int WhiteThreads
        {
            get { return _whiteThreads; }
        }

        public int MutantThreads
        {
            get { return _mutantThreads; }
        }

        public string OtherParams
        {
            get { return _otherParams; }
        }
    }
}