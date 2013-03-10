namespace VisualMutator.Tests.Operators
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using CommonUtilityInfrastructure;
    using Microsoft.Cci;
    using Model.Decompilation;
    using Model.Decompilation.CodeDifference;
    using Model.Mutations.MutantsTree;
    using Model.Mutations.Operators;
    using Model.StoringMutants;
    using Roslyn.Compilers;
    using Roslyn.Compilers.CSharp;
    using VisualMutator.Extensibility;
    using VisualMutator.Model;
    using VisualMutator.Model.Mutations;
    using VisualMutator.Model.Mutations.Types;
    using VisualMutator.Tests.Util;
    using log4net;

    public class Common
    {
        protected static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static void RunMutations(string code, IMutationOperator oper, out List<Mutant>  mutants, 
            out AssembliesProvider original, out CodeDifferenceCreator diff)
        {
            _log.Info("Common.RunMutations configuring for "+oper+"...");
                
            var cci = new CommonCompilerAssemblies();
            var utils = new OperatorUtils(cci);
          
            var container = new MutantsContainer(cci, utils );
            var visualizer = new CodeVisualizer(cci);
            var cache = new MutantsCache(container);
            

            container.DebugConfig = true;
            mutants = CreateMutants(code, oper,  container, cci);
            original = new AssembliesProvider(cci.Modules);

            cache.Initialize(original, new List<TypeIdentifier>());
            diff = new CodeDifferenceCreator(cache, visualizer);

            Console.WriteLine("ORIGINAL:");
            var listing = diff.GetListing(CodeLanguage.CSharp, original);
            Console.WriteLine(listing);

            
        }
      
        public static string CreateModule(string code)
        {
            _log.Info("Parsing test code...");
            var tree = SyntaxTree.ParseText(code);
            _log.Info("Creating compilation...");
            var comp = Compilation.Create("MyCompilation",
                new CompilationOptions(OutputKind.DynamicallyLinkedLibrary))
                .AddSyntaxTrees(tree)
                .AddReferences(new MetadataFileReference(typeof(object).Assembly.Location));

            var outputFileName = Path.Combine(Path.GetTempPath(), "MyCompilation.lib");
            var ilStream = new FileStream(outputFileName, FileMode.OpenOrCreate);
            _log.Info("Emiting file...");
           // var pdbStream = new FileStream(Path.ChangeExtension(outputFileName, "pdb"), FileMode.OpenOrCreate);
          //  _log.Info("Emiting pdb file...");

            var result = comp.Emit(ilStream);
            ilStream.Close();
            if(!result.Success)
            {
                var aggregate = result.Diagnostics.Select(a => a.Info.GetMessage()).Aggregate((a, b) => a + "\n" + b);
                throw new InvalidProgramException(aggregate);
            }
            return outputFileName;
        }
        public static List<Mutant> CreateMutants(string code, IMutationOperator operatorr,
            MutantsContainer container, CommonCompilerAssemblies cci)
        {
            
            cci.AppendFromFile(CreateModule(code));
            _log.Info("Copying assemblies...");
            var copiedModules = cci.Modules.Select(cci.Copy).Cast<IModule>().ToList();

            

            MutantsContainer.OperatorWithTargets operatorWithTargets = container.FindTargets(operatorr, 
                copiedModules, new List<TypeIdentifier>());

            var mutants = new List<Mutant>();
            foreach (MutationTarget mutationTarget in operatorWithTargets.MutationTargets.Values.SelectMany(v => v))
            {
                var exec = new ExecutedOperator("", "", operatorWithTargets.Operator);
                var mutant = new Mutant("0", exec, mutationTarget, operatorWithTargets.CommonTargets);
  
                container.ExecuteMutation(mutant, cci.Modules, new List<TypeIdentifier>(), ProgressCounter.Inactive());
                mutants.Add(mutant);
            }
            return mutants;
        }

 
    }
}