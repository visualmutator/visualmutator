namespace PiotrTrzpil.VisualMutator_VSPackage.Infrastructure.WpfUtils
{
    #region Usings

    using System;
    using System.Collections.Specialized;
    using System.Linq.Expressions;
    using System.Windows.Input;

    #endregion

    public class BasicCommand<TParam> : ICommand
    {
        protected readonly Func<TParam, bool> canExecute;

        protected readonly Action<TParam> execute;

        public BasicCommand(Action<TParam> execute)
            : this(execute, null)
        {
        }

        protected BasicCommand(Action<TParam> execute, Func<TParam, bool> canExecute)
        {
            if (execute == null)
            {
                throw new ArgumentNullException("execute");
            }

            this.execute = execute;
            this.canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return canExecute == null || canExecute((TParam)parameter);
        }

        public void Execute(object parameter)
        {
            if (!CanExecute(parameter))
            {
                throw new InvalidOperationException("canExecute is false.");
            }

            execute((TParam)parameter);
        }

        public void RaiseCanExecuteChanged()
        {
            OnCanExecuteChanged(EventArgs.Empty);
        }

        public void UpdateOnChanged<T>(IEventNotifier notifier, 
            Expression<Func<T>> propertyExpression) 
        {
            
            notifier.EventListeners.Add(notifier, propertyExpression.PropertyName(),() =>
            {
                OnCanExecuteChanged(EventArgs.Empty);
            });
        }
        public void UpdateOnCollectionChanged(IEventNotifier notifier, INotifyCollectionChanged collection)
        {

            notifier.EventListeners.AddCollectionChangedEventHandler(collection, () =>
            {
                OnCanExecuteChanged(EventArgs.Empty);
            });
        }
        public void ExecuteOnChanged<T>(IEventNotifier notifier,
            Expression<Func<T>> propertyExpression)
        {
            notifier.EventListeners.Add(notifier, propertyExpression.PropertyName(), () =>
            {
                if (CanExecute(null))
                {
                    Execute(null);
                }
            });
        }
        //public void ExecuteOnChanged<T>(IEventNotifier notifier,
        //    Expression<Func<T>> propertyExpression, Func<bool> canExecuteFunc )
        //{
        //    notifier.EventListeners.Add(notifier, propertyExpression.PropertyName(), () =>
        //    {
        //        if (CanExecute(null))
        //        {
        //            Execute(null);
        //        }
        //    });
        //}
        protected virtual void OnCanExecuteChanged(EventArgs e)
        {
            EventHandler canExecuteChanged = CanExecuteChanged;
            if (canExecuteChanged != null)
            {
                canExecuteChanged(this, e);
            }
        }
    }

    public class BasicCommand : BasicCommand<object>
    {
        public BasicCommand(Action execute, Func<bool> canExecute = null)
            : base(execute != null ? p => execute() : (Action<object>)null,
                canExecute != null ? p => canExecute() : (Func<object, bool>)null)
        {
        }
    }
}