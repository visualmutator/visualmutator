namespace PiotrTrzpil.VisualMutator_VSPackage.Infrastructure.WpfUtils
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq.Expressions;
    using System.Reflection;

    #endregion

    public static class UtilityExtensionMethods
    {
        public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> toAdd)
        {
            foreach (T item in toAdd)
            {
                collection.Add(item);
            }
        }

        public static string PropertyName<T>(Expression<Func<T>> propertyExpression)
        {
            var memberExpression = propertyExpression.Body as MemberExpression;

            var property = memberExpression.Member as PropertyInfo;

            return memberExpression.Member.Name;
        }

        public static bool PropertyChanged<T>(
            this PropertyChangedEventArgs e, Expression<Func<T>> propertyExpression)
        {
            return e.PropertyName == PropertyName(propertyExpression);
        }
    }
}