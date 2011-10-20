namespace CommonUtilityInfrastructure.WpfUtils
{
    #region Usings

    using System;
    using System.ComponentModel;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Windows.Threading;

    #endregion

    public abstract class ViewModel<TView> : ModelElement, IEventNotifier
        where TView : class, IView
    {
        private readonly EventListeners _eventListeners;

        private readonly TView _view;

        protected ViewModel(TView view)
        {
            if (view == null)
            {
                throw new ArgumentNullException("view");
            }
            this._view = view;
            _eventListeners = new EventListeners();

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
                return _eventListeners;
            }
        }

        public TView View
        {
            get
            {
                return _view;
            }
        }

        public NotifyChangedObservable<T> RegisterPropertyChanged<T>(Expression<Func<T>> propertyExpression)
        {
            var changedObservable = this.WhenPropertyChanged(propertyExpression);
            EventListeners.AddReference(changedObservable);
            
            return changedObservable;

        }

        public void AddListener<T>(Expression<Func<T>> propertyExpression, Action action )
        {
            EventListeners.Add(this, propertyExpression.PropertyName(),action);
        }
    }
}