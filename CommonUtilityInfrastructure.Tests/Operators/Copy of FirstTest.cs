using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VisualMutator.OperatorTests
{
    using System.IO;
    using CommonUtilityInfrastructure;
    using Extensibility;
    using Microsoft.Cci;
    using Microsoft.Cci.MutableCodeModel;
    using Model;
    using Model.CodeDifference;
    using Model.Exceptions;
    using Model.Mutations;
    using Model.Mutations.Structure;
    using Model.Mutations.Types;
    using Mono.Cecil;
    using NUnit.Framework;
    using OperatorsStandard;
    using Roslyn.Compilers;
    using Roslyn.Compilers.CSharp;
    using Tests.Util;
    using Decompiler = Microsoft.Cci.ILToCodeModel.Decompiler;


    [TestFixture]
    public class FirstTest2
    {
        string CreateModule(string code)
        {
        //    CoreAssembly = Host.LoadAssembly(Host.CoreAssemblySymbolicIdentity);

            var tree = SyntaxTree.ParseText(code);

            var comp = Compilation.Create("MyCompilation",
                new CompilationOptions(OutputKind.DynamicallyLinkedLibrary))
                .AddSyntaxTrees(tree)
                .AddReferences(new MetadataFileReference(typeof(object).Assembly.Location));

            var outputFileName = Path.Combine(Path.GetTempPath(), "MyCompilation.lib");
            var ilStream = new FileStream(outputFileName, FileMode.OpenOrCreate);

            var result = comp.Emit(ilStream);
            ilStream.Close();
            return outputFileName;
        }
        List<Mutant> CreateMutants(string code, IMutationOperator operatorr, MutantsContainer container, CommonCompilerAssemblies cci)
        {


            cci.AppendFromFile(CreateModule(code));

            var operatorWithTargets = container.FindTargets(operatorr, cci.Modules, new List<TypeIdentifier>());
            var mutants = new List<Mutant>();
            foreach (MutationTarget mutationTarget in operatorWithTargets.MutationTargets.Values.SelectMany(v => v))
            {

                var mutant = new Mutant(0, operatorWithTargets.ExecutedOperator, mutationTarget, operatorWithTargets.CommonTargets);
                operatorWithTargets.ExecutedOperator.Children.Add(mutant);
                container.ExecuteMutation(mutant, cci.Modules, new List<TypeIdentifier>(), ProgressCounter.Inactive());
                mutants.Add(mutant);
            }
            return mutants;
        }

 

        [Test]
        public void Test1()
        {

            const string code = @"using System;
                namespace Ns
                {
                    public class Test
                    {
                        public int FailOnZero(Test test)
                        {
                            return test.Equals(test) ? 0 : 1;
                      
                        }
                        public override bool Equals(object obj)
                        {
                            return false;
                        }
                    }
                }";

                        
            var cci = new CommonCompilerAssemblies();
            var manager = new AssembliesManager(cci, Create.TestServices(), new AssemblyReaderWriter());
            var container = new MutantsContainer(cci, manager);
            var original = manager.Load(cci.Modules);

            var mutants = CreateMutants(code, new EqualityOperatorChange(), container, cci);

            var diff = new CodeDifferenceCreator(manager);
            foreach (var mutant in mutants)
            {
                var codeWithDifference = diff.CreateDifferenceListing(CodeLanguage.CSharp, mutant, original);
                Console.WriteLine(codeWithDifference);
            }

            Assert.IsTrue(mutants.Count == 1);
        }

        public AssembliesProvider Load(IList<IModule> assemblies, PeReader.DefaultHost host, 
            ISourceLocationProvider sourceLocationProvider, ILocalScopeProvider localScopeProvider, PdbReader pdbReader)
        {
            var pro = new AssembliesProvider()
            {
                Assemblies = new List<AssemblyDefinition>()
            };
            Directory.CreateDirectory(@"C:\VisualMutatorTemp");
            foreach (var module in assemblies)
            {

                string file = @"C:\VisualMutatorTemp\" + module.Name.Value;



                using (FileStream peStream = File.Create(file))
                {
                    if (pdbReader == null)
                    {
                        PeWriter.WritePeToStream(module, host, peStream);
                    }
                    else
                    {
                        using (var pdbWriter = new PdbWriter(Path.ChangeExtension(file, "pdb"), pdbReader))
                        {
                            PeWriter.WritePeToStream(module, host, peStream, sourceLocationProvider,
                                                     localScopeProvider, pdbWriter);
                        }
                    }
                }


                try
                {
                    var assemblyDefinition = AssemblyDefinition.ReadAssembly(file);
                    
                    pro.Assemblies.Add(assemblyDefinition);
                }
                catch (FileNotFoundException e)
                {
                    throw new AssemblyReadException("", e);
                }
                catch (DirectoryNotFoundException e)
                {
                    throw new AssemblyReadException("", e);
                }
                catch (IOException e)
                {
                    throw new AssemblyReadException("", e);
                }
                finally
                {
                    File.Delete(file);
                }
            }


            return pro;
        }
     /*   public static IEnumerable<TestListing> CreateListings(AssemblyDefinition original, ExecutedOperator oper)
        {

            var assembliesManager = new AssembliesManager();
            var codeDifferenceCreator = new CodeDifferenceCreator(assembliesManager);


            foreach (var mutant in oper.Mutants)
            {
                var code = codeDifferenceCreator.CreateDifferenceListing(CodeLanguage.CSharp, mutant, new[] { original });
                TestListing listings = new TestListing()
                {
                    Text = code.Code
                };
                yield return listings;
            }


        }*/

    }
}
