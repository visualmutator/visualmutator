namespace VisualMutator.Infrastructure
{
    using System.Collections.Generic;

    public interface IHasChildren<TThis, TChild>
        where TChild :  Node,IHasParent<TThis, TChild>
        where TThis : IHasChildren<TThis, TChild>
    {
  
        IList<TChild> Children { get; }

    }
}