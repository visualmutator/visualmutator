namespace VisualMutator.OperatorTests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;

    using Mono.Cecil;

    using VisualMutator.Controllers;
    using VisualMutator.Extensibility;
    using VisualMutator.Model.CodeDifference;
    using VisualMutator.Model.Mutations;
    using VisualMutator.Model.Mutations.Structure;
    using VisualMutator.MvcMutations;

    public class TestAssemblyFile : IDisposable
    {

        public string FilePath
        {
            get;
            set;
        }

        public TestAssemblyFile()
        {
            string p = Path.Combine(Utils.NerdDinner3Directory, Utils.NerdDinner3AssemblyName);


            FilePath = Path.Combine(Utils.NerdDinner3Directory, "session", Utils.NerdDinner3AssemblyName);
            Directory.CreateDirectory(Path.Combine(Utils.NerdDinner3Directory, "session"));
            File.Copy(p, FilePath, true);
        }

        public void Dispose()
        {
            // File.Delete(FilePath);
        }
    }
    public class Utils
    {
        public const string TestAssembliesFolder = @"..\..\TestAssemblies";

     
        public static readonly string NerdDinner3Directory
            = Path.Combine(TestAssembliesFolder, @"NerdDinner3-Debug");
        public static readonly string NerdDinner3AssemblyName
         =  @"NerdDinner.dll";



        public static AssemblyDefinition ReadTestAssembly()
        {
            var assemblyFile = new TestAssemblyFile();
            var assembly = AssemblyDefinition.ReadAssembly(assemblyFile.FilePath);
            return assembly;
        }
        public static ExecutedOperator ExecuteMutation(IMutationOperator mutator, AssemblyDefinition assembly)
        {
            var mutationSessionChoices = new MutationSessionChoices
            {
                SelectedOperators = new[] { mutator },
                Assemblies = new[] { assembly },
                SelectedTypes = assembly.MainModule.Types
            };

            var assembliesManager = new AssembliesManager();

            MutantsContainer mutantsContainer = new MutantsContainer(assembliesManager);

            var mutationTestingSession = mutantsContainer.PrepareSession(mutationSessionChoices);
            mutantsContainer.GenerateMutantsForOperators(mutationTestingSession);
               var executedOperator = mutationTestingSession.MutantsGroupedByOperators.Single();

            return executedOperator;
        }

        public static AssemblyDefinition LoadMutantAssembly(Mutant mutant)
        {
            var assembliesManager = new AssembliesManager();
            return assembliesManager.Load(mutant.StoredAssemblies).Single();
        }

        public static IEnumerable<TestListing> CreateListings(AssemblyDefinition original, ExecutedOperator oper)
        {

            var assembliesManager = new AssembliesManager();
            var codeDifferenceCreator = new CodeDifferenceCreator(assembliesManager);


            foreach (var mutant in oper.Mutants)
            {
                var code =codeDifferenceCreator.CreateDifferenceListing(CodeLanguage.CSharp, mutant, new[] { original });
                TestListing listings = new TestListing()
                {
                    Text = code.Code
                };
                yield return listings;
            }

   
        }




    }

    public class TestListing
    {
        public string Text
        {
            get; set; }
    }
}