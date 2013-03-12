namespace CommonUtilityInfrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using FunctionalUtils;


    public enum ProgressMode
    {
        Before,
        After
    }
    public class ProgressCounter
    {
        private readonly Action<int> _progress;

        private int _all = -1;

        private int _tick;

        private ProgressMode _mode;

        private bool _inactive;
        private ProgressCounter _child;
     

        public ProgressCounter(Action<int> progress )
        {
            _progress = progress;
        }

        public void Initialize(int all, ProgressMode mode = ProgressMode.After)
        {
            _all = all;
            _mode = mode;
            _tick = 0;
        }

        public ProgressCounter CreateSubprogress()
        {
            _child = new ProgressCounter(FromChild);
            return _child;

        }
        private void FromChild(int value)
        {
            if (!_inactive)
            {
                Throw.If(_all == -1);

                int current = (int) (_tick.AsPercentageOf(_all) + value * (double)_all/100);

                _progress(current);

            }
        }

    
        public void Progress()
        {
            if (!_inactive)
            {
                Throw.If(_all == -1);

                if (_mode == ProgressMode.After)
                {
                    _tick++;
                }

                int current = _tick.AsPercentageOf(_all);

                if (_mode == ProgressMode.Before)
                {
                    _tick++;
                }

                _progress(current);
             
            }
           

        }

        public static ProgressCounter Inactive()
        {
            return new ProgressCounter((c) => { }) { _inactive = true };
          
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

        public override string ToString()
        {
            return string.Format("{0}%", _tick.AsPercentageOf(_all));
        }
    }
}