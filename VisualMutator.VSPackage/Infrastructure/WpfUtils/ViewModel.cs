namespace PiotrTrzpil.VisualMutator_VSPackage.Infrastructure.WpfUtils
{
    #region Usings

    using System;
    using System.Threading;
    using System.Windows.Threading;

    #endregion

    public abstract class ViewModel<TView> : ModelElement
        where TView : class, PiotrTrzpil.VisualMutator_VSPackage.Infrastructure.WpfUtils.IView
    {
        private readonly EventListeners eventListeners;

        private readonly TView view;

        protected ViewModel(TView view)
        {
            if (view == null)
            {
                throw new ArgumentNullException("view");
            }
            this.view = view;
            eventListeners = new EventListeners();

            if (SynchronizationContext.Current is DispatcherSynchronizationContext)
            {
                // Is running within WPF
                Dispatcher.CurrentDispatcher.BeginInvoke(
                    (Action)delegate
                    {
                        // Delay
                        view.DataContext = this;
                    });
            }
            else
            {
                view.DataContext = this;
            }
        }

        public EventListeners EventListeners
        {
            get
            {
                return eventListeners;
            }
        }

        public TView View
        {
            get
            {
                return view;
            }
        }
    }
}