namespace VisualMutator.Tests.Infrastructure
{
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using Model.StoringMutants;
    using NUnit.Framework;
    using SoftwareApproach.TestingExtensions;
    using UsefulTools.Paths;

    [TestFixture]
    public class FileSystemManagerTests
    {
        [Test]
         public async void TestCopying()
         {
            File.Delete(@"C:\PLIKI\p.txt");
            Directory.Delete(@"C:\PLIKI\Test", true);
            var m = new FilesManager(null);
            Directory.CreateDirectory(@"C:\PLIKI\Test");
            //File.Exists(@"C:\PLIKI\p.txt").ShouldBeTrue();

            List<Task> l = new List<Task>();
            for (int i = 0; i < 100; i++)
            {
                int i1 = i;
                var task = m.CopyOverwriteAsync(@"C:\PLIKI\t.txt".ToFilePathAbs(),
                    (@"C:\PLIKI\Test\p.txt" + i1).ToFilePathAbs());
              //  var task = Task.Run(
              //      () =>
              //         .Wait());
                l.Add(task);
            }
            Task.WaitAll(l.ToArray());



           
        }
    }
}