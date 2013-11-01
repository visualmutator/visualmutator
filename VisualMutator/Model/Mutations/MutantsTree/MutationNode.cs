namespace VisualMutator.Model.Mutations.MutantsTree
{
    #region

    using System.Collections.Generic;
    using UsefulTools.CheckboxedTree;

    #endregion

    public abstract class MutationNode : StateNode<MutantResultState>
    {

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
                MutantResultState.Live,
                MutantResultState.Killed
            })
        {
     
        }


    
    }
}