using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VisualMutator.OperatorTests
{
    using System.IO;

    using Mono.Cecil;
    using Mono.Cecil.Cil;

    using NUnit.Framework;

    using VisualMutator.MvcMutations;
    using VisualMutator.Tests.Util;

    [TestFixture]
    public class RedirectToAction
    {

        public class TestAssemblyFile : IDisposable
        {

            public string FilePath { get; set; }

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


        [Test]
        public void Test1()
        {
            using (var assemblyFile = new TestAssemblyFile())
            {
                var assembly = AssemblyDefinition.ReadAssembly(assemblyFile.FilePath);

                var dinnersController = assembly.MainModule.Types.Single(t => t.Name == "DinnersController");

                var createMethod = dinnersController.Methods.Single(m => m.Name == "Create" && m.Parameters.Count == 1);


                var instr = createMethod.Body.Instructions.Single(i =>
                {
                    if (i.OpCode == OpCodes.Call)
                    {
                        var method = ((MethodReference)i.Operand);
                        if (method.DeclaringType.FullName == "System.Web.Mvc.Controller"
                            && method.Name == "View")
                        //  && method.Parameters.Count == 0)
                        {
                            return true;
                        }
                    }
                    return false;
                });


                var mutator = new ReplaceViewWithRedirectToAction();

                mutator.Mutate(assembly.MainModule, assembly.MainModule.Types);

                assembly.Write(assemblyFile.FilePath);

                var assembly2 = AssemblyDefinition.ReadAssembly(assemblyFile.FilePath);

                var dinnersController2 = assembly2.MainModule.Types.Single(t => t.Name == "DinnersController");

                var createMethod2 = dinnersController2.Methods.Single(m => m.Name == "Create" && m.Parameters.Count == 1);

                var instr3 = createMethod2.Body.Instructions.Where(i =>
                {
                    if (i.OpCode == OpCodes.Call)
                    {
                        var method = ((MethodReference)i.Operand);
                        if (method.DeclaringType.FullName == "System.Web.Mvc.Controller"
                            && method.Name == "RedirectToAction")
                        //  && method.Parameters.Count == 0)
                        {
                            return true;
                        }
                    }
                    return false;
                });

                instr3.Count().ShouldEqual(2);


                

            }
            
        }

    }
}
