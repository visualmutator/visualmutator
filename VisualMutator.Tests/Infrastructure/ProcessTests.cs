namespace VisualMutator.Tests.Infrastructure
{
    #region

    using System;
    using System.Diagnostics;
    using System.Reactive.Linq;
    using System.Threading;
    using Ninject;
    using Ninject.Activation.Strategies;
    using Ninject.Modules;
    using NUnit.Framework;
    using SoftwareApproach.TestingExtensions;
    using UsefulTools.DependencyInjection;
    using VisualMutator.Infrastructure;
    using VisualMutator.Infrastructure.NinjectModules;

    #endregion

    [TestFixture]
    public class ProcessTests
    {

        [Test]
        public void Test1()
        {
            var processes = new Processes();
            var p = new ProcessStartInfo("mspaint.exe");
           // p.
            var s = new CancellationTokenSource();
            processes.RunAsync(p, s.Token).ContinueWith(t =>
            {
                if(t.IsCanceled)
                {
                    Console.WriteLine("cancelled");
                 }
            });

            Observable.Timer(TimeSpan.FromSeconds(3)).Subscribe( a => s.Cancel());

            Thread.Sleep(10000);
        }
    }
}