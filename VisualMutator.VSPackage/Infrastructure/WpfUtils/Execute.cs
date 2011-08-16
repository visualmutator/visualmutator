namespace PiotrTrzpil.VisualMutator_VSPackage.Infrastructure.WpfUtils
{
    #region Usings

    using System;
    using System.Windows;
    using System.Windows.Threading;

    #endregion

    public interface IExecute
    {
        void OnUIThread(Action action);
    }

    public class Execute : IExecute
    {
        private Action<Action> _executor = action => action();

        public void OnUIThread(Action action)
        {
            _executor(action);
        }

        public void InitializeWithDispatcher()
        {
            Dispatcher dispatcher = Application.Current.Dispatcher;

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