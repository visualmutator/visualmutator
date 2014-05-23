namespace VisualMutator.Model
{
    using System;
    using CommandLine;
    using UsefulTools.Core;

    public class OptionsModel : ModelElement
    {
        public OptionsModel()
        {
            WhiteCacheThreadsCount = Environment.ProcessorCount + 1;
            MutantsCacheEnabled = true;
            ProcessingThreadsCount = Environment.ProcessorCount + 1;
            OtherParams = "";
        }

        private bool _mutantsCacheEnabled;
        public bool MutantsCacheEnabled
        {
            get
            {
                return _mutantsCacheEnabled;
            }
            set
            {
                SetAndRise(ref _mutantsCacheEnabled, value, () => MutantsCacheEnabled);
            }
        }

        private int _whiteCacheThreadsCount;
        public int WhiteCacheThreadsCount
        {
            get
            {
                return _whiteCacheThreadsCount;
            }
            set
            {
                SetAndRise(ref _whiteCacheThreadsCount, value, () => WhiteCacheThreadsCount);
            }
        }
        private int _processingThreadsCount;
        public int ProcessingThreadsCount
        {
            get
            {
                return _processingThreadsCount;
            }
            set
            {
                SetAndRise(ref _processingThreadsCount, value, () => ProcessingThreadsCount);
            }
        }

        private string _otherParams;
        public string OtherParams
        {
            get
            {
                return _otherParams;
            }
            set
            {
                SetAndRise(ref _otherParams, value, () => OtherParams);
            }
        }
        
        public OtherParams ParsedParams
        {
            get
            {
                var options = new OtherParams();
                if (CommandLine.Parser.Default.ParseArguments(OtherParams.Split(' '), options))
                {
                    return options;
                }
                else
                {
                    throw new Exception("Invalid params string in options.");
                }
            }
        }

        
    }
    public class OtherParams
    {

        [Option('l', "loglevel", DefaultValue = "DEBUG", HelpText = "")]
        public string LogLevel
        {
            get; set;
        }
        [Option('d', "debugfiles", DefaultValue = false, HelpText = "")]
        public bool DebugFiles
        {
            get; set;
        }
        [Option('n', "nunitnetversion", DefaultValue = "", HelpText = "")]
        public string NUnitNetVersion
        {
            get; set;
        }
        [ParserState]
        public IParserState LastParserState
        {
            get; set;
        }

    }
}