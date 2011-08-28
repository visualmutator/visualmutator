namespace PiotrTrzpil.VisualMutator_VSPackage.Model.Mutations
{
    #region Usings

    using VisualMutator.Extensibility;

    #endregion

    public class OperatorNode : LeafNode<PackageNode, OperatorNode>
    {
        public OperatorNode(PackageNode parent, IMutationOperator mutationOperator)
            : base(parent, mutationOperator.Name)
        {
            Operator = mutationOperator;
         
        }

        public IMutationOperator Operator { get; set; }
    }
}