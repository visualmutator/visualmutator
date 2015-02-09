namespace VisualMutator.Console
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using CommandLine;
    using Model;

    public class CommandLineParser : OtherParams
    {

    
        public void ParseFrom(string[] args)
        {
            if (!Parser.Default.ParseArguments(args, this))
            {// 
                // var str = options.LastParserState.Errors.Select(a=>a.ToString()).Aggregate((a, b) => a.ToString() + "n" + b.ToString());
                throw new Exception("Invalid params string in options.: " + args);
            }
        }

        [Option("sourceThreads", DefaultValue = 2, HelpText = "The number of original source processing threads.")]
        public int SourceThreads
        {
            get; set;
        }
        [Option("mutationThreads", DefaultValue = 3, HelpText = "The number of mutant processing threads.")]
        public int MutationThreads
        {
            get; set;
        }
        [Option("methodIdentifier", DefaultValue = "null", HelpText = "An identifier of a single method to mutation test.")]
        public string MethodIdentifier
        {
            get; set;
        }
        [Option("resultsXml", HelpText = "The path to store the xml result at.")]
        public string ResultsXml
        {
            get; set;
        }
        [Option("sourceAssemblies", HelpText = "The ';'-separated paths to source assemblies to mutate.")]
        public string AssembliesPaths
        {
            get; set;
        }
        [Option("testAssemblies", HelpText = "The ';'-separated list of assemblies' names to include the tests from.")]
        public string TestAssemblies
        {
            get; set;
        }

//        [ParserState]
//        public IParserState LastParserState
//        {
//            get; set;
//        }

     
        public OtherParams OtherParams
        {
            get
            {
                var o = new OtherParams();
                o.DebugFiles = DebugFiles;
                o.LegacyCreation = LegacyCreation;
                o.LogLevel = LogLevel;
                o.NUnitNetVersion = NUnitNetVersion;
                return o;
            }
        }

        public List<string> AssembliesPathsList
        {
            get
            {
                return AssembliesPaths.Split(';').ToList();
            }
        }

        public List<string> TestAssembliesList
        {
            get
            {
                return TestAssemblies.Split(';').ToList();
            }
        }
    }
}