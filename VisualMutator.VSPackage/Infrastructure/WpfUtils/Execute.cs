namespace PiotrTrzpil.VisualMutator_VSPackage.Infrastructure.WpfUtils
{
    #region Usings

    using System;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Threading;

    #endregion

    public interface IExecute
    {
        void OnUIThread(Action action);

        TaskScheduler WpfScheduler { get; }
    }

    public class Execute : IExecute
    {
        private Action<Action> _executor = action => action();

        private TaskScheduler _wpfScheduler;

        public void OnUIThread(Action action)
        {
            _executor(action);
        }

        public TaskScheduler WpfScheduler
        {
            get
            {
                return _wpfScheduler;
            }
        }

        public void InitializeWithDispatcher()
        {
            Dispatcher dispatcher = Application.Current.Dispatcher;
            _wpfScheduler = TaskScheduler.FromCurrentSynchronizationContext();


            _executor = action =>
            {
                if (dispatcher.CheckAccess())
                {
                    action();
                }
                else
                {
                    dispatcher.BeginInvoke(action);
                }
            };
        }
    }
}