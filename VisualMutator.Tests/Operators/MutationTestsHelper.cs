namespace VisualMutator.Tests.Operators
{
    #region

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Security.Policy;
    using Extensibility;
    using log4net;
    using Microsoft.Cci;
    using Model;
    using Model.Decompilation;
    using Model.Decompilation.CodeDifference;
    using Model.Mutations;
    using Model.Mutations.MutantsTree;
    using Model.Mutations.Operators;
    using Model.Mutations.Types;
    using Model.StoringMutants;
    using Roslyn.Compilers;
    using Roslyn.Compilers.CSharp;
    using UsefulTools.CheckboxedTree;
    using UsefulTools.Core;
    using UsefulTools.ExtensionMethods;
    using UsefulTools.Paths;

    #endregion

    public class MutationTestsHelper
    {
        protected static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
     

      
        public static string CompileCodeToFile(string code)
        {
            _log.Info("Parsing test code...");
            SyntaxTree tree = SyntaxTree.ParseText(code);
            _log.Info("Creating compilation...");
            Compilation comp = Compilation.Create("VisualMutator-Test-Compilation",
                                                  new CompilationOptions(OutputKind.DynamicallyLinkedLibrary))
                .AddSyntaxTrees(tree)
                .AddReferences(new MetadataFileReference(typeof (object).Assembly.Location));

            string outputFileName = Path.Combine(Path.GetTempPath(), "VisualMutator-Test-Compilation.lib");
            var ilStream = new FileStream(outputFileName, FileMode.OpenOrCreate);
            _log.Info("Emiting file...");
            // var pdbStream = new FileStream(Path.ChangeExtension(outputFileName, "pdb"), FileMode.OpenOrCreate);
            //  _log.Info("Emiting pdb file...");

            EmitResult result = comp.Emit(ilStream);
            ilStream.Close();
            if (!result.Success)
            {
                string aggregate = result.Diagnostics.Select(a => a.Info.GetMessage()).Aggregate((a, b) => a + "\n" + b);
                throw new InvalidProgramException(aggregate);
            }
            return outputFileName;
        }
        public static CciModuleSource CreateModuleFromCode(string code)
        {
            var path = CompileCodeToFile(code);
            return new CciModuleSource(path);
        }

    }
     public class MutMod
     {
         public Mutant Mutant { get; set; }
         public IModuleSource ModulesProvider { get; set; }

         public MutMod(Mutant mutant, IModuleSource modulesProvider)
         {
             Mutant = mutant;
             ModulesProvider = modulesProvider;
         }
     }

}