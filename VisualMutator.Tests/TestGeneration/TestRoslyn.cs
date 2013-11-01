namespace VisualMutator.Tests.TestGeneration
{
    #region

    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using log4net.Appender;
    using log4net.Config;
    using log4net.Layout;
    using NUnit.Framework;
    using Roslyn.Compilers;
    using Roslyn.Compilers.CSharp;
    using VisualMutator.TestGeneration;

    #endregion

    [TestFixture]
    public class TestRoslyn 
    {
        #region Setup/Teardown

        [SetUp]
        public void Setup()
        {
            BasicConfigurator.Configure(
                new ConsoleAppender
                    {
                        Layout = new SimpleLayout()
                    });
        }

        #endregion
            string execTemplate =
@" using System; 
using System.Collections.Generic; 
using System.Text; 
using System.Linq; 
  
namespace VisualMutator_generated 
{ 
    public class Program : VisualMutator.Model.TestGeneration.GenerationInterface
    { 
        public override string TestRun(int s) 
        { 
            return (s+1).ToString() +  AppDomain.CurrentDomain.FriendlyName;
        } 
        public string Process(int s) 
        { 
            return (s+1).ToString() +  AppDomain.CurrentDomain.FriendlyName;
        } 
        public static void Main(string[] s) 
        { 
            
        } 

        public override object InitializeLifetimeService()
        {
            // This ensures the object lasts for as long as the client wants it
            return null;
        }
    } 
}";

        [Test]
        public void Process()
        {
            var tree = SyntaxTree.ParseText(execTemplate);
           
            CompilationUnitSyntax root = tree.GetRoot();
            var body =
                (from methodDeclaration in root.DescendantNodes().OfType<MethodDeclarationSyntax>()
                where methodDeclaration.Identifier.ValueText == "Main"
                select methodDeclaration).Single();
                        /*

                                   var block = body.Body.WithStatements(
                                       Syntax.LocalDeclarationStatement(
                                           Syntax.VariableDeclaration(
                                               Syntax.IdentifierName(Syntax.Token(SyntaxKind.IntKeyword)))
                                                   .AddVariables(Syntax.VariableDeclarator("x"))));
                                   */

          //  var block = body.Body.WithStatements(Syntax.ParseStatement("Dsa.Utility.Guard.ArgumentNull(null, null);").WithLeadingTrivia(Syntax.Tab));
        //    root = root.ReplaceNode(body.Body, block);

            var newTree = SyntaxTree.Create(root);
      
            Console.WriteLine(newTree.GetText());
            

            var comp = Compilation.Create("MyCompilation",
                new CompilationOptions(OutputKind.DynamicallyLinkedLibrary))
                .AddSyntaxTrees(newTree)
                .AddReferences(new MetadataFileReference(typeof(object).Assembly.Location))
                .AddReferences(new MetadataFileReference(@"D:\PLIKI\Dropbox\++Inzynierka\VisualMutator\Projekty do testów\dsa-96133\Dsa\Dsa\bin\Debug\Dsa.dll"))
                .AddReferences(new MetadataFileReference(@"C:\Users\SysOp\Documents\Visual Studio 2012\Projects\VisualMutator\VisualMutator\bin\x86\Debug\VisualMutator.dll"))
                .AddReferences(MetadataReference.CreateAssemblyReference("System.Linq"));

            var outputFileName = Path.Combine(Path.GetTempPath(), "MyCompilation.dll");
         //   var ilStream = new FileStream(outputFileName, FileMode.OpenOrCreate);



            var memStream = new MemoryStream();

            var result = comp.Emit(memStream);
        //    memStream.Close();
            if (!result.Success)
            {
                var aggregate = result.Diagnostics.Select(a => a.Info.GetMessage() + " at line"+ a.Location.GetLineSpan(false)).Aggregate((a, b) => a + "\n" + b);
                throw new InvalidProgramException(aggregate);
            }

            AppDomain.CurrentDomain.AssemblyResolve += MyResolver;

            AppDomain newDomain = AppDomain.CreateDomain("New Domain");

            AppDomainCreator foo = (AppDomainCreator)newDomain.CreateInstanceFromAndUnwrap(
                             @"C:\Users\SysOp\Documents\Visual Studio 2012\Projects\VisualMutator\VisualMutator\obj\x86\Debug\VisualMutator.dll",
                             typeof(AppDomainCreator).FullName);


            try
            {
                foo.Execute(memStream.ToArray());
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            
           
         //   Console.WriteLine(foo.TestRun(4));
            AppDomain.Unload(newDomain);
          //  Console.ReadLine();
            /*
            AppDomainSetup domainSetup = new AppDomainSetup();
            AppDomain domain = AppDomain.CreateDomain("PluginDomain", null, domainSetup);
            var obj = domain.CreateInstanceAndUnwrap().CreateInstanceFromAndUnwrap(@"C:\Users\SysOp\Documents\Visual Studio 2012\Projects\ConsoleApplication1\ConsoleApplication1\bin\Debug\ConsoleApplication1.exe", "ConsoleApplication1.Klasa1");

            var m = obj.GetType().GetMethod("Method1");
            
            */
        }

        private Assembly MyResolver(object sender, ResolveEventArgs args)
        {
            //args.Name == 
            AppDomain domain = (AppDomain)sender;
            return Assembly.LoadFile(@"C:\Users\SysOp\Documents\Visual Studio 2012\Projects\VisualMutator\VisualMutator\obj\x86\Release\VisualMutator.dll");
        }
    }
}