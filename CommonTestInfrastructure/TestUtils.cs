namespace VisualMutator.Tests.Util
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;

    using CommonUtilityInfrastructure;
    using CommonUtilityInfrastructure.Threading;
    using CommonUtilityInfrastructure.WpfUtils;
    using CommonUtilityInfrastructure.WpfUtils.Messages;

    using Moq;

    using NUnit.Framework;

    using VisualMutator.Infrastructure.Factories;

    #endregion

    public static class TestUtils
    {
        [DebuggerStepThrough]
        public static void ShouldEqual<T>(this T obj, T another)
        {
            Assert.AreEqual(another, obj);
        }

        public static IEnumerable<T> Each<T>(this IEnumerable<T> collection, Action<T> action)
        {
            var coll = collection.ToList();
            foreach (T item in coll)
            {
                action(item);
            }
            return coll;
        }
    }

    public static class Factory
    {

        public static FuncFactory<DateTime> DateTime(DateTime d)
        {
            return new FuncFactory<DateTime>(() => d);
        }

        public static FuncFactory<T> New<T>(Func<T> func)
        {
            return new FuncFactory<T>(func);
        }

    }


    public class TestScheduler : TaskScheduler
    {
        protected override void QueueTask(Task task)
        {
            base.TryExecuteTask(task);
        }

        protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
        {
            return false;
        }

        protected override IEnumerable<Task> GetScheduledTasks()
        {
            return Enumerable.Empty<Task>();
        }
    }


    public class FakeDispatcherExecute : IDispatcherExecute
    {
        public void OnUIThread(Action action)
        {
            action();
        }

        public TaskScheduler GuiScheduler
        {
            get
            {
                return new TestScheduler();
            }
        }
    }


    public static class Create
    {


        public static CommonServices TestServices()
        {
            var testScheduler = new TestScheduler();
            var messageService = new Mock<IMessageService>().Object;
            var eventService = new Mock<IEventService>().Object;



            var threadPooolExecuteMock = new Mock<IThreadPoolExecute>();
            threadPooolExecuteMock.SetupGet(_ => _.ThreadPoolScheduler).Returns(testScheduler);
            threadPooolExecuteMock.Setup(_ => _.LimitedThreadPoolScheduler(It.IsAny<int>())).Returns(testScheduler);


            var threading = new Threading(new FakeDispatcherExecute(), threadPooolExecuteMock.Object,messageService);

            return new CommonServices(messageService, eventService, threading);
        }


    }


}