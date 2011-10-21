namespace VisualMutator.OperatorTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Mono.Cecil;
    using Mono.Cecil.Cil;

    using NUnit.Framework;

    using VisualMutator.MvcMutations;
    using VisualMutator.Tests.Util;

    [TestFixture]
    public class ChangeRouteTest
    {

        [Test]
        public void Test1()
        {

            var assemblyFile = new TestAssemblyFile();
            var assembly = AssemblyDefinition.ReadAssembly(assemblyFile.FilePath);

     
            var mutator = new ChangeRoute();

          //  mutator.Mutate(assembly.MainModule, assembly.MainModule.Types);

            assembly.Write(assemblyFile.FilePath);

            var assembly2 = AssemblyDefinition.ReadAssembly(assemblyFile.FilePath);


                

        }
    }
}

