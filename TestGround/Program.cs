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
    using log4net.Appender;
    using log4net.Config;
    using log4net.Layout;
    using Microsoft.Cci;
    using Microsoft.Cci.ILToCodeModel;
    using Microsoft.Cci.MutableCodeModel;
    using Ninject;
    using Ninject.Activation.Strategies;
    using Ninject.Modules;
    using NUnit.Core;
    using Roslyn.Compilers;
    using Roslyn.Compilers.CSharp;
    using UsefulTools.DependencyInjection;
    using VisualMutator.Infrastructure;
    using VisualMutator.Infrastructure.NinjectModules;
    using VisualMutator.Model.Tests.Services;

    class Program
    {
        private StandardKernel _kernel;

        static void Main(string[] args)
        {
            BasicConfigurator.Configure(
                new ConsoleAppender
            {
                Layout = new SimpleLayout()
            });
            //  var nUnitWrapper = new NUnitWrapper(null);
            // ITest loadTests = nUnitWrapper.LoadTests(new[] { @"C:\Users\Arego\AppData\Local\Microsoft\VisualStudio\12.0Exp\Extensions\PiotrTrzpil\VisualMutator\2.0.8\VisualMutator.dll" });
            new Program().TestMemory();
            Console.ReadLine();
            //  int i = 0;
        }

        public void TestMemory()
        {
            var modules = new INinjectModule[]
            {
                new TestModule(),
            };

            _kernel = new StandardKernel();
            _kernel.Components.Add<IActivationStrategy, MyMonitorActivationStrategy>();

            _kernel.Load(modules);

            var factory = _kernel.Get<IRootFactory<SomeMainModule>>();
            for (int i = 0;  i < 10000;  i++)
            {
                IObjectRoot<SomeMainModule> someMainModule = factory.Create();
                someMainModule.Get.CreateSub();
                var s = someMainModule as ObjectRoot<SomeMainModule>;
             //   s.Kernel.Dispose();
            }
        }

        public class TestModule : NinjectModule
        {
            public override void Load()
            {
                Kernel.BindObjectRoot<SomeMainModule>().ToSelf(k =>
                {
                    k.Bind<SomeObject>().ToSelf().AndFromFactory();
                });
            }
        }

        public class SomeMainModule
        {
            private readonly IFactory<SomeObject> _objectFactory;
            private SomeObject someObject;

            public SomeMainModule(IFactory<SomeObject> objectFactory)
            {
                _objectFactory = objectFactory;
               
            }
            public void CreateSub()
            {
                someObject = _objectFactory.Create();
            }
        }

        public class SomeObject
        {
            private readonly SomeMainModule _module;
            private object[] _bigArray;

            public SomeObject(SomeMainModule module)
            {
                _module = module;
                _bigArray = new object[5000000];
            }
        }












        static void Main2(string[] args)
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
