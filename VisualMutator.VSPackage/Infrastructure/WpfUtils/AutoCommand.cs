namespace PiotrTrzpil.VisualMutator_VSPackage.Infrastructure.WpfUtils
{
    #region Usings

    using System;
    using System.Windows.Input;

    #endregion

    public class AutoCommand<TParam> : BasicCommand
    {
        protected readonly Func<TParam, bool> canExecute;

        protected readonly Action<TParam> execute;

        public AutoCommand(Action<TParam> execute)
            : this(execute, null)
        {
        }

        public AutoCommand(Action<TParam> execute, Func<TParam, bool> canExecute)
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
            return canExecute != null ? canExecute((TParam)parameter) : true;
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
        public BasicCommand(Action execute)
            : this(execute, null)
        {
        }

        public BasicCommand(Action execute, Func<bool> canExecute)
            : base(execute != null ? p => execute() : (Action<object>)null,
                canExecute != null ? p => canExecute() : (Func<object, bool>)null)
        {
        }
    }
}