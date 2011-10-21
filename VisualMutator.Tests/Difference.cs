namespace VisualMutator.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using CommonUtilityInfrastructure.Comparers;

    using NUnit.Framework;

    [TestFixture]
    public class Difference
    {






        [Test]
        public void Test1()
        {

            string testMethod = @"

    //SomeNamespace.Namespace:
    public void Method()
    {
        int i = 0;
        for(int j=0; j<10; j++)
        {
            i = i + j;
        }
    }

";

            string mutatedMethod = @"

    //SomeNamespace.Namespace:
    public void Method()
    {
        int i = 0;
        for(int j=0; j<10; j++)
        {
            i = i - j;
        }
    }

";

            var diff = CodeAssert.GetDiff(testMethod, mutatedMethod);
        }
    }
}

