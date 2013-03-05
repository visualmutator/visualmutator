namespace CommonUtilityInfrastructure.Tests
{
    using NUnit.Framework;
    using VisualMutator.Tests.Util;

    [TestFixture]
    public class ProgressCounterTest
    {
         [Test]
         public void Ts()
         {
             int current = -1;
             var progress = ProgressCounter.Invoking(val =>
                 {
                     current = val;
                 });

             progress.Initialize(10);

             progress.Progress();


             current.ShouldEqual(10);

             var sub = progress.CreateSubprogress();

             sub.Initialize(10);

             sub.Progress();

             current.ShouldEqual(11);

             progress.Progress();

             current.ShouldEqual(20);

             sub.Initialize(5);

             sub.Progress();

             current.ShouldEqual(22);

             sub.Progress();
             sub.Progress();
             sub.Progress();
             sub.Progress();
             current.ShouldEqual(30);

             progress.Progress();

             current.ShouldEqual(30);

             sub.Initialize(1);

             sub.Progress();
             current.ShouldEqual(40);
         }


    }
}