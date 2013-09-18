namespace VisualMutator.Tests.Mutations
{
    using NUnit.Framework;

    [TestFixture]
    public class MutantsContainerTests
    {
        [Test]
        public void Test1()
        {
            //TODO: ReadAssembly does not work on this artificial assembly

            /*
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

            var assembliesManager = new AssembliesManager();

            var mutantsContainer = new MutantsContainer(assembliesManager);

            var choices = new MutationSessionChoices
            {
                Assemblies = assemblies,
                SelectedTypes = types, 
                SelectedOperators = new[] { new TestOperator() }
            };
            // Act
            var mutationTestingSession = mutantsContainer.Initialize(choices);
            mutantsContainer.InitMutantsForOperators(mutationTestingSession);
            var executedOperator = mutationTestingSession.MutantsGroupedByOperators.Single();

            // Assert
            executedOperator.Name.ShouldEqual("TestOperatorName");
            executedOperator.Mutants.Count().ShouldEqual(2);

            assembliesManager.Load(executedOperator.Mutants.First().StoredAssemblies).Single()
                .MainModule.Types.Single(t => t.Name == "Type1").Methods.Single().Name.ShouldEqual("MutatedMethodName0");
*/
        }
    }
}