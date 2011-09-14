namespace PiotrTrzpil.VisualMutator_VSPackage.Infrastructure.WpfUtils
{
    #region Usings

    using System;
    using System.ComponentModel;
    using System.Linq.Expressions;
    using System.Windows.Forms;

    #endregion

    public abstract class ModelElement : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged<T>(Expression<Func<T>> propertyExpression)
        {
            string propertyName = propertyExpression.PropertyName();

            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }
       
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, e);
            }
        }
    }
}