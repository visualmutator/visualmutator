namespace PiotrTrzpil.VisualMutator_VSPackage.Model.Mutations
{
    #region Usings

    using VisualMutator.Extensibility;

    #endregion

    public class MutationOperator : TreeElement
    {
        public MutationOperator(IMutationOperator mutationOperator)
        {
            Operator = mutationOperator;
            Name = "oper";
        }

        public IMutationOperator Operator { get; set; }
    }
}