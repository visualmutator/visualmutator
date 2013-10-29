namespace CommonUtilityInfrastructure.CheckboxedTree
{
    using System;
    using System.Collections.Specialized;
    using System.Linq;
    using WpfUtils;


    public class CheckedNode : CheckedNodeBase
    {
        private readonly NotifyingCollection<CheckedNode> _children;

        public CheckedNode(string name, bool hasChildren = true)
            : base(name)
        {
            if (hasChildren)
            {
                _children = new NotifyingCollection<CheckedNode>();
                _children.CollectionChanged += (sender, args) =>
                    {
                        if (args.Action == NotifyCollectionChangedAction.Add)
                        {
                            if (args.NewItems.Cast<CheckedNode>().Any(n => ReferenceEquals(this, n)))
                            {
                                throw new InvalidOperationException("Cannot add self as child.");
                            }
                            if (args.NewItems.Cast<CheckedNode>().Any(n => n.Parent == null))
                            {
                                throw new InvalidOperationException(@"Property ""Parent"" is not set on one of the items.");
                            }
                            if (args.NewItems.Cast<CheckedNode>().Any(n => !ReferenceEquals(n.Parent, this)))
                            {
                                throw new InvalidOperationException("One of the children has invalid parent.");
                            }
                        }
                    };
            }

        }

        private CheckedNode _parent;

    
        public CheckedNode Parent
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

        public NotifyingCollection<CheckedNode> Children
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
            {
                foreach (var child in Children)
                {
                    child.SetIsIncluded(_isIncluded, updateChildren: true, updateParent: false);
                }
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
            return string.Format("{0}, IsIncluded: {1}, Children: {2}", 
                Name, IsIncluded, Children != null ? Children.Count.ToString() : "none");
        }
    }
}