namespace VisualMutator.Tests.Operators
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Security.Policy;
    using CommonUtilityInfrastructure;
    using Extensibility;
    using Microsoft.Cci;
    using Model;
    using Model.Decompilation;
    using Model.Decompilation.CodeDifference;
    using Model.Mutations;
    using Model.Mutations.MutantsTree;
    using Model.Mutations.Operators;
    using Model.Mutations.Types;
    using Roslyn.Compilers;
    using Roslyn.Compilers.CSharp;
    using log4net;

    public class Common
    {
        protected static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static void RunMutations(string code, IMutationOperator oper, out List<Mutant> mutants,
                                        out AssembliesProvider original, out CodeDifferenceCreator diff)
        {
            RunMutationsFromFile(CreateModule(code), oper, out mutants, out original, out diff);



        }
        public static void RunMutationsFromFile(string filePath, IMutationOperator oper, out List<Mutant> mutants,
                                       out AssembliesProvider original, out CodeDifferenceCreator diff)
        {
            _log.Info("Common.RunMutations configuring for " + oper + "...");

            var cci = new CommonCompilerAssemblies();
            var utils = new OperatorUtils(cci);

            var container = new MutantsContainer(cci, utils);
            var visualizer = new CodeVisualizer(cci);
            var cache = new MutantsCache(container);

            cci.AppendFromFile(filePath);

            original = new AssembliesProvider(cci.Modules);

            cache.Initialize(original, new List<TypeIdentifier>(), true);
            diff = new CodeDifferenceCreator(cache, visualizer);

           

            container.DebugConfig = true;
            var mutmods = CreateMutants(oper, container, cci, cache);
            mutants = mutmods.Select(m => m.Mutant).ToList();




        }

        public static void DebugTraverse(string code)
        {
            DebugTraverseFile(CreateModule(code));
        }

        public static  void DebugTraverseFile(string filePath)
        {
        

            var cci = new CommonCompilerAssemblies();
            var utils = new OperatorUtils(cci);

            var container = new MutantsContainer(cci, utils);
            var visualizer = new CodeVisualizer(cci);
            var cache = new MutantsCache(container);

            cci.AppendFromFile(filePath);

            var original = new AssembliesProvider(cci.Modules);

            cache.Initialize(original, new List<TypeIdentifier>());
            var diff = new CodeDifferenceCreator(cache, visualizer);

            var visitor = new DebugOperatorCodeVisitor();
            var traverser = new DebugCodeTraverser(visitor);

            traverser.Traverse(cci.Modules);
            
            Console.WriteLine("ORIGINAL ObjectStructure:");
            string listing0 = visitor.ToString();
            Console.WriteLine(listing0);

            Console.WriteLine("ORIGINAL C#:");
            string listing = diff.GetListing(CodeLanguage.CSharp, original);
            Console.WriteLine(listing);

            Console.WriteLine("ORIGINAL IL:");
            string listing2 = diff.GetListing(CodeLanguage.IL, original);
            Console.WriteLine(listing2);




        }

        public static void RunMutationsReal(string assemblyPath, IMutationOperator oper, out List<MutMod> mutants,
                                        out AssembliesProvider original, out CodeVisualizer visualizer, 
                                        out CommonCompilerAssemblies cci)
        {
            _log.Info("Common.RunMutations configuring for " + oper + "...");

            cci = new CommonCompilerAssemblies();
            var utils = new OperatorUtils(cci);

            var container = new MutantsContainer(cci, utils);
            visualizer = new CodeVisualizer(cci);
            var cache = new MutantsCache(container);


            container.DebugConfig = true;
            original = new AssembliesProvider(cci.Modules);

            var diff = new CodeDifferenceCreator(cache, visualizer);
            cache.Initialize(original, new List<TypeIdentifier>());

            Console.WriteLine("ORIGINAL:");
            string listing = diff.GetListing(CodeLanguage.CSharp, original);
            //  string listing = visualizer.Visualize(CodeLanguage.CSharp,)
            Console.WriteLine(listing);


            mutants = CreateMutantsExt(assemblyPath, oper, container, cci, visualizer, original);
       


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
            return CreateModules(path, new CommonCompilerAssemblies());
        }

        public static string CreateModule2(string code)
        {
            var e = new Evidence();
            return null;
        }

        public static List<MutMod> CreateMutants(IMutationOperator operatorr, MutantsContainer container, 
            CommonCompilerAssemblies cci, MutantsCache cache)
        {
            
            _log.Info("Copying assemblies...");
            AssembliesProvider copiedModules = new AssembliesProvider(cci.Modules.Select(cci.Copy).Cast<IModule>().ToList());


            var choices = new MutationSessionChoices()
                {
                    MutantsCreationOptions = new MutantsCreationOptions()
                        {
                            MaxNumerOfMutantPerOperator = 100,
                        },
                        Assemblies = new List<AssemblyNode>(),
                        SelectedTypes = new LoadedTypes(new List<INamespaceTypeDefinition>())
                };

            container.PrepareSession(choices);

   

            var executedOperators = container.GenerateMutantsForOperators(operatorr.InList(), new List<TypeIdentifier>(),
                                                                          copiedModules, ProgressCounter.Inactive());

            return executedOperators.Single().MutantGroups.SelectMany(g=>g.Mutants)
                .Select(m => new MutMod(m, cache.GetMutatedModules(m))).ToList();

            /*
            MutantsContainer.OperatorWithTargets operatorWithTargets = container.CreateVisitor(operatorr,
                                                                                             copiedModules,
                                                                                             new List<TypeIdentifier>());

            var mutants = new List<MutMod>();
            foreach (MutationTarget mutationTarget in operatorWithTargets.MutationTargets.Values.SelectMany(v => v))
            {
                var exec = new ExecutedOperator("", "", operatorWithTargets.Operator);
                var mutant = new Mutant("0", exec, mutationTarget, operatorWithTargets._sharedTargets);

                var assembliesProvider = container.ExecuteMutation(mutant, cci.Modules, new List<TypeIdentifier>(), ProgressCounter.Inactive());
                mutants.Add(new MutMod ( mutant, assembliesProvider ));
            
            

            
            
            }
            return mutants;*/
        }
        public static List<IModule> CreateModules(string filePath, CommonCompilerAssemblies cci)
        {
            cci.AppendFromFile(filePath);
            _log.Info("Copying assemblies...");
            List<IModule> copiedModules = cci.Modules.Select(cci.Copy).Cast<IModule>().ToList();


            return copiedModules;
        }
        public static List<MutMod> CreateMutantsExt(string filePath, IMutationOperator operatorr, MutantsContainer container, CommonCompilerAssemblies cci, CodeVisualizer visualizer, AssembliesProvider original)
        {
            cci.AppendFromFile(filePath);
            _log.Info("Copying assemblies...");
            List<IModule> copiedModules = cci.Modules.Select(cci.Copy).Cast<IModule>().ToList();


            MutantsContainer.OperatorWithTargets operatorWithTargets = container.FindTargets(operatorr,
                                                                                             copiedModules,
                                                                                             new List<TypeIdentifier>());

            var mutants = new List<MutMod>();
            foreach (MutationTarget mutationTarget in operatorWithTargets.MutationTargets.Select(x => x.Item2).Flatten())
            {
                var exec = new ExecutedOperator("", "", operatorWithTargets.Operator);
                var group = new MutantGroup("", exec);
                var mutant = new Mutant("0", group, mutationTarget, operatorWithTargets.CommonTargets);

                var assembliesProvider = container.ExecuteMutation(mutant, cci.Modules, new List<TypeIdentifier>(), ProgressCounter.Inactive());
                mutants.Add(new MutMod ( mutant, assembliesProvider ));


                string code = visualizer.Visualize(CodeLanguage.CSharp, mutant.MutationTarget,
                                                                                     assembliesProvider);
                Console.WriteLine(code);


                cci.WriteToFile(assembliesProvider.Assemblies.First(), @"D:\PLIKI\mutest.dll");
            
            
            }
            return mutants;
        }


       
    }
     public class MutMod
     {
         public Mutant Mutant { get; set; }
         public AssembliesProvider AssembliesProvider { get; set; }

         public MutMod(Mutant mutant, AssembliesProvider assembliesProvider)
         {
             Mutant = mutant;
             AssembliesProvider = assembliesProvider;
         }
     }

}