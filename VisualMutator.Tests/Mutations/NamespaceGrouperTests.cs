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
        public void Test1()
        {
            System.Action<CheckedNode, ICollection<MutationTarget>> typeNodeCreator = (parent, targets) =>
            {
               
            };

            Func<MutationTarget, string> namespaceExtractor = target => target.NamespaceName;
            Func<MutationTarget, string> nameExtractor = target => target.TypeName;

            var mt1 = new MutationTarget(null);
            mt1.TypeName = "Name1";
            mt1.NamespaceName = "Root.NamespaceName1";

            var mt2 = new MutationTarget(null);
            mt2.TypeName = "Name2";
            mt2.NamespaceName = "Root.NamespaceName2";

            var mt3 = new MutationTarget(null);
            mt3.TypeName = "Name3";
            mt3.NamespaceName = "Another.NamespaceName3";

            var ass = new AssemblyNode("", null);
            new NamespaceGrouper().
                GroupTypes2(ass, "", namespaceExtractor, nameExtractor, typeNodeCreator,
                    new List<MutationTarget> { mt1, mt2, mt3 });

            ass.Children.Count.ShouldEqual(2);
            ass.Children.Single(c => c.Name == "Root").Children.Count.ShouldEqual(2);
        }
    }
}