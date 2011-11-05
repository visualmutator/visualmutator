namespace CommonUtilityInfrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

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

        public bool HasResult
        {
            get;
            set;
        }
    }

    public static class SwitchMixin
    {
        public static void Do<T>(this Switch<T> @switch)
        {
            if (!@switch.HasResult)
            {
                throw new InvalidOperationException("No case matched value: " + @switch.Value);
            }
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
    }
}