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
        public static Switch<T> Case<T>(this Switch<T> @switch, T caseValue1, T caseValue2, Action action)
        {
            if (@switch.Value.Equals(caseValue1) || @switch.Value.Equals(caseValue2))
            {
                action();
                @switch.HasResult = true;
            }
            return @switch;
        }
        public static Switch<T> Case<T>(this Switch<T> @switch, T caseValue1, T caseValue2, T caseValue3, Action action)
        {
            if (@switch.Value.Equals(caseValue1) 
                || @switch.Value.Equals(caseValue2)
                || @switch.Value.Equals(caseValue3))
            {
                action();
                @switch.HasResult = true;
            }
            return @switch;
        }
        public static Switch<T> Case<T>(this Switch<T> @switch, IEnumerable<T> caseValues, Action action)
        {
            if (caseValues.Contains(@switch.Value))
            {
                action();
                @switch.HasResult = true;
            }
            return @switch;
        }
    }
}