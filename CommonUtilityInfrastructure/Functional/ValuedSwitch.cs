namespace CommonUtilityInfrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class ValuedSwitch<T, R>
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

        public bool HasResult
        {
            get;
            set;
        }

        public R Result
        {
            get;
            set;
        }
    }

    public static class ValuedSwitchMixin
    {

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
        public static R GetResult<T, R>(this ValuedSwitch<T, R> @switch)
        {
            if (!@switch.HasResult)
            {
                throw new InvalidOperationException("No case matched value: " + @switch.Value);
            }
            return @switch.Result;
        }
    }
}