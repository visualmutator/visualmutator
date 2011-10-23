namespace VisualMutator.Infrastructure
{
    using System.Collections.Generic;
    using System.Linq;

    using CommonUtilityInfrastructure.WpfUtils;

    public abstract class NodeWithChildrenBase<TThis, TChild> : Node,
                                                                IHasChildren<TThis, TChild>
        where TThis : NodeWithChildrenBase<TThis, TChild>,
            IHasChildren<TThis, TChild>
        where TChild : Node, IHasParent<TThis, TChild>
    {
        private BetterObservableCollection<TChild> _children;

        protected NodeWithChildrenBase(string name)
            : base(name)
        {
            this._children = new BetterObservableCollection<TChild>();
        }

        public IList<TChild> Children
        {
            get
            {
                return this._children;
            }
        }
        protected override void UpdateChildren()
        {
            foreach (var child in Children)
            {
                child.SetIsIncluded(_isIncluded, true, false);
            }
          
        }
        
        internal virtual void UpdateIsIncludedBasedOnChildren()
        {
      
            bool? state = Children.Select(n => n.IsIncluded)
                .Aggregate((one, two) => one != null && one == two ? one : null);


            SetIsIncluded(state, updateChildren: false, updateParent:true);
        }
    }
}