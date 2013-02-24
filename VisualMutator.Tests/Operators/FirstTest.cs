namespace VisualMutator.Tests.Operators
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.IO;
    using CommonUtilityInfrastructure;
    using VisualMutator.Extensibility;
    using Microsoft.Cci;
    using Microsoft.Cci.MutableCodeModel;
    using VisualMutator.Model;
    using VisualMutator.Model.Exceptions;
    using Mono.Cecil;
    using NUnit.Framework;
    using VisualMutator.OperatorsStandard;
    using Decompiler = Microsoft.Cci.ILToCodeModel.Decompiler;

    [TestFixture]
    public class FirstTest
    {

        public class MyVisitor : OperatorCodeVisitor
        {
            public override void Visit(IAddition addition)
            {
                if(addition.RightOperand is CompileTimeConstant)
                {
                    MarkMutationTarget(addition);
                }
            }
        }
        public class MyRewriter : OperatorCodeRewriter
        {
            public override IExpression Rewrite(IAddition addition)
            {
                return new Subtraction
                {
                    LeftOperand = addition.LeftOperand,
                    RightOperand = addition.RightOperand,
                    Type = addition.Type,
                    Locations = addition.Locations.ToList()
                };
            }
        }

        [Test]
        public void Test1()
        {
         //   string file = @"C:\Users\SysOp\Documents\Visual Studio 2012\Projects\ConsoleApplication1\ConsoleApplication1\bin\Debug\ConsoleApplication1.exe";
            string file = @"C:\Users\SysOp\Documents\Visual Studio 2012\Projects\VisualMutator\TestGround\bin\Debug\TestGround.exe";

            IMutationOperator mutationOperator = new AbsoluteValueInsertion();
            
            using (var host = new PeReader.DefaultHost())
            {
                //Read the Metadata Model from the PE file
                var module = host.LoadUnitFrom(file) as IModule;
                if (module == null || module == Dummy.Module || module == Dummy.Assembly)
                {
                    Console.WriteLine(file + " is not a PE file containing a CLR module or assembly.");
                    return;
                }

                //Get a PDB reader if there is a PDB file.
                PdbReader /*?*/ pdbReader = null;
                string pdbFile = Path.ChangeExtension(module.Location, "pdb");
                if (File.Exists(pdbFile))
                {
                    Stream pdbStream = File.OpenRead(pdbFile);
                    pdbReader = new PdbReader(pdbStream, host);
                }
                using (pdbReader)
                {
                    //Construct a Code Model from the Metadata model via decompilation
                    Module decompiledModule = Decompiler.GetCodeModelFromMetadataModel(host, module, pdbReader);
                    ISourceLocationProvider sourceLocationProvider = pdbReader;
                    //The decompiler preserves the Locations from the IOperation values, so the PdbReader still works.
                    //Recompiling the CodeModel to IL might change the IL offsets, so a new provider is needed.
                    ILocalScopeProvider localScopeProvider = new Decompiler.LocalScopeProvider(pdbReader);

                    //Get a mutable copy of the Code Model. The ISourceLocationProvider is needed to provide copied source method bodies with the
                    //ability to find out where to mark sequence points when compiling themselves back into IL.
                    //(ISourceMethodBody does not know about the Source Model, so this information must be provided explicitly.)
                    var copier = new CodeDeepCopier(host, sourceLocationProvider);
                    Module originalModule = copier.Copy(decompiledModule);
                
                    INamedTypeDefinition allowedClass = UnitHelper.FindType(host.NameTable, originalModule, "ConsoleApplication1.Klasa1");
                    var allowed = new List<TypeIdentifier> { new TypeIdentifier((INamespaceTypeDefinition)allowedClass) };
                    var myvisitor = mutationOperator.FindTargets();
                    
                    var visitor = new VisualCodeVisitor(myvisitor);
                    myvisitor.MarkMutationTarget = visitor.MarkMutationTarget;
                    myvisitor.MarkCommon = visitor.MarkCommon;
                    myvisitor.Host = host;
                    myvisitor.Initialize();
                    // myvisitor.MarkMutationTarget = visitor.MarkMutationTarget;
                    //Traverse the mutable copy. In a real application the traversal will collect information to be used during rewriting.
                    var traverser = new VisualCodeTraverser(allowed, visitor);
                    traverser.Traverse(originalModule);



                    var operatorCodeRewriter = mutationOperator.Mutate();


                    List<IModule> mutantModules = new List<IModule>();
                    foreach (var mutationTarget in visitor.MutationTargets)
                    {
                        var copierNew = new CodeDeepCopier(host, sourceLocationProvider);
                        Module mutableModule = copierNew.Copy(decompiledModule);

                        var visitor2 = new VisualCodeVisitorBack(mutationTarget.InList(), visitor.CommonTargets);
                        var traverser2 = new VisualCodeTraverser(allowed, visitor2);
                        traverser2.Traverse(mutableModule);

                        operatorCodeRewriter.MutationTarget = mutationTarget;
                        operatorCodeRewriter.NameTable = host.NameTable;
                        operatorCodeRewriter.Host = host;
                        operatorCodeRewriter.Module = mutableModule;
                        operatorCodeRewriter.OperatorCodeVisitor = myvisitor;
                        var rewriter = new VisualCodeRewriter(host, visitor2.MutationTargetsElements, visitor2.CommonTargetsElements, allowed, operatorCodeRewriter);
                        IModule rewrittenModule = rewriter.Rewrite(mutableModule);
                        mutantModules.Add(rewrittenModule);
                    }


                    int i = 0;


                    // var visitor2 = new VisualCodeVisitorBack(visitor.MutationTargets);
                    // // var traverser2 = new VisualCodeTraverser(allowed, visitor);
                    //   traverser2.Traverse(mutableModule);

                    /*var stringList = myvisitor.AllElements.Select(elem => elem.MethodType+" ==== "
                        +elem.Obj.GetType().ToString() + " --- " + elem.Obj.ToString());
                    //var builder = new StringBuilder();
                    foreach (var str in stringList)
                    {
                        Console.WriteLine(str);
                    }
                               */
                    //Assert.AreEqual(visitor2.MutationTargetsElements.Count, 1);
                    //Rewrite the mutable Code Model. In a real application CodeRewriter would be a subclass that actually does something.
                    //(This is why decompiled source method bodies must recompile themselves, rather than just use the IL from which they were decompiled.)
                    /*var rewriter = new CodeRewriter(host);
                    IModule rewrittenModule = rewriter.Rewrite(mutableModule);

                    //Write out the Code Model by traversing it as the Metadata Model that it also is.
                    using (FileStream peStream = File.Create(rewrittenModule.Location + ".pe"))
                    {
                        if (pdbReader == null)
                        {
                            PeWriter.WritePeToStream(rewrittenModule, host, peStream);
                        }
                        else
                        {
                            using (var pdbWriter = new PdbWriter(rewrittenModule.Location + ".pdb", pdbReader))
                            {
                                PeWriter.WritePeToStream(rewrittenModule, host, peStream, sourceLocationProvider,
                                                         localScopeProvider, pdbWriter);
                            }
                        }
                    }*/
                }
            }
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
