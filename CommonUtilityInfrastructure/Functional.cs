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
                        var children = selector(obj)?? new List<TSource>();
                        var selected = children.Where(predicate).SelectMany(f);
                        return selected.Any() ? selected : selected.Prepend(obj);
                    }));
            }


            return source.Where(predicate).SelectMany(Y<TSource, IEnumerable<TSource>>
                (f => obj => selector(obj).Where(predicate).SelectMany(f).Prepend(obj)));


        }

        public static Switch<T> Switch<T>(T obj)
        {
            return new Switch<T>(obj);
        }

        public static ValuedSwitch<T, R> ValuedSwitch<T, R>(T obj)
        {
            return new ValuedSwitch<T, R>(obj);
        }
        public static Switch<T> Case<T>(this Switch<T> @switch, T caseValue, Action action)
        {
            if (@switch.Value.Equals(caseValue))
            {
                action();
                @switch.HasResult = true;
            }
            return @switch;
        }
        public static ValuedSwitch<T, R> Case<T, R>(this ValuedSwitch<T, R> @switch, T caseValue, Func<R> action)
        {
            if (@switch.HasResult)
            {
                if (@switch.Value.Equals(caseValue))
                {
                    throw new InvalidOperationException("Match was already found: " + @switch.Value);
                }


            }
            else if (@switch.Value.Equals(caseValue))
            {
                @switch.Result = action();
                @switch.HasResult = true;
            }
            
            return @switch;
        }
        public static ValuedSwitch<T, R> Case<T, R>(this ValuedSwitch<T, R> @switch, T caseValue, R result)
        {
            return @switch.Case(caseValue, () => result);
        }
        public static R GetResult<T,R>(this ValuedSwitch<T,R> @switch)
        {
            if (!@switch.HasResult)
            {
                throw new InvalidOperationException("No case matched value: " + @switch.Value);
            }
            return @switch.Result;
        }

        public static void ThrowIfNoMatch<T>(this Switch<T> @switch)
        {
            if (!@switch.HasResult)
            {
                throw new InvalidOperationException("No case matched value: " + @switch.Value);
            }
        }
    }
    public class Switch<T>
    {
        public Switch(T o)
        {
            Value = o;
        }

        public T Value
        {
            get;
            private set;
        }

        public bool HasResult { get; set; }
    }

    public class ValuedSwitch<T,R> 
    {
        public ValuedSwitch(T o)
        {
            Value = o;
        }

        public T Value
        {
            get;
            private set;
        }

        public bool HasResult { get; set; }

        public R Result { get; set; }
    }
}