namespace VisualMutator.Tests.Mutations
{
    #region

    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using log4net.Appender;
    using log4net.Config;
    using log4net.Layout;
    using Microsoft.Cci;
    using Model;
    using Model.Mutations.Types;
    using NUnit.Core;
    using NUnit.Framework;
    using Operators;
    using UsefulTools.CheckboxedTree;
    using UsefulTools.ExtensionMethods;
    using UsefulTools.Paths;
    using TypeHelper = Microsoft.Cci.TypeHelper;

    #endregion

    [TestFixture("Dsa.DataStructures.Deque`1", "EnqueueFront", 1)]
    [TestFixture("Dsa.Utility.Guard", "ArgumentNull", 3)]
    [TestFixture("Dsa.Utility.Compare", "IsLessThan`1", 3)]
    public class TestSelectionIntTests
    {
        private List<FilePathAbsolute> paths;
        private SolutionTypesManager typesManager;
        private MethodIdentifier context;
        private int expectedCount;

        public TestSelectionIntTests(string className, string method, int expectedCount)
        {
            context = new MethodIdentifier(className, method);
            this.expectedCount = expectedCount;
        }

        [SetUp]
        public void Setup()
        {

            BasicConfigurator.Configure(
                new ConsoleAppender
                {
                    Layout = new SimpleLayout()
                });

            paths = new[] { 
                 MutationTestsHelper.DsaPath, 
                 MutationTestsHelper.DsaTestsPath,
                 MutationTestsHelper.DsaTestsPath2}.Select(_ => new FilePathAbsolute(_)).ToList();

            var cci = new CciModuleSource();
            typesManager = new SolutionTypesManager(cci);
        }
     

        [Test]
        public void ShouldFindCoveringTests()
        {
       
            List<MethodIdentifier> coveredTests;
            var types = typesManager.GetTypesFromAssemblies(paths, context, out coveredTests);
        
          //  Assert.AreEqual(1, types.Cast<CheckedNode>().SelectManyRecursive(_ => _.Children, leafsOnly: true).Count(n => n is TypeNode));
            Assert.AreEqual(expectedCount, coveredTests.Count());
        }

      //  [Test]
        public void Test12()
        {
            var cam = new MethodIdentifier("Dsa.DataStructures.Deque`1.EnqueueFront");

            List<MethodIdentifier> coveredTests;
            var types = typesManager.GetTypesFromAssemblies(paths.ToList(),
                cam, out coveredTests);
            Assert.AreEqual(1, coveredTests.Count());

        }

       // [Test]
        public void Test1()
        {
            var a = MutationTestsHelper.DsaTestsPath;
            var b = MutationTestsHelper.DsaPath;
            var cci = new CciModuleSource();
            var typesManager = new SolutionTypesManager(cci);
            var cam = new MethodIdentifier("Dsa.Utility.Guard.ArgumentNull");

            List<MethodIdentifier> coveredTests;
            var types = typesManager.GetTypesFromAssemblies(new[] {a, b}.Select(_ => new FilePathAbsolute(_)).ToList(),
                cam, out coveredTests);
            //TODO: dodac filtrowanie tylko lisci?
           // Assert.AreEqual(1,types.Cast<CheckedNode>().SelectManyRecursive(_=>_.Children, leafsOnly:true).Count(n => n is TypeNode));
            Assert.AreEqual(3, coveredTests.Count());
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