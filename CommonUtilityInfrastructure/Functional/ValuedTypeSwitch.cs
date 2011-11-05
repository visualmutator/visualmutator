namespace CommonUtilityInfrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class ValuedTypeSwitch<R>
    {
        public ValuedTypeSwitch(object o)
        {
            Value = o;
        }

        public object Value
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

    public static class ValuedTypeSwitchMixin
    {

        public static ValuedTypeSwitch< R> Case<T, R>(this ValuedTypeSwitch<R> @switch, Func<T,R> action) where T : class
        {
            T val = @switch.Value as T;
 
            if (@switch.HasResult)
            {
                if (val != null)
                {
                    throw new InvalidOperationException("Match was already found: " + @switch.Value);
                }
            }
            else if (val != null)
            {
                @switch.Result = action(val);
                @switch.HasResult = true;
            }

            return @switch;
        }


   
        public static R GetResult<R>(this ValuedTypeSwitch< R> @switch)
        {
            if (!@switch.HasResult)
            {
                throw new InvalidOperationException("No case matched value: " + @switch.Value);
            }
            return @switch.Result;
        }
    }
}