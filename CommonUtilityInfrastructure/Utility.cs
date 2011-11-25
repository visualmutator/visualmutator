namespace CommonUtilityInfrastructure
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Xml.Linq;

    using CommonUtilityInfrastructure.WpfUtils;

    #endregion

    public static class Utility
    {

        
        
        public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> toAdd)
        {
            foreach (T item in toAdd)
            {
                collection.Add(item);
            }
            
        }
  
        public static BetterObservableCollection<T> ToObsCollection<T>(this IEnumerable<T> collection)
        {
            var obs = new BetterObservableCollection<T>();
            foreach (T item in collection)
            {
                obs.Add(item);
            }
            return obs;
        }
        public static IEnumerable<T> ToEmptyIfNull<T>(this IEnumerable<T> enumerable)
        {
            return enumerable ?? Enumerable.Empty<T>();
        }
        public static bool ContainsAny<T>(this IEnumerable<T> enumerable, T val1, T val2)
        {
            return enumerable.Any(obj => obj.Equals(val1) || obj.Equals(val2));
        }
        public static bool ContainsAny<T>(this IEnumerable<T> enumerable, T val1, T val2, T val3)
        {
            return enumerable.Any(obj => obj.Equals(val1) || obj.Equals(val2) || obj.Equals(val3));
        }
        public static string InQuotes(this string str)
        {
            return string.Format(@"""{0}""", str);
        }
        public static string Formatted(this string str, params object[] args)
        {
            return string.Format(str, args);
        }
        public static int IncrementedIf(this int value, bool condition)
        {
            return condition ? value + 1 : value;
        }
        public static int AsPercentageOf(this int part, int all)
        {
            Throw.If(part < 0 || all < 0 || part > all || all == 0);
            return (int)(((double)part / all) * 100);
        }
        public static double RoundToSignificantDigits(this double d, int digits)
        {
            double scale = Math.Pow(10, Math.Floor(Math.Log10(d)) + 1);
            return scale * Math.Round(d / scale, digits);
        }

        public static string PropertyName<T>(this Expression<Func<T>> propertyExpression)
        {
            var memberExpression = propertyExpression.Body as MemberExpression;
            return memberExpression.Member.Name;
        }
        public static string PropertyName<Param, T>(this Expression<Func<Param,T>> propertyExpression)
        {
            var memberExpression = propertyExpression.Body as MemberExpression;
            return memberExpression.Member.Name;
        }



        public static bool PropertyChanged<T>(
            this PropertyChangedEventArgs e, Expression<Func<T>> propertyExpression)
        {
            return e.PropertyName == PropertyName(propertyExpression);
        }



        public static IEnumerable<XElement> ElementsAnyNS<T>(this T source, string localName)
    where T : XElement
        {
            return source.Elements().Where(e => e.Name.LocalName == localName);
        }
        public static XElement ElementAnyNS<T>(this T source, string localName)
    where T : XElement
        {
            return source.Elements().Single(e => e.Name.LocalName == localName);
        }
        public static IEnumerable<XElement> DescendantsAnyNs<T>(this T source, string localName)
    where T : XElement
        {
            return source.Descendants().Where(e => e.Name.LocalName == localName);
        }
    }
}