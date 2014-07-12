namespace VisualMutator.Model
{
    using System;
    using System.Linq;
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
        private bool _useLegacyCreationMode;

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
                   // var str = options.LastParserState.Errors.Select(a=>a.ToString()).Aggregate((a, b) => a.ToString() + "n" + b.ToString());
                    throw new Exception("Invalid params string in options.: ");
                }
            }
        }
       

    }
    public class OtherParams
    {

        [Option("loglevel", DefaultValue = "DEBUG", HelpText = "")]
        public string LogLevel
        {
            get; set;
        }
        [Option( "debugfiles", DefaultValue = false, HelpText = "")]
        public bool DebugFiles
        {
            get; set;
        }
        [Option( "nunitnetversion", DefaultValue = "", HelpText = "")]
        public string NUnitNetVersion
        {
            get; set;
        }
        [Option("legacyCreation", DefaultValue = false, HelpText = "")]
        public bool LegacyCreation
        {
            get; set;
        }
        [ParserState]
        public IParserState LastParserState
        {
            get; set;
        }

        public override string ToString()
        {
            return string.Format("LogLevel: {0}, DebugFiles: {1}, NUnitNetVersion: {2}, LegacyCreation: {3}, LastParserState: {4}", LogLevel, DebugFiles, NUnitNetVersion, LegacyCreation, LastParserState);
        }
    }
}