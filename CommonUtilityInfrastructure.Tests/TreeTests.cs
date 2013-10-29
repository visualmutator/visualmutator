namespace CommonUtilityInfrastructure.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using CheckboxedTree;
    using CommonUtilityInfrastructure;
    using FunctionalUtils;
    using NUnit.Framework;
    using VisualMutator.Tests.Util;
    using Switch = FunctionalUtils.Switch;

    [TestFixture]
    public class TreeTests
    {
        

        private CheckedNode CreateTree()
        {
            var o = new CheckedNode("o");
            var a = new CheckedNode("a");
            var ab = new CheckedNode("ab");
            var aba = new CheckedNode("aba", false);
            var b = new CheckedNode("b");
            var ba = new CheckedNode("ba");
            var baa = new CheckedNode("baa", false);
            var bab = new CheckedNode("bab", false);
            var bac = new CheckedNode("bac");
            var baca = new CheckedNode("baca", false);

            o.Children.AddRange(new[] {a, b});
            a.Children.AddRange(new[] {ab});
            ab.Children.AddRange(new[] {aba});
            b.Children.AddRange(new[] {ba});
            ba.Children.AddRange(new[] {baa, bab, bac});
            bac.Children.AddRange(new[] {baca});

            o.IsIncluded = true;
            b.IsIncluded = false;

            return o;
        }

        [Test]
        public void SelectManyRecursive_Normal()
        {
            var o = CreateTree();


            var result = new[] {o}.SelectManyRecursive(n => n.Children, n=> n.IsIncluded ?? true, leafsOnly:true).ToList();

            result.Count().ShouldEqual(1);
            result.Select(obj => obj.ToString()).Distinct().Count().ShouldEqual(1);
       
        }

        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void InvalidSelfChild()
        {
            var a = new CheckedNode("a");
            var ab = new CheckedNode("ab");

            ab.Parent = a;

            a.Children.Add(a);

        }
        [Test]
        public void ValidParent()
        {
  
            var a = new CheckedNode("a");
            var ab = new CheckedNode("ab");

            ab.Parent = a;

            a.Children.Add(ab);

        }
        [Test,ExpectedException(typeof(InvalidOperationException))]
        public void InvalidParent()
        {
            var o = new CheckedNode("o");
            var a = new CheckedNode("a");
            var ab = new CheckedNode("ab");

            ab.Parent = a;

            o.Children.Add(a);
           
        }

        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void TooLateParent()
        {
            var o = new CheckedNode("o");
            var a = new CheckedNode("a");
            var ab = new CheckedNode("ab");

            o.Children.Add(a);

            ab.Parent = a;
        }

        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void InvalidSelfParent()
        {
            var ab = new CheckedNode("ab");

            ab.Parent = ab;
        }
    }
}

