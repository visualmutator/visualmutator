namespace VisualMutator.Infrastructure.CheckboxedTree
{
    using System.Linq;

    using CommonUtilityInfrastructure.WpfUtils;

    public class NormalNode<TParent, TThis, TChildren> : NodeWithChildrenBase<TThis, TChildren>, IHasChildren<TThis, TChildren>, IHasParent<TParent, TThis>
        where TThis : NormalNode<TParent, TThis, TChildren>, IHasChildren<TThis, TChildren>,
            IHasParent<TParent, TThis>
        where TChildren : Node, IHasParent<TThis, TChildren>
        where TParent : NodeWithChildrenBase<TParent, TThis>
    {
        private TParent _parent;

        protected NormalNode(TParent parent, string name)
            : base(name)
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

    }

    public class NormalNode : Node
  
    {
        private NotifyingCollection<NormalNode> _children;

        protected NormalNode(string name, bool hasChildren)
            : base(name)
        {
       
            if (hasChildren)
                _children = new NotifyingCollection<NormalNode>();

        }

        private NormalNode _parent;

    
        public NormalNode Parent
        {
            get
            {
                return _parent;
            }
            set
            {
                _parent = value;
            }
        }

        public NotifyingCollection<NormalNode> Children
        {
            get
            {
                return _children;
            }
        }

        protected override void UpdateParent()
        {
            if (_parent  != null)
            {
                _parent.UpdateIsIncludedBasedOnChildren();
            }
        }

        protected override void UpdateChildren()
        {
            if(Children != null)
                foreach (var child in Children)
                {
                    child.SetIsIncluded(_isIncluded, updateChildren: true, updateParent: false);
                }
    
        }

        private void UpdateIsIncludedBasedOnChildren()
        {

            bool? state = Children.Select(n => n.IsIncluded)
                .Aggregate((one, two) => one != null && one == two ? one : null);


            SetIsIncluded(state, updateChildren: false, updateParent: true);
        }
    }
}