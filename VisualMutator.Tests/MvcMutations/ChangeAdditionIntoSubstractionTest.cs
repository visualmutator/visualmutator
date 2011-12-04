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

    using VisualMutator.Controllers;
    using VisualMutator.Extensibility;
    using VisualMutator.Model.Mutations;
    using VisualMutator.MvcMutations;
    using VisualMutator.OperatorsStandard;
    using VisualMutator.Tests.Util;

   


  //  [TestFixture]
    public class ChangeAdditionIntoSubstractionTest
    {

 //       [Test]
        public void Test1()
        {

            var assembly = Utils.ReadTestAssembly();

            var executedOperator = Utils.ExecuteMutation(new ChangeAdditionIntoSubstraction(), Utils.ReadTestAssembly());

            var testListings = Utils.CreateListings(assembly, executedOperator).ToList();
        }

    }
}
