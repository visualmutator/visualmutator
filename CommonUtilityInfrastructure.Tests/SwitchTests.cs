namespace CommonUtilityInfrastructure.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using CommonUtilityInfrastructure;
    using FunctionalUtils;
    using NUnit.Framework;

    using Switch = FunctionalUtils.Switch;

    [TestFixture]
    public class SwitchTests
    {


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
                .CaseAll("C", "C")
                .GetValue();

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
                .CaseAll("A", "A").GetValue();

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
                .CaseAll("A", "A").GetValue();

            Assert.AreEqual("B", str);
        }
    }
}

