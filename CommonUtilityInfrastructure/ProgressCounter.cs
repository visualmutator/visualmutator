namespace CommonUtilityInfrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;


    public enum ProgressMode
    {
        Before,
        After
    }
    public class ProgressCounter
    {
        private readonly Action<int> _progress;

        private int _all = -1;

        private int tick;

        private ProgressMode _mode;

        private bool _inactive;

        public ProgressCounter(Action<int> progress )
        {
            _progress = progress;

            //     _all = all;
        }

        public void Initialize(int all, ProgressMode mode = ProgressMode.Before)
        {
            _all = all;
            _mode = mode;
            tick = 0;
        }

        public void Progress()
        {
            if (!_inactive)
            {
                Throw.If(_all == -1);

                if (_mode == ProgressMode.Before)
                {
                    tick++;
                }

                int current = tick.AsPercentageOf(_all);

                if (_mode == ProgressMode.After)
                {
                    tick++;
                }


                _progress(current);
            }
           

        }

        public static ProgressCounter Inactive()
        {
            var p = new ProgressCounter((c)=> { });
            p.InitializeInactive();
            return p;
        }

        private void InitializeInactive()
        {
            _inactive = true;
        }

        public static ProgressCounter Invoking(Action<int> action)
        {
            return new ProgressCounter(action);
        }

        public static ProgressCounter Invoking<T>(Action<T, int> action, T arg1)
        {
            return new ProgressCounter(value => action(arg1, value));
        }
        public static ProgressCounter Invoking<T1, T2>(Action<T1, T2, int> action, T1 arg1, T2 arg2)
        {
            return new ProgressCounter(value => action(arg1, arg2, value));
        }

    }
}