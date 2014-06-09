namespace VisualMutator.Tests.Mutations
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Microsoft.Cci;
    using Model;
    using Model.Mutations;
    using Model.Mutations.MutantsTree;
    using Model.Mutations.Types;
    using Model.Tests;
    using Model.Tests.TestsTree;
    using NUnit.Framework;
    using SoftwareApproach.TestingExtensions;
    using UsefulTools.CheckboxedTree;
    using UsefulTools.ExtensionMethods;
    using UsefulTools.Paths;

    #endregion

    [TestFixture]
    public class NamespaceGrouperTests
    {
      
        [Test]
        public void TestResultTreeCreator()
        {
            var list = new List<TmpTestNodeMethod>()
                       {
                           new TmpTestNodeMethod("A.qeq.ri.T1.M1"),
                           new TmpTestNodeMethod("B.wow.io.T2.M2"),
                           new TmpTestNodeMethod("C.oeio.ww.T2.M1"),
                           new TmpTestNodeMethod("C.oeio.ww.T2.M2"),
                           new TmpTestNodeMethod("C.oeio.ww.T3.M1"),
                       };
            

            var cr = new TestResultTreeCreator();

            List<TestNodeNamespace> testNodeNamespaces = cr.CreateMutantTestTree(list).ToList();

            testNodeNamespaces.Count.ShouldEqual(3);
            testNodeNamespaces.Single(n => n.Name == "C.oeio.ww").Children.Count.ShouldEqual(2);
            testNodeNamespaces.Single(n => n.Name == "C.oeio.ww")
                .Children.Cast<TestNodeClass>().Single(c => c.Name == "T2")
                .Children.Cast<TestNodeMethod>().Count().ShouldEqual(2); //Single(c => c.Name == "M1");
        }
        
        [Test]
        public void Test1()
        {
            System.Action<CheckedNode, ICollection<MutationTarget>> typeNodeCreator = (parent, targets) =>
            {
               
            };

            Func<MutationTarget, string> namespaceExtractor = target => target.NamespaceName;

            var mt1 = new MutationTarget(null);
            mt1.NamespaceName = "Root.NamespaceName1";

            var mt2 = new MutationTarget(null);
            mt2.NamespaceName = "Root.NamespaceName2";

            var mt3 = new MutationTarget(null);
            mt3.NamespaceName = "Another.NamespaceName3";

            var mt4 = new MutationTarget(null);
            mt4.NamespaceName = "Another.NamespaceName3";

            var ass = new AssemblyNode("");
            NamespaceGrouper<MutationTarget, CheckedNode>.
                GroupTypes(ass, namespaceExtractor, 
                (parent, name) => new TypeNamespaceNode(parent, name), typeNodeCreator,
                    new List<MutationTarget> { mt1, mt2, mt3 });

            ass.Children.Count.ShouldEqual(2);
            ass.Children.Single(c => c.Name == "Root").Children.Count.ShouldEqual(2);
            ass.Children.Single(c => c.Name == "Another.NamespaceName3").Children.Count.ShouldEqual(0);
        }
    }
}