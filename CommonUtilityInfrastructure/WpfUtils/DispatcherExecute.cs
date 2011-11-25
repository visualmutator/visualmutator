namespace CommonUtilityInfrastructure.WpfUtils
{
    #region Usings

    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Threading;

    #endregion

    public interface IDispatcherExecute
    {
        void OnUIThread(Action action);

        TaskScheduler GuiScheduler { get; }

        SynchronizationContext GuiSyncContext { get; }
    }

    public class DispatcherExecute : IDispatcherExecute
    {

        private TaskScheduler _guiScheduler;

        private Dispatcher _dispatcher;

        private SynchronizationContext _syncContext;

        public void OnUIThread(Action action)
        {
            if (_dispatcher.CheckAccess())
            {
                action();
            }
            else
            {
                _dispatcher.BeginInvoke(action);
            }
        }

        public TaskScheduler GuiScheduler
        {
            get
            {
                return _guiScheduler;
            }
        }

        public SynchronizationContext GuiSyncContext
        {
            get
            {
                return _syncContext;
            }
        }

        /// <summary>
        /// Must be called on dispatcher thread.
        /// </summary>
        public void InitializeWithDispatcher()
        {
            _dispatcher = Application.Current.Dispatcher;
            _guiScheduler = TaskScheduler.FromCurrentSynchronizationContext();

            _syncContext = SynchronizationContext.Current;


        }
    }
}