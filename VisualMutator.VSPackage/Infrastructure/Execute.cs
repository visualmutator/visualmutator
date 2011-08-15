namespace PiotrTrzpil.VisualMutator_VSPackage.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Windows.Threading;

    public interface IExecute
    {
        void OnUIThread(Action action);
    }

    public class Execute : IExecute
    {
        private Action<Action> _executor = action => action();

       
        public void InitializeWithDispatcher()
        {

            var dispatcher = Application.Current.Dispatcher;

            _executor = action =>
            {
                if (dispatcher.CheckAccess())
                    action();
                else
                    dispatcher.BeginInvoke(action);
            };
        }

        
        public void OnUIThread(Action action)
        {
            _executor(action);
        }
    }

}