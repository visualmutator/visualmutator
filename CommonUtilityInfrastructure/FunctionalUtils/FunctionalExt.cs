namespace CommonUtilityInfrastructure.FunctionalUtils
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class FunctionalExt
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
                        var children = selector(obj)?? new List<TSource>();
                        var selected = children.Where(predicate).SelectMany(f);
                        return selected.Any() ? selected : selected.Prepend(obj);
                    }));
            }


            return source.Where(predicate).SelectMany(Y<TSource, IEnumerable<TSource>>
                (f => obj => selector(obj).Where(predicate).SelectMany(f).Prepend(obj)));


        }



        public static bool IsIn<T>(this T obj, params T[] values)
        {
            return values.Contains(obj);
        }


        public static SwitchIntoSelector<R> SwitchInto<R>(R obj)
        {
            return new SwitchIntoSelector<R>();
        }

        public static Switch<T> Switch<T>(T obj)
        {
            return new Switch<T>(obj);
        }

        public static ValuedSwitch<T, R> ValuedSwitch<T, R>(T obj)
        {
            return new ValuedSwitch<T, R>(obj);
        }

        public static ValuedTypeSwitch<R> ValuedTypeSwitch<R>(object obj)
        {
            return new ValuedTypeSwitch<R>(obj);
        }

        public static TypeSwitch TypeSwitch<T>(T obj)
        {
            return new TypeSwitch(obj);
        }


        public static ValuedSwitch<T, R> From<T,R>(this SwitchIntoSelector<R> obj, T from)
        {
            return new ValuedSwitch<T, R>(from);
        }
        public static CollectiveSwitch<T, R> AsCascadingCollectiveOf<T, R>(this SwitchIntoSelector<R> obj, IEnumerable<T> from)
        {
            return new CollectiveSwitch<T, R>(from);
        }
        public static ValuedTypeSwitch<R> FromTypeOf<R>(this SwitchIntoSelector<R> sel, object obj)
        {
            return new ValuedTypeSwitch<R>(obj);
        }

    }
    public static class Switch
    {
        public static SwitchIntoSelector<R> Into<R>()
        {
            return new SwitchIntoSelector<R>();
        }
        public static Switch<T> On<T>(T obj)
        {
            return new Switch<T>(obj);
        }

    }
    public static class Throw
    {
        public static void If(bool condition, string message=null, string parameterName=null)
        {
            if(message == null)
            {
                message = "Object is in invalid state";
            }
            if (condition)
            {
                throw new InvalidOperationException(message);
            }
        }
        public static void IfNull<T>(T parameter, string parameterName = null )
        {
            if (parameterName == null)
            {
                parameterName = "";
            }
            if (parameter == null)
            {
                throw new ArgumentNullException("Parameter {0} cannot be null".Formatted(parameterName));
            }
        }
    }
    public class SwitchIntoSelector<T>
    {
        public SwitchIntoSelector()
        {
 
        }

    }



}