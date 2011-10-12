namespace VisualMutator.Tests.Mutations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Mono.Cecil;

    using Moq;

    using NUnit.Framework;

    using VisualMutator.Controllers;
    using VisualMutator.Extensibility;
    using VisualMutator.Model.Mutations;
    using VisualMutator.Model.Mutations.Operators;
    using VisualMutator.Model.Mutations.Types;
    using VisualMutator.Model.Tests;
    using VisualMutator.Tests.Util;

    [TestFixture]
    public class MutantsContainerTests
    {
        public class TestOperator : IMutationOperator
        {
            /*
            public MutationResultDetails Mutate(ModuleDefinition module, IEnumerable<TypeDefinition> types)
            {
                int i = 0;
                foreach (var typeDefinition in types)
                {
                    typeDefinition.Name = "MutatedTypeName" + i++;
                }
                return new MutationResultDetails
                {
                //    ModifiedMethods = new List<string>(),

                };
            }
            */

            class ThisMutationTarget : MutationTarget
            {
                public ThisMutationTarget(MethodDefinition method)
                    : base(method)
                {
                }
            }
            public string Name
            {
                get
                {
                    return "TestOperatorName";
                }
            }

            public string Description
            {
                get
                {
                    return "TestOperatorDescription";
                }
            }

            public IEnumerable<MutationTarget> FindTargets(IEnumerable<TypeDefinition> types)
            {
                yield return new MutationTarget(types.Single(t => t.Name == "Type1").Methods.Single());
                yield return new MutationTarget(types.Single(t => t.Name == "Type3").Methods.Single());

            }

            public MutationResultsCollection CreateMutants(IEnumerable<MutationTarget> targets, AssembliesToMutateFactory assembliesFactory)
            {
                int i = 0;
                var results = new MutationResultsCollection();
                foreach (var mutationTarget in targets)
                {
                    var assemblyDefinitions = assembliesFactory.GetNewCopy();
                    mutationTarget.GetMethod(assemblyDefinitions).Name = "MutatedMethodName" + i++;
                    
                    results.MutationResults.Add(new MutationResult
                    {
                        MutatedAssemblies = assemblyDefinitions,
                        MutationTarget = mutationTarget
                    });
                }

                return results;

            }

         
        }
        
        [Test]
        public void Test1()
        {
            var t1 = CecilUtils.CreateTypeDefinition("ns1", "Type1");
            var t2 = CecilUtils.CreateTypeDefinition("ns1", "Type2");
            var t3 = CecilUtils.CreateTypeDefinition("ns1", "Type3");
            
            var types = new List<TypeDefinition>
            {
                t1,t2,t3
            };

            var assembly = CecilUtils.CreateAssembly("ass", types);
            var assemblies = new[] { assembly };

            t1.Methods.Add(CecilUtils.CreateMethodDefinition("Method1", t1));
            t3.Methods.Add(CecilUtils.CreateMethodDefinition("Method2", t3));


            var mutantsContainer = new MutantsContainer();

            var choices = new MutationSessionChoices
            {
                Assemblies = assemblies,
                SelectedTypes = types, 
                SelectedOperators = new[] { new TestOperator() }
            };
            // Act
            var executedOperator = mutantsContainer.GenerateMutantsForOperator(choices, choices.SelectedOperators.Single());

            // Assert
            executedOperator.Name.ShouldEqual("TestOperatorName");
            executedOperator.Mutants.Count().ShouldEqual(2);
            executedOperator.Mutants.First().MutatedAssemblies.Single()
                .MainModule.Types.Single(t => t.Name == "Type1").Methods.Single().Name.ShouldEqual("MutatedMethodName1");

            Assert.IsTrue(types.All(t => t.Name.StartsWith("MutatedTypeName")));
        }
    }
}

