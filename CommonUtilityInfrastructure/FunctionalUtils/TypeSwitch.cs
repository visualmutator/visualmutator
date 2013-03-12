namespace CommonUtilityInfrastructure.FunctionalUtils
{
    using System;

    public class TypeSwitch
    {
        public TypeSwitch(object o)
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

    }
    public static class TypeSwitchMixin
    {

        public static TypeSwitch Case<TThis>(this TypeSwitch @switch, Action<TThis> action) where TThis : class
        {

            TThis val = @switch.Value as TThis;
            if (val != null)
            {
                @switch.HasResult = true;
                action(val);
            }
            return @switch;
        }
        public static void Do(this TypeSwitch @switch)
        {
            if (!@switch.HasResult)
            {
                throw new InvalidOperationException("No case matched value: " + @switch.Value);
            }
        }
    }
}