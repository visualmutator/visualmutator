namespace VisualMutator.Model.Mutations.MutantsTree
{
    #region

    using System.Collections.Generic;
    using Infrastructure;
    using UsefulTools.CheckboxedTree;

    #endregion

    public abstract class MutationNode : StateNode<MutantResultState>, IExpandableNode
    {
        private bool _isExpanded;

        protected MutationNode(string name, bool hasChildren)
            : base( name, hasChildren, 
            propagationDown: new List<MutantResultState>()
            {
                MutantResultState.Untested
            }, propagationUp: new List<MutantResultState>()
            {
                MutantResultState.Error,
                MutantResultState.Tested,
                MutantResultState.Creating,
                MutantResultState.Writing,
                MutantResultState.Live,
                MutantResultState.Killed
            })
        {
     
        }
        public bool IsExpanded
        {
            get
            {
                return _isExpanded;
            }
            set
            {
                SetAndRise(ref _isExpanded, value, () => _isExpanded);
            }
        }
        
    
    }
}