namespace VisualMutator.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using CommonUtilityInfrastructure.Comparers;
    using Model.Decompilation.CodeDifference;
    using NUnit.Framework;

    using VisualMutator.Model;
    using VisualMutator.Model.Exceptions;
    using VisualMutator.Model.Mutations;

    using Assert = NUnit.Framework.Assert;

    [TestFixture]
    public class Difference
    {

        struct Ss
        {
            public int i;

            public Ss(int v)
            {
                i = v;
            }

        }


        [Test]
        public void Test0()
        {
            Ss aa = new Ss();
            System.Console.WriteLine(aa.i);
            Assert.Fail();
        }




        [Test]
        public void Test2()
        {
            var ilCodeLineEqualityComparer = new ILCodeLineEqualityComparer();
            Assert.IsTrue(ilCodeLineEqualityComparer.Equals(@"IL_00ca: mov 435", @"IL_0aeg: mov 435"));
        }

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
        int j=4;

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
        int j=4;
        int j2=5;
    }

";

          //  var diff = new CodeDifferenceCreator(
         //       new AssembliesManager()).GetDiff(CodeLanguage.CSharp, testMethod, mutatedMethod);
        }
    }
}

