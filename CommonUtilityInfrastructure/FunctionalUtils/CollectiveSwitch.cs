namespace CommonUtilityInfrastructure.FunctionalUtils
{
    using System;
    using System.Collections.Generic;

    public class CollectiveSwitch<T, R>
    {
        private readonly IEnumerable<T> _values;
        private List<Case> _cases;

        private class Case
        {
            public Case(T caseValue, Func<R> action)
            {
                this.caseValue = caseValue;
                this.action = action;
            }

            public T caseValue
            {
                get;
                set;
            }

            public Func<R> action
            {
                get;
                set;
            }

            public bool HasResult { get; set; }
        }
        public CollectiveSwitch(IEnumerable<T> values)
        {
            _values = values;
            _cases = new List<Case>();
        }

   
        public CollectiveSwitch<T, R> CaseAny(T caseValue, Func<R> action)
        {
            _cases.Add(new Case( caseValue, action ));
            for (int i = 0; i < _cases.Count; i++)
            {
                for (int j = 0; j < _cases.Count; j++)
                {
                    if (i != j && _cases[i].caseValue.Equals(_cases[j].caseValue))
                    {
                        throw new InvalidOperationException("Case values are not distict.");
                    }
                }
            }
            return this;
        }


        public CollectiveSwitch<T, R> CaseAny(T caseValue, R result)
        {
            return CaseAny(caseValue, () => result);
        }
        public R CaseAll(T caseAllValue, R allResult)
        {
            bool hasAllResult = true;
        
            foreach (var value in _values)
            {
                foreach (var @case in _cases)
                {
                    if (value.Equals(@case.caseValue))
                    {
                        @case.HasResult = true;
                    }
                }
                if (!value.Equals(caseAllValue))
                {
                    hasAllResult = false;
                }
            }

             foreach (var @case in _cases)
            {
                if (@case.HasResult)
                {
                    return @case.action();
                }
            }
            if (hasAllResult)
            {
                return allResult;
            }
            else
            {
                throw new InvalidOperationException("No match found.");
            }
        }


    }
   
}