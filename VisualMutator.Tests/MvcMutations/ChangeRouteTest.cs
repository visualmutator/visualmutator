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

            var assembly = Utils.ReadTestAssembly();

            var executedOperator = Utils.ExecuteMutation(new ChangeRoute(), Utils.ReadTestAssembly());

                
        }
    }
}

