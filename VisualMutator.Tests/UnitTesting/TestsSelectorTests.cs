namespace VisualMutator.Tests.UnitTesting
{
    using Model.Tests;
    using NUnit.Framework;
    using SoftwareApproach.TestingExtensions;
    using UsefulTools.CheckboxedTree;


    [TestFixture]
    public class TestsSelectorTests
    {
         [Test]
        public void Tet()
         {
             var a = new CheckedNode("A");

             var aa = new CheckedNode("AA", false) {Parent = a};
             var ab = new CheckedNode("AB"){ Parent = a};

             var aba = new CheckedNode("ABA"){ Parent = ab};
             var abaa = new CheckedNode("ABAA", false){ Parent = aba};

             var abb = new CheckedNode("ABB"){ Parent = ab};

             var abba = new CheckedNode("ABBA", false){ Parent = abb};
             var abbb = new CheckedNode("ABBA", false){ Parent = abb};

             a.Children.Add(aa);
             a.Children.Add(ab);
             ab.Children.Add(aba);
             ab.Children.Add(abb);
             abb.Children.Add(abba);
             abb.Children.Add(abbb);
             aba.Children.Add(abaa);

             a.IsIncluded = false;
             abba.IsIncluded = true;
             aba.IsIncluded = true;

             var testsSelector = new TestsSelector();
             string minimalTreeId = testsSelector.MinimalTreeId(a, n => n.Name, n => n.Children);

             minimalTreeId.ShouldEqual("ABA ABBA");
         }

        [Test]
        public void Should_get_included_tests()
        {

            var a = new CheckedNode("A");

            var aa = new CheckedNode("AA", false) { Parent = a };
            var ab = new CheckedNode("AB") { Parent = a };

            var aba = new CheckedNode("ABA") { Parent = ab };
            var abaa = new CheckedNode("ABAA", false) { Parent = aba };

            var abb = new CheckedNode("ABB") { Parent = ab };

            var abba = new CheckedNode("ABBA", false) { Parent = abb };
            var abbb = new CheckedNode("ABBA", false) { Parent = abb };

            a.Children.Add(aa);
            a.Children.Add(ab);
            ab.Children.Add(aba);
            ab.Children.Add(abb);
            abb.Children.Add(abba);
            abb.Children.Add(abbb);
            aba.Children.Add(abaa);

            a.IsIncluded = false;
            abba.IsIncluded = true;
            aba.IsIncluded = true;

            var testsSelector = new TestsSelector();
            string minimalTreeId = testsSelector.MinimalTreeId(a, n => n.Name, n => n.Children);

            minimalTreeId.ShouldEqual("ABA ABBA");
        }
    }
}