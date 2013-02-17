using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestGround
{
    using System.Diagnostics;
    using System.IO;
  
    using System.Reflection.Emit;
    using Microsoft.Cci;
    using Microsoft.Cci.ILToCodeModel;
    using Microsoft.Cci.MutableCodeModel;
    using Roslyn.Compilers;
    using Roslyn.Compilers.CSharp;

    class Program
    {
        static void Main(string[] args)
        {

   var tree = SyntaxTree.ParseText(@"using System;
namespace HelloWorld
{
    public class Program
    {
        public static void Tain(int x)
        {
           
            Console.ReadKey();
        }
    }
}");

        var mscorlib = new MetadataFileReference(
            typeof(object).Assembly.Location);

        var comp = Compilation.Create("MyCompilation", new CompilationOptions(OutputKind.DynamicallyLinkedLibrary))
            .AddSyntaxTrees(tree).AddReferences(mscorlib);
        

        var outputFileName = Path.Combine(Path.GetTempPath(), "MyCompilation.lib");
        var ilStream = new FileStream(outputFileName, FileMode.OpenOrCreate);

        var result = comp.Emit(ilStream);
        ilStream.Close();

        using (var host = new PeReader.DefaultHost())
        {
            //Read the Metadata Model from the PE file
            var module = host.LoadUnitFrom(outputFileName) as IModule;
            if (module == null || module == Dummy.Module || module == Dummy.Assembly)
            {
                Console.WriteLine(outputFileName + " is not a PE file containing a CLR module or assembly.");
                return;
            }

        //Construct a Code Model from the Metadata model via decompilation
            Module decompiledModule = Decompiler.GetCodeModelFromMetadataModel(host, module, null);
        //    decompiledModule.UnitNamespaceRoot.GetMembersNamed(host.NameTable.GetNameFor("Tain"), true);
            var type = decompiledModule.AllTypes.Single(t => t.Name.Value == "Program");
            //type.Methods.Single(m=>m.)
            
        }
           
        if (result.Success)
        {
            // Run the compiled program.
            Process.Start(outputFileName);
        }
        else
        {
            foreach (var diag in result.Diagnostics)
            {
                Console.WriteLine(diag.ToString());
            }
        }
    

            int i = 0;
            /*
     var   sourceText =     @"using System; 
using System.Collections; 
using System.Linq; 
using System.Text; 
  
namespace HelloWorld 
{ 
    class Program 
    { 
        static void Main(string[] args) 
        { 
            Console.WriteLine(""Hello, World!""); 
        } 
    } 
}"; 

            SyntaxTree tree = SyntaxTree.ParseText(sourceText);

            MetadataReference mscorlib = MetadataReference.CreateAssemblyReference(
                                             "mscorlib");


            Compilation compilation = Compilation.Create("HelloWorld", new CompilationOptions(OutputKind.ConsoleApplication))
                            .AddReferences(mscorlib)
                            .AddSyntaxTrees(tree);

         
            //Directory.CreateDirectory(@"C:\Ttemp")
            compilation.Emit(@"C:\VisualMutatorTemp\Test.exe");*/

        }
    }
}
