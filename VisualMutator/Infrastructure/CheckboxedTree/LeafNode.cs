namespace VisualMutator.Infrastructure
{
    public class LeafNode<TParent, TThis> :Node, IHasParent<TParent, TThis>
        where TThis : LeafNode<TParent, TThis>, IHasParent<TParent, TThis>
        where TParent : NodeWithChildrenBase<TParent, TThis>
    {
        private TParent _parent;

        protected LeafNode(TParent parent, string name):base(name)
        {
            this._parent = parent;
        }

        public TParent Parent
        {
            get
            {
                return this._parent;
            }
        }
        protected override void UpdateParent()
        {
            _parent.UpdateIsIncludedBasedOnChildren();
        }

        protected override void UpdateChildren()
        {
            
        }

        public bool IsLeafIncluded
        {
            get
            {
                return (bool)IsIncluded;
            }
            set
            {
                IsIncluded = value;
            }
        }
    }
}