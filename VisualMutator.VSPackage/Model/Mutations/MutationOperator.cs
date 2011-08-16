namespace PiotrTrzpil.VisualMutator_VSPackage.Model.Mutations
{
    using VisualMutator.Extensibility;

    public class MutationOperator : TreeElement
    {
        public IMutationOperator Operator { get; set; }

        public MutationOperator(IMutationOperator mutationOperator)
        {
            Operator = mutationOperator;
            Name = "oper";
        }

        
    }
}