using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VisualMutator.OperatorTests
{
    using System.IO;
    using Extensibility;
    using Microsoft.Cci;
    using Microsoft.Cci.ILToCodeModel;
    using Microsoft.Cci.MutableCodeModel;

    using NUnit.Framework;


    [TestFixture]
    public class FirstTest
    {

        public class MyVisitor : OperatorCodeVisitor
        {
            public override void Visit(IAddition addition)
            {
                MarkMutationTarget(addition);
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
                };
                
            }
        }

        [Test]
        public void Test1()
        {
            string file =  @"C:\Users\SysOp\Documents\Visual Studio 2012\Projects\VisualMutator\TestGround\bin\Debug\TestGround.exe";
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
                    Module mutableModule = copier.Copy(decompiledModule);

                    var allowed = new List<string> {"TestGround"};
                    var visitor = new VisualCodeVisitor(new MyVisitor());
                    //Traverse the mutable copy. In a real application the traversal will collect information to be used during rewriting.
                    var traverser = new VisualCodeTraverser(allowed)
                    {
                        PreorderVisitor = visitor,
                    };
                    traverser.Traverse(mutableModule);

                    var visitor2 = new VisualCodeVisitorBack(visitor.MutationTargets);
                    var traverser2 = new VisualCodeTraverser(allowed)
                    {
                        PreorderVisitor = visitor2,
                    };
                    traverser2.Traverse(mutableModule);

                    Assert.AreEqual(visitor2.MutationTargetsElements.Count, 1);


                    //Rewrite the mutable Code Model. In a real application CodeRewriter would be a subclass that actually does something.
                    //(This is why decompiled source method bodies must recompile themselves, rather than just use the IL from which they were decompiled.)
                    var rewriter = new VisualCodeRewriter(host, visitor2.MutationTargetsElements, allowed, new MyRewriter());
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
                    }
                }
            }
        }

    }
}
