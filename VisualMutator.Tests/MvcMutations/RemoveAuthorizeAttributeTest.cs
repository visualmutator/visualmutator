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

   


    [TestFixture]
    public class RemoveAuthorizeAttributeTest
    {

        [Test]
        public void Test1()
        {

            var assembly = Utils.ReadTestAssembly();

            var executedOperator = Utils.ExecuteMutation(new ChangeParameterName(), Utils.ReadTestAssembly());

            var mut = executedOperator.Mutants.ToList();
            var testListings = Utils.CreateListings(assembly, executedOperator).ToList();
        }

    }
}
