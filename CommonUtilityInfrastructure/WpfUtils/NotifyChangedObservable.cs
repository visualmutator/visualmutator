using System.Collections;
namespace CommonUtilityInfrastructure.WpfUtils
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Text;
    using System.Windows;

    public class NotifyChangedObservable<T> : IObservable<T>, IWeakEventListener, IDisposable
    {
        private readonly INotifyPropertyChanged _source;

        private readonly string _propertyName;

        private IObserver<T> _observer;

        private IPropertyAccessor _propertyAccessor;

        public NotifyChangedObservable(INotifyPropertyChanged source, string propertyName)
        {
            _source = source;
            _propertyName = propertyName;
            var propertyInfo = _source.GetType().GetProperty(propertyName);
            _propertyAccessor = PropertyInfoHelper.GetFastAccessor(propertyInfo);
            
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            _observer = observer;
            PropertyChangedEventManager.AddListener(_source, this, _propertyName);
            return this;
        }

        public bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
           // var eArgs = (PropertyChangedEventArgs)e ;

            T value = (T)_propertyAccessor.GetValue(_source);

            _observer.OnNext(value);

            return true;
        }

        public void Dispose()
        {
            PropertyChangedEventManager.RemoveListener(_source, this, _propertyName);
        }
    }

    public static class Mixin
    {
        public static NotifyChangedObservable<T> WhenPropertyChanged<T>(this INotifyPropertyChanged @this, 
            Expression<Func<T>> propertyExpression)
        {

            return new NotifyChangedObservable<T>(@this, propertyExpression.PropertyName());
 
           
            
        }
    }

    
}