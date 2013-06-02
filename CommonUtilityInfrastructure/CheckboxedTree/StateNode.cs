namespace CommonUtilityInfrastructure.CheckboxedTree
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using CommonUtilityInfrastructure.FunctionalUtils;

    public abstract class StateNode<TState> : NormalNode
    {
        private readonly IList<TState> _propagationDown;
        private readonly IList<TState> _propagationUp;
        private TState _state;

        protected StateNode(string name, bool hasChildren,
                            IList<TState> propagationDown, IList<TState> propagationUp)
            : base( name, hasChildren)
        {
            _propagationDown = propagationDown;
            _propagationUp = propagationUp;
        }

        public TState State
        {
            set
            {
                SetState(value, true, true);
            }
            get
            {
                return _state;
            }
        }

        protected virtual void SetState(TState value, bool updateChildren, bool updateParent)
        {

            _state = value;

            if (updateChildren && Children != null)
            {

                if (!_propagationDown.Contains(value))
                {
                    throw new InvalidOperationException("Tried to set invalid state: " + value);
                }

                foreach (var node in Children.Cast<StateNode<TState>>())
                {
                    node.SetState(value, updateChildren: true, updateParent: false);
                }

            }

            if (updateParent && Parent != null)
            {
                if (!_propagationUp.Contains(value))
                {
                    throw new InvalidOperationException("Tried to set invalid state: " + value);
                }

                ((StateNode<TState>)Parent).UpdateStateBasedOnChildren();
            }
            RaisePropertyChanged(() => State);

        }


        private void UpdateStateBasedOnChildren()
        {
            var children = Children.Cast<StateNode<TState>>().ToList();

            var @switch = Switch.Into<TState>().AsCascadingCollectiveOf(children.Select(n => n.State));

            foreach (var state1 in _propagationUp)
            {
                @switch = @switch.CaseAny(state1, state1);
            }
            foreach (var state1 in _propagationDown)
            {
                @switch = @switch.CaseAll(state1, state1);
            }
            TState state = @switch.GetValue();

        
            SetState(state, updateChildren: false, updateParent: true);





        }

    }
}