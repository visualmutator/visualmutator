namespace VisualMutator.Model
{
    using System;
    using System.Linq;
    using System.Reflection;
    using CommandLine;
    using log4net;
    using UsefulTools.Core;

    public class OptionsModel : ModelElement
    {
        
        public OptionsModel()
        {
            WhiteCacheThreadsCount = 2;
            MutantsCacheEnabled = true;
            ProcessingThreadsCount = 3;
            OtherParams = "";
            MaxNumerOfMutantPerOperator = 100;
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

        private int _maxNumerOfMutantPerOperator;
        public int MaxNumerOfMutantPerOperator
        {
            get
            {
                return _maxNumerOfMutantPerOperator;
            }
            set
            {
                SetAndRise(ref _maxNumerOfMutantPerOperator, value, () => MaxNumerOfMutantPerOperator);
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

        private OtherParams _parsedParams;
        public OtherParams ParsedParams
        {
            set
            {
                _parsedParams = value;
            }
            get
            {
                if(_parsedParams == null)
                {
                    var options = new OtherParams();
                    if (Parser.Default.ParseArguments(OtherParams.Split(' '), options))
                    {
                        _parsedParams = options;
                        return options;
                    }
                    else
                    {
                        // var str = options.LastParserState.Errors.Select(a=>a.ToString()).Aggregate((a, b) => a.ToString() + "n" + b.ToString());
                        throw new Exception("Invalid params string in options.: ");
                    }
                }
                else
                {
                    return _parsedParams;
                }
                
            }
        }
       

    }
    public class OtherParams
    {

        [Option("loglevel", DefaultValue = "DEBUG", HelpText = "", Required = false)]
        public string LogLevel
        {
            get; set;
        }
        [Option( "debugfiles", DefaultValue = false, HelpText = "", Required = false)]
        public bool DebugFiles
        {
            get; set;
        }
        [Option( "nunitnetversion", DefaultValue = "", HelpText = "", Required = false)]
        public string NUnitNetVersion
        {
            get; set;
        }
        [Option("legacyCreation", DefaultValue = false, HelpText = "", Required = false)]
        public bool LegacyCreation
        {
            get; set;
        }
//        [ParserState]
//        public IParserState LastParserState
//        {
//            get; set;
//        }

        public override string ToString()
        {
            return string.Format("LogLevel: {0}, DebugFiles: {1}, NUnitNetVersion: {2}, LegacyCreation: {3}", LogLevel, DebugFiles, NUnitNetVersion, LegacyCreation);
        }
    }
}