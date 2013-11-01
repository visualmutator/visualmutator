namespace CommonUtilityInfrastructure.WpfUtils
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    public interface IPropertyAccessor
    {
        PropertyInfo PropertyInfo
        {
            get;
        }

        string Name
        {
            get;
        }

        object GetValue(object source);

        void SetValue(object source, object value);
    }


    public static class PropertyInfoHelper
    {
        private static Dictionary<PropertyInfo, IPropertyAccessor> _cache 
            = new Dictionary<PropertyInfo, IPropertyAccessor>();
 
        public static IPropertyAccessor GetFastAccessor(PropertyInfo propertyInfo)
        {
            IPropertyAccessor result;
            lock (_cache)
            {
                if (!_cache.TryGetValue(propertyInfo, out result))
                {
                    result = CreateAccessor(propertyInfo);
                    _cache.Add(propertyInfo, result);
                }
            }
            return result;
        }

   
        public static IPropertyAccessor CreateAccessor(PropertyInfo propertyInfo)
        {
            return (IPropertyAccessor)Activator.CreateInstance(
                        typeof(PropertyWrapper<,>).MakeGenericType
                           (propertyInfo.DeclaringType, propertyInfo.PropertyType), propertyInfo);
        }
    }

    internal class PropertyWrapper<TObject, TValue> : IPropertyAccessor
    {
        private PropertyInfo _propertyInfo;

        private Func<TObject, TValue> _getMethod;
        private Action<TObject, TValue> _setMethod;


        public PropertyWrapper(PropertyInfo propertyInfo)
        {
            _propertyInfo = propertyInfo;

            MethodInfo mGet = propertyInfo.GetGetMethod(true);
            MethodInfo mSet = propertyInfo.GetSetMethod(true);

            _getMethod = (Func<TObject, TValue>)Delegate.CreateDelegate
                    (typeof(Func<TObject, TValue>), mGet);
            _setMethod = (Action<TObject, TValue>)Delegate.CreateDelegate
                    (typeof(Action<TObject, TValue>), mSet);
        }

        object IPropertyAccessor.GetValue(object source)
        {
            return _getMethod((TObject)source);
        }
        void IPropertyAccessor.SetValue(object source, object value)
        {
            _setMethod((TObject)source, (TValue)value);
        }


        public string Name
        {
            get
            {
                return _propertyInfo.Name;
            }
        }

        public PropertyInfo PropertyInfo
        {
            get
            {
                return _propertyInfo;
            }
        }

    }

}