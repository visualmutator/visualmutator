namespace CommonUtilityInfrastructure.CheckboxedTree
{
    public interface IHasParent<out TParent, TThis>
        where TParent : IHasChildren<TParent, TThis>
        where TThis : Node, IHasParent<TParent, TThis>

    {
      
        TParent Parent { get; }

    }
}