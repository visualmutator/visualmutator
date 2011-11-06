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

        public  ValuedTypeSwitch<R> Case<T>( Func<T,R> action) where T : class
        {
            T val = this.Value as T;

            if (this.HasResult)
            {
                if (val != null)
                {
                    throw new InvalidOperationException("Match was already found: " + this.Value);
                }
            }
            else if (val != null)
            {
                this.Result = action(val);
                this.HasResult = true;
            }

            return this;
        }

        public R GetResult()
        {
            if (!this.HasResult)
            {
                throw new InvalidOperationException("No case matched value: " + this.Value);
            }
            return this.Result;
        }
    }

    public static class ValuedTypeSwitchMixin
    {
/*
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
*/


    }
}