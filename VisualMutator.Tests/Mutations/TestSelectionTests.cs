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

    [TestFixture]
    public class TestSelectionTests
    {
        private CciModuleSource cci;
        private IModule dsaTest;
        private IModule dsa;
        private IModule dsaTest2;

        [SetUp]
        public void sss()
        {

            BasicConfigurator.Configure(
                new ConsoleAppender
                {
                    Layout = new SimpleLayout()
                });
            cci = new CciModuleSource();
            dsaTest2 = cci.AppendFromFile(MutationTestsHelper.DsaTestsPath2);
            dsaTest = cci.AppendFromFile(MutationTestsHelper.DsaTestsPath);
            dsa = cci.AppendFromFile(MutationTestsHelper.DsaPath);

        }
        public ICollection<MethodIdentifier> FindCovering(IModule module, MethodIdentifier constraints)
        {
            var typeManager = new CoveringTestsFinder();
            return typeManager.FindCoveringTests(module, new CciMethodMatcher(constraints));
        }
        [Test]
        public void TestRegex()
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
        public void ShouldFindCoveringTests3()
        {
            var constraints = new MethodIdentifier("Dsa.Utility.Guard", "ArgumentNull");

            var found = FindCovering(dsaTest2, constraints);
            Assert.AreEqual(3, found.Count());
        }

        [Test]
        public void ShouldFindCoveringTests2()
        {
            var constraints = new MethodIdentifier("Dsa.DataStructures.Deque`1", "EnqueueFront");

            var found = FindCovering(dsaTest, constraints);


            Assert.AreEqual(1, found.Count());

//            IMethodDefinition method = found.Single();
//            var def = method.ContainingTypeDefinition as INamespaceTypeDefinition;
//
//            Assert.NotNull(def);
//            var result = (from m in found
//                    let t = m.ContainingTypeDefinition as INamespaceTypeDefinition
//                    where t != null
//                    select new MethodIdentifier(
//                        TypeHelper.GetNamespaceName(t.ContainingUnitNamespace, NameFormattingOptions.None)
//                         + "." + t.Name.Value, m.Name.Value)).ToList();
//
//            Assert.IsNotEmpty(result);
        }
       
    }
}