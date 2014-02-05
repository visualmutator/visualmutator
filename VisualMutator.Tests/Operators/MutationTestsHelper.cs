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

        public const String DsaPath = @"C:\PLIKI\Dropbox\++Inzynierka\VisualMutator\Dsa.dll";
        public const String DsaTestsPath = @"C:\PLIKI\Dropbox\++Inzynierka\VisualMutator\Dsa.Test.dll";
        public const String DsaTestsPath2 = @"C:\PLIKI\Dropbox\++Inzynierka\VisualMutator\Dsa.Test2.dll";
        

        public static void RunMutations(string code, IMutationOperator oper, out List<Mutant> mutants,
         
            out CodeDifferenceCreator diff)
        {
            RunMutationsFromFile(CreateModule(code), oper, out mutants, out diff);
        }
        public static void RunMutationsFromFile(string filePath, IMutationOperator oper, out List<Mutant> mutants,
            out CodeDifferenceCreator diff)
        {
            _log.Info("MutationTests.RunMutations configuring for " + oper + "...");

            var cci = new CciModuleSource();
            var utils = new OperatorUtils(cci);

            var container = new MutantsContainer(cci, utils);
            var visualizer = new CodeVisualizer(cci);
            var cache = new MutantsCache(new DisabledWhiteCache(), container);
            cache.WhiteCache.Initialize(new[]{filePath});
            IModule module = cci.AppendFromFile(filePath);


            cache.setDisabled(disableCache: false);
            diff = new CodeDifferenceCreator(cache, visualizer);

            container.DebugConfig = true;
            var mutmods = CreateMutants(oper, container, cci, cache, 100);
            mutants = mutmods.Select(m => m.Mutant).ToList();

        }

        public static void DebugTraverse(string code)
        {
            DebugTraverseFile(CreateModule(code));
        }

        public static  void DebugTraverseFile(string filePath)
        {

            var cci = new CciModuleSource();
            var utils = new OperatorUtils(cci);

            var container = new MutantsContainer(cci, utils);
            var visualizer = new CodeVisualizer(cci);
            var cache = new MutantsCache(new DisabledWhiteCache(), container);
            cache.WhiteCache.Initialize(new[] { filePath });
            IModule module = cci.AppendFromFile(filePath);


            List<AssemblyNode> assemblyNodes = new AssemblyNode("", module)
            {
            }.InList();


            //cache.setDisabled();
            var diff = new CodeDifferenceCreator(cache, visualizer);

            var visitor = new DebugOperatorCodeVisitor();
            var traverser = new DebugCodeTraverser(visitor);

            traverser.Traverse(cci.Modules);
            
            Console.WriteLine("ORIGINAL ObjectStructure:");
            string listing0 = visitor.ToString();
            Console.WriteLine(listing0);

            Console.WriteLine("ORIGINAL C#:");
            string listing = diff.GetListing(CodeLanguage.CSharp, cci);
            Console.WriteLine(listing);

            Console.WriteLine("ORIGINAL IL:");
            string listing2 = diff.GetListing(CodeLanguage.IL, cci);
            Console.WriteLine(listing2);




        }

      
        public static string CreateModule(string code)
        {
            _log.Info("Parsing test code...");
            SyntaxTree tree = SyntaxTree.ParseText(code);
            _log.Info("Creating compilation...");
            Compilation comp = Compilation.Create("MyCompilation",
                                                  new CompilationOptions(OutputKind.DynamicallyLinkedLibrary))
                .AddSyntaxTrees(tree)
                .AddReferences(new MetadataFileReference(typeof (object).Assembly.Location));

            string outputFileName = Path.Combine(Path.GetTempPath(), "MyCompilation.lib");
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
        public static List<IModule> CreateModules(string code)
        {
            var path = CreateModule(code);
            return CreateModules(path, new CciModuleSource());
        }

        public static List<MutMod> CreateMutants(IMutationOperator operatorr, MutantsContainer container,
            CciModuleSource ccii, MutantsCache cache, int numberOfMutants)
        {
            _log.Info("Copying modules...");
            var mutantsCreationOptions = new MutantsCreationOptions()
            {
                MaxNumerOfMutantPerOperator = numberOfMutants,
            };
            container.Initialize(operatorr.InList(), mutantsCreationOptions, MutationFilter.AllowAll());

            var assemblyNodes = container.InitMutantsForOperators(ccii, ProgressCounter.Inactive());

            return assemblyNodes.Single().Children.SelectManyRecursive(g => g.Children, leafsOnly:true).Cast<Mutant>()
                .Select(m => new MutMod(m, cache.GetMutatedModules(m))).ToList();
        }
        public static IEnumerable<MutantGroup> CreateMutantGroupsLight(IMutationOperator operatorr, MutantsContainer container,
            CciModuleSource ccii, MutantsCache cache, int numberOfMutants)
        {

            _log.Info("Copying modules...");
            var mutantsCreationOptions = new MutantsCreationOptions()
            {
                MaxNumerOfMutantPerOperator = numberOfMutants,
            };

            container.Initialize(operatorr.InList(), mutantsCreationOptions, MutationFilter.AllowAll());

            var assemblyNodes = container.InitMutantsForOperators(ccii, ProgressCounter.Inactive());
            return assemblyNodes.Single().Children.SelectManyRecursive(g => g.Children).OfType<MutantGroup>();

        }
        public static List<Mutant> CreateMutantsLight(IMutationOperator operatorr,
            CciModuleSource ccii, int numberOfMutants, out CodeDifferenceCreator diff)
        {
            var container = new MutantsContainer(ccii, new OperatorUtils(ccii));
            var visualizer = new CodeVisualizer(ccii);
            var cache = new MutantsCache(new DisabledWhiteCache() ,container);
            cache.setDisabled(disableCache: false);
            cache.WhiteCache.Initialize(ccii.ModulesInfo.Select(m => m.FilePath).ToList());
            diff = new CodeDifferenceCreator(cache, visualizer);
            container.DebugConfig = true;

            _log.Info("Copying modules...");
            var mutantsCreationOptions = new MutantsCreationOptions()
            {
                MaxNumerOfMutantPerOperator = numberOfMutants,
            };

            container.Initialize(operatorr.InList(), mutantsCreationOptions, MutationFilter.AllowAll());

            var assemblyNodes = container.InitMutantsForOperators(ccii, ProgressCounter.Inactive());
            return assemblyNodes.Single().Children.SelectManyRecursive(
                g => g.Children??new NotifyingCollection<CheckedNode>()).OfType<Mutant>().ToList();

        }
        public static List<IModule> CreateModules(string filePath, CciModuleSource cci)
        {
            cci.AppendFromFile(filePath);
            _log.Info("Copying modules...");
            List<IModule> copiedModules = cci.Modules.Select(cci.Copy).Cast<IModule>().ToList();
            return copiedModules;
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