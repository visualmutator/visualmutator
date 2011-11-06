namespace VisualMutator.Model.Mutations.Structure
{
    #region Usings

    using System;
    using System.Linq;

    using CommonUtilityInfrastructure;

    using VisualMutator.Infrastructure.CheckboxedTree;

    #endregion
    public class MutationRootNode : MutationNode
    {
        public MutationRootNode()
            : base( "", true)
        {

        }
    }
    public abstract class MutationNode : NormalNode
    {

        private MutantResultState _state;

        protected MutationNode(string name, bool hasChildren)
            : base( name, hasChildren)
        {
     
        }

     
        public  MutantResultState State
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
        public bool HasResults
        {
            get
            {
                return (State == MutantResultState.Tested || State == MutantResultState.Live
                    || State == MutantResultState.Killed);
            }
        }

        protected virtual void SetState(MutantResultState value, bool updateChildren, bool updateParent)
        {
            if (_state != value)
            {
                _state = value;

                if (updateChildren && Children != null)
                {

                    if (value != MutantResultState.Untested)
                    {
                        throw new InvalidOperationException("Tried to set invalid state: " + value);
                    }

                    foreach (var node in Children.Cast<MutationNode>())
                    {
                        node.SetState(value, updateChildren: true, updateParent: false);
                    }
  
                }
                
                if (updateParent && Parent != null)
                {
                    if (!value.IsIn(MutantResultState.Tested, MutantResultState.Live,
                        MutantResultState.Killed, MutantResultState.Error))
                    {
                        throw new InvalidOperationException("Tried to set invalid state: " + value);
                    }

                    ((MutationNode)Parent).UpdateStateBasedOnChildren();
                }
                RaisePropertyChanged(() => State);
            }
        }

     
        private void UpdateStateBasedOnChildren()
        {
            var children = Children.Cast<MutationNode>().ToList();

            if(children.All(_ => _.HasResults))
            {
                MutantResultState state;
                if (children.Any(n => n.State == MutantResultState.Tested))
                {
                    state = MutantResultState.Tested;
                }
                else if (children.Any(n => n.State == MutantResultState.Live))
                {
                    state = MutantResultState.Live;
                }
                else 
                {
                    state = MutantResultState.Killed;
                }
                SetState(state, updateChildren: false, updateParent: true);
            }
  

            
           
        }

    
    }
}