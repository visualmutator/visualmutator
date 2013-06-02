namespace CommonUtilityInfrastructure.CheckboxedTree
{
    using System;
    using System.Collections.Specialized;
    using System.Linq;
    using WpfUtils;

    public class NormalNode<TParent, TThis, TChildren> : NodeWithChildrenBase<TThis, TChildren>, IHasChildren<TThis, TChildren>, IHasParent<TParent, TThis>
        where TThis : NormalNode<TParent, TThis, TChildren>, IHasChildren<TThis, TChildren>,
            IHasParent<TParent, TThis>
        where TChildren : Node, IHasParent<TThis, TChildren>
        where TParent : NodeWithChildrenBase<TParent, TThis>
    {
        private readonly TParent _parent;

        protected NormalNode(TParent parent, string name)
            : base(name)
        {
            _parent = parent;
        }

        public TParent Parent
        {
            get
            {
                return _parent;
            }
        }

        protected override void UpdateParent()
        {
            _parent.UpdateIsIncludedBasedOnChildren();
        }

    }

    public class NormalNode : Node
  
    {
        private readonly NotifyingCollection<NormalNode> _children;

        public NormalNode(string name, bool hasChildren = true)
            : base(name)
        {
       
            if (hasChildren)
            {
                _children = new NotifyingCollection<NormalNode>();
                _children.CollectionChanged += (sender, args) =>
                    {
                        if (args.Action == NotifyCollectionChangedAction.Add)
                        {
                            if (args.NewItems.Cast<NormalNode>().Any(n => ReferenceEquals(this, n)))
                            {
                                throw new InvalidOperationException("Cannot add self as child.");
                            }
                            if (args.NewItems.Cast<NormalNode>().Any(n => n.Parent == null))
                            {
                                throw new InvalidOperationException("Parent is not set on one of the items.");
                            }
                            if (args.NewItems.Cast<NormalNode>().Any(n => !ReferenceEquals(n.Parent, this)))
                            {
                                throw new InvalidOperationException("One of the children has invalid parent.");
                            }
                        }
                    };
            }

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
                if (value != null)
                {
                    if (ReferenceEquals(this, value))
                    {
                        throw new InvalidOperationException("Cannot set self as parent.");
                    }
                    if (value.Children != null &&
                        !value.Children.All(child => child.Parent == null || ReferenceEquals(value, child.Parent)))
                    {
                        throw new InvalidOperationException("One of the children has invalid parent.");
                    }
                }
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

        public override string ToString()
        {
            return string.Format("{0}, IsIncluded: {1}, Children: {2}", Name, IsIncluded, Children != null ? Children.Count.ToString() : "none");
        }
    }
}