namespace CommonUtilityInfrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public static class Functional
    {
        // One-argument Y-Combinator.
        public static Func<A, TResult> Y<A, TResult>(Func<Func<A, TResult>, Func<A, TResult>> f)
        {
            Func<A, TResult> g = null;
            g = f(a => g(a));
            return g;
        }

        // Two-argument Y-Combinator.
        public static Func<T1, T2, TResult> Y<T1, T2, TResult>(Func<Func<T1, T2, TResult>, Func<T1, T2, TResult>> F)
        {
            return (t1, t2) => F(Y(F))(t1, t2);
        }

        // Three-arugument Y-Combinator.
        public static Func<T1, T2, T3, TResult> Y<T1, T2, T3, TResult>(Func<Func<T1, T2, T3, TResult>, Func<T1, T2, T3, TResult>> F)
        {
            return (t1, t2, t3) => F(Y(F))(t1, t2, t3);
        }

        // Four-arugument Y-Combinator.
        public static Func<T1, T2, T3, T4, TResult> Y<T1, T2, T3, T4, TResult>(Func<Func<T1, T2, T3, T4, TResult>, Func<T1, T2, T3, T4, TResult>> F)
        {
            return (t1, t2, t3, t4) => F(Y(F))(t1, t2, t3, t4);
        }

        // Curry first argument
        public static Func<T1, Func<T2, TResult>> Curry<T1, T2, TResult>(Func<T1, T2, TResult> F)
        {
            return t1 => t2 => F(t1, t2);
        }

        // Curry second argument.
        public static Func<T2, Func<T1, TResult>> Curry2nd<T1, T2, TResult>(Func<T1, T2, TResult> F)
        {
            return t2 => t1 => F(t1, t2);
        }

        // Uncurry first argument.
        public static Func<T1, T2, TResult> Uncurry<T1, T2, TResult>(Func<T1, Func<T2, TResult>> F)
        {
            return (t1, t2) => F(t1)(t2);
        }

        // Uncurry second argument.
        public static Func<T1, T2, TResult> Uncurry2nd<T1, T2, TResult>(Func<T2, Func<T1, TResult>> F)
        {
            return (t1, t2) => F(t2)(t1);
        }

        public static IEnumerable<TSource> Prepend<TSource>(this IEnumerable<TSource> source, TSource element)
        {
            return new[] { element }.Concat(source);
        }

        public static IEnumerable<T> Append<T>(this IEnumerable<T> source, T element)
        {
            return source.Concat(Enumerable.Repeat(element, 1));
            
        }

        public static IEnumerable<T> ToEnumerable<T>(this T obj)
        {
            return new[] { obj };
        } 


   

        public static IEnumerable<TSource> SelectManyRecursive<TSource>( this IEnumerable<TSource> source,
            Func<TSource, IEnumerable<TSource>> selector, Func<TSource, bool> predicate = null, bool leafsOnly = false)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            if (predicate == null)
            {
                predicate = x => true;
            }

            if (leafsOnly)
            {
                return source.Where(predicate).SelectMany(Y<TSource, IEnumerable<TSource>>
                    (f => obj =>
                    {
                        var s = selector(obj).Where(predicate).SelectMany(f);
                        return s.Any() ? s : s.Prepend(obj);
                    }));
            }


            return source.Where(predicate).SelectMany(Y<TSource, IEnumerable<TSource>>
                (f => obj => selector(obj).Where(predicate).SelectMany(f).Prepend(obj)));


        }
    }

}