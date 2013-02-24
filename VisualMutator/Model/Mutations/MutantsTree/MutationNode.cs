namespace VisualMutator.Model.Mutations.MutantsTree
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
  
        protected virtual void SetState(MutantResultState value, bool updateChildren, bool updateParent)
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

     
        private void UpdateStateBasedOnChildren()
        {
            var children = Children.Cast<MutationNode>().ToList();



            MutantResultState state = Switch.Into<MutantResultState>().AsCascadingCollectiveOf(children.Select(n => n.State))
                .CaseAny(MutantResultState.Error, MutantResultState.Error)
                .CaseAny(MutantResultState.Tested, MutantResultState.Tested)
                .CaseAny(MutantResultState.Live, MutantResultState.Live)
                .CaseAny(MutantResultState.Killed, MutantResultState.Killed)
                .CaseAll(MutantResultState.Untested, MutantResultState.Untested);

/*

            MutantResultState state;
            if (children.Any(n => n.State == MutantResultState.Error))
            {
                state = MutantResultState.Error;
            }
            else if (children.Any(n => n.State == MutantResultState.Tested))
            {
                state = MutantResultState.Tested;
            }
            else if (children.Any(n => n.State == MutantResultState.Live))
            {
                state = MutantResultState.Live;
            }
            else if (children.Any(n => n.State == MutantResultState.Killed))
            {
                state = MutantResultState.Killed;
            }
            else if (children.All(n => n.State == MutantResultState.Untested))
            {
                state = MutantResultState.Untested;
            }
            else
            {
                throw new InvalidOperationException("Unknown state");
            }*/
            SetState(state, updateChildren: false, updateParent: true);
            
  

            
           
        }

    
    }
}