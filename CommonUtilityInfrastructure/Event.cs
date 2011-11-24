namespace CommonUtilityInfrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public static class Event
    {

        public static void RaiseIfNotNull(Action action )
        {
            if (action != null)
            {
                action();
            }
        }
        public static void RaiseIfNotNull<T>(Action<T> action, T p1)
        {
            if (action != null)
            {
                action(p1);
            }
        }
        public static void RaiseIfNotNull<T1,T2>(Action<T1,T2> action, T1 p1, T2 p2)
        {
            if (action != null)
            {
                action(p1,p2);
            }
        }
        public static void RaiseIfNotNull<T1, T2, T3>(Action<T1, T2, T3> action, T1 p1, T2 p2, T3 p3)
        {
            if (action != null)
            {
                action(p1, p2, p3);
            }
        }
    }
}