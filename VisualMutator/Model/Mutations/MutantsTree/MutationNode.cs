namespace VisualMutator.Model.Mutations.MutantsTree
{
    #region Usings

    using System.Collections.Generic;
    using CommonUtilityInfrastructure;
    using CommonUtilityInfrastructure.CheckboxedTree;

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