namespace VisualMutator.Tests.Mutations
{
    #region

    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Model;
    using Model.Mutations.Types;
    using NUnit.Framework;
    using UsefulTools.CheckboxedTree;
    using UsefulTools.ExtensionMethods;
    using UsefulTools.Paths;

    #endregion

    [TestFixture]
    public class TestSelectionTests
    {
        [Test]
        public void Test31()
        {
            var r = new Regex(@"[\w\d]+<(\w+,?)+>");
            Match match = r.Match("Deque<T,Sfr>");
            var capturesCount = match.Groups[1].Captures.Cast<Capture>().Count();
            var cap = new List<Capture>();
            List<Group> groups = new List<Group>();
            foreach (Group gr in match.Groups)
            {
                groups.Add(gr);
                foreach (Capture c in gr.Captures)
                {
                    cap.Add(c);
                }
                int e = 3;
            }
            
            Assert.True(match.Success);
        }
        [Test]
        public void Test12()
        {
            var a = @"C:\PLIKI\Dropbox\++Inzynierka\VisualMutator\Projekty do testów\dsa-96133\Dsa\Dsa.Test\bin\Debug\Dsa.Test.dll";
            var b = @"C:\PLIKI\Dropbox\++Inzynierka\VisualMutator\Projekty do testów\dsa-96133\Dsa\Dsa.Test\bin\Debug\Dsa.dll";
            var cci = new ModuleSource();
            var typesManager = new SolutionTypesManager(cci);
            var cam = new ClassAndMethod
            {
                ClassName = "Dsa.DataStructures.Deque`1",
                MethodName = "EnqueueFront",
            };

            List<ClassAndMethod> coveredTests;
            var types = typesManager.GetTypesFromAssemblies(new[] { a, b }.Select(_ => new FilePathAbsolute(_)).ToList(),
                cam, out coveredTests);
            //TODO: dodac filtrowanie tylko lisci?
            Assert.AreEqual(1, types.Cast<CheckedNode>().SelectManyRecursive(_ => _.Children, leafsOnly: true).Count(n => n is TypeNode));
            Assert.AreEqual(3, coveredTests.Count());
            //TODO: ReadAssembly does not work on this artificial assembly

        }
            [Test]
        public void Test1()
        {
           var a = @"C:\PLIKI\Dropbox\++Inzynierka\VisualMutator\Projekty do testów\dsa-96133\Dsa\Dsa.Test\bin\Debug\Dsa.Test.dll";
           var b = @"C:\PLIKI\Dropbox\++Inzynierka\VisualMutator\Projekty do testów\dsa-96133\Dsa\Dsa.Test\bin\Debug\Dsa.dll";
            var cci = new ModuleSource();
            var typesManager = new SolutionTypesManager(cci);
            var cam = new ClassAndMethod
                      {
                          ClassName = "Dsa.Utility.Guard",
                          MethodName = "ArgumentNull",
                      };

            List<ClassAndMethod> coveredTests;
            var types = typesManager.GetTypesFromAssemblies(new[] {a, b}.Select(_ => new FilePathAbsolute(_)).ToList(),
                cam, out coveredTests);
            //TODO: dodac filtrowanie tylko lisci?
            Assert.AreEqual(1,types.Cast<CheckedNode>().SelectManyRecursive(_=>_.Children, leafsOnly:true).Count(n => n is TypeNode));
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