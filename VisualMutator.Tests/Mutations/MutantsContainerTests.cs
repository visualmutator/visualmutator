namespace VisualMutator.Tests.Mutations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Mono.Cecil;

    using Moq;

    using NUnit.Framework;

    using PiotrTrzpil.VisualMutator_VSPackage.Infrastructure;

    using VisualMutator.Extensibility;
    using VisualMutator.Model.Mutations;
    using VisualMutator.Model.Mutations.Operators;
    using VisualMutator.Model.Mutations.Types;
    using VisualMutator.Tests.Util;

    [TestFixture]
    public class MutantsContainerTests
    {
        public class TestOperator : IMutationOperator
        {
            public MutationResultDetails Mutate(ModuleDefinition module, IEnumerable<TypeDefinition> types)
            {
                int i = 0;
                foreach (var typeDefinition in types)
                {
                    typeDefinition.Name = "MutatedTypeName" + i++;
                }
                return new MutationResultDetails
                {
                    ModifiedMethods = new List<string>(),

                };
            }

            public string Name
            {
                get
                {
                    return "TestName";
                }
            }

            public string Description
            {
                get
                {
                    return "TestDescription";
                }
            }
        }

        [Test]
        public void Test1()
        {

            var types = new List<TypeDefinition>
            {
                CecilUtils.CreateTypeDefinition("ns1", "Type1"),
                CecilUtils.CreateTypeDefinition("ns1", "Type2"),
                CecilUtils.CreateTypeDefinition("ns1", "Type3"),
            };

            var assembly = CecilUtils.CreateAssembly("ass", types);
            var assemblies = new[] { assembly };

            var operatorsManagerMock = new Mock<IOperatorsManager>();
            operatorsManagerMock.Setup(_ => _.GetActiveOperators()).Returns(new[] { new TestOperator(), });

            var typesManagerMock = new Mock<ITypesManager>();
            typesManagerMock.Setup(_=>_.GetIncludedTypes()).Returns(assembly.MainModule.Types);
            typesManagerMock.Setup(_ => _.GetLoadedAssemblies()).Returns(assemblies);


            var mutantsFileManager = new Mock<IMutantsFileManager>();

            var mutantsContainer = new MutantsContainer(
                operatorsManagerMock.Object,
                typesManagerMock.Object,
                mutantsFileManager.Object,
                Factory.DateTime(DateTime.MinValue));

            // Act
            var session = mutantsContainer.GenerateMutant("testName", str=> { });

            // Assert
            mutantsFileManager.Verify(_ => _.StoreMutant(session, assemblies));



            Assert.IsTrue(types.All(t => t.Name.StartsWith("MutatedTypeName")));
        }
    }
}

