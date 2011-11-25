namespace CommonUtilityInfrastructure.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using CommonUtilityInfrastructure;
    using NUnit.Framework;

    using Switch = CommonUtilityInfrastructure.Switch;

    [TestFixture]
    public class Tests
    {


        class Node
        {
            public int Val { get; set; }

            public string S { get; set; }

            public Node(int val, string s)
            {
                Val = val;
                S = s;
                Children = new List<Node>();
            }

            public List<Node> Children
            {
                get; private set;
            }

            public override string ToString()
            {
                return "Val: " + Val + " Str: " + S;
            }
        }

        private Node CreateTree()
        {
            var o = new Node(0, "o");
                var a = new Node(1, "a");
                    var ab = new Node(0, "ab");
                        var aba = new Node(1, "aba");
                var b = new Node(0, "b");
                    var ba = new Node(0, "ba");
                        var baa = new Node(1, "baa");
                        var bab = new Node(0, "bab");
                        var bac = new Node(1, "bac");
                            var baca = new Node(1, "baca");

            o.Children.AddRange(new[] { a, b });
            a.Children.AddRange(new[] { ab });
            ab.Children.AddRange(new[] { aba });
            b.Children.AddRange(new[] { ba });
            ba.Children.AddRange(new[] { baa, bab, bac });
            bac.Children.AddRange(new[] { baca });

            return o;
        }

        [Test]
        public void SelectManyRecursive_Normal()
        {
            var o = CreateTree();


            var result = new[]{o}.SelectManyRecursive(n => n.Children).ToList();

            Assert.AreEqual(10,result.Count());
            Assert.AreEqual(10, result.Select(obj => obj.ToString()).Distinct().Count());

        }


        [Test]
        public void Test2()
        {
            var o = CreateTree();


            var result = new[] { o }.SelectManyRecursive(n => n.Children, n => n.Val == 0).ToList();

            Assert.AreEqual(4, result.Count());


        }

        [Test]
        public void Test23()
        {
            var o = CreateTree();


            var result = new[] { o }.SelectManyRecursive(n => n.Children, leafsOnly: true).ToList();

            Assert.AreEqual(4, result.Count());


        }

        [Test]
        public void Test233()
        {
            var o = CreateTree();


            var result = new[] { o }.SelectManyRecursive(n => n.Children, n => n.Val == 0, leafsOnly: true).ToList();

            Assert.AreEqual(1, result.Count());

            Assert.AreEqual("bab", result.Single().S);
        }
        [Test]
        public void Test23322()
        {
            Trace.WriteLine(string.Format("= {0:F2}", 1d));
            Trace.WriteLine(string.Format("= {0:F2}", 1d/3d));
          


        }
        [Test]
        public void CollectiveSwitchTest1()
        {
            var strings = new[]
            {
                "A",
                "A",
                "C",
                "A",
                "A",
                "B",
                "A",
            };

            String str = Switch.Into<string>().AsCascadingCollectiveOf(strings)
                .CaseAny("A", "A")
                .CaseAll("C", "C");

            Assert.AreEqual("A", str);
        }
        [Test]
        public void CollectiveSwitchTest2()
        {
            var strings = new[]
            {
                "A",
                "A",
                "A",
                "A",
                "A",
                "A",
                "A",
            };

            String str = Switch.Into<string>().AsCascadingCollectiveOf(strings)
                .CaseAny("B", "B")
                .CaseAll("A", "A");

            Assert.AreEqual("A", str);
        }
        [Test]
        public void CollectiveSwitchTest3()
        {
            var strings = new[]
            {
                "A",
                "C",
                "A",
                "C",
                "B",
                "A",
                "A",
            };

            String str = Switch.Into<string>().AsCascadingCollectiveOf(strings)
                 .CaseAny("B", "B")
                .CaseAny("C", "C")
               
                
                .CaseAll("A", "A");

            Assert.AreEqual("B", str);
        }
    }
}

