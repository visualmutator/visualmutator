using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VisualMutator.OperatorTests
{
    using Mono.Cecil;
    using Mono.Cecil.Cil;

    using NUnit.Framework;

    using VisualMutator.MvcMutations;
    using VisualMutator.Tests.Util;

    [TestFixture]
    public class RedirectToAction
    {
        [Test]
        public void Test1()
        {
            var assembly = AssemblyDefinition.ReadAssembly(Utils.NerdDinner3AssemblyPath);

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


            var instr2 = createMethod.Body.Instructions.Where(i =>
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

            instr2.Count().ShouldEqual(2);

        }

    }
}
