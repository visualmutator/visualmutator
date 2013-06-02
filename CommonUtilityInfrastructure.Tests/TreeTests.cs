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
        

        private NormalNode CreateTree()
        {
            var o = new NormalNode("o");
            var a = new NormalNode("a");
            var ab = new NormalNode("ab");
            var aba = new NormalNode("aba", false);
            var b = new NormalNode("b");
            var ba = new NormalNode("ba");
            var baa = new NormalNode("baa", false);
            var bab = new NormalNode("bab", false);
            var bac = new NormalNode("bac");
            var baca = new NormalNode("baca", false);

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
            var a = new NormalNode("a");
            var ab = new NormalNode("ab");

            ab.Parent = a;

            a.Children.Add(a);

        }
        [Test]
        public void ValidParent()
        {
  
            var a = new NormalNode("a");
            var ab = new NormalNode("ab");

            ab.Parent = a;

            a.Children.Add(ab);

        }
        [Test,ExpectedException(typeof(InvalidOperationException))]
        public void InvalidParent()
        {
            var o = new NormalNode("o");
            var a = new NormalNode("a");
            var ab = new NormalNode("ab");

            ab.Parent = a;

            o.Children.Add(a);
           
        }

        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void TooLateParent()
        {
            var o = new NormalNode("o");
            var a = new NormalNode("a");
            var ab = new NormalNode("ab");

            o.Children.Add(a);

            ab.Parent = a;
        }

        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void InvalidSelfParent()
        {
            var ab = new NormalNode("ab");

            ab.Parent = ab;
        }
    }
}

