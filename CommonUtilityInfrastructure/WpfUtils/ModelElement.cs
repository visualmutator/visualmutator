namespace PiotrTrzpil.VisualMutator_VSPackage.Infrastructure.WpfUtils
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq.Expressions;
  

    #endregion

    public abstract class ModelElement : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;


        protected void SetAndRise<T>(ref T field, T value, Expression<Func<T>> propertyExpression)
        {
            if (!EqualityComparer<T>.Default.Equals(field, value))
            {
                field = value;
                string propertyName = propertyExpression.PropertyName();
                OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
            }
            
        }

        protected void RaisePropertyChanged<T>(Expression<Func<T>> propertyExpression)
        {
            string propertyName = propertyExpression.PropertyName();

            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }
       
        protected void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}