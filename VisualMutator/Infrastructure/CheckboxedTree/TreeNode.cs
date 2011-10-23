namespace VisualMutator.Infrastructure.CheckboxedTree
{
    #region Usings

    using System.Linq;

    using CommonUtilityInfrastructure.WpfUtils;

    #endregion

    public class RecursiveNode<TThis> : NormalNode<TThis, TThis, TThis>
        where TThis : NormalNode<TThis, TThis, TThis>, IHasChildren<TThis, TThis>, IHasParent<TThis, TThis>
    {
        protected RecursiveNode(TThis parent, string name)
            : base(parent, name)
        {
          
        }

    }

    public class RootNode<TThis, TChildren> : NodeWithChildrenBase<TThis, TChildren>
        where TThis : NodeWithChildrenBase<TThis, TChildren>
        where TChildren : Node, IHasParent<TThis, TChildren>
    {
        protected RootNode(string name)
            : base(name)
        {
        }

        protected override void UpdateParent()
        {
            
        }
     
    }


    public class FakeRootNode<TThis, TChildren> : RootNode<TThis, TChildren>
        where TThis : NodeWithChildrenBase<TThis, TChildren>
        where TChildren : Node, IHasParent<TThis, TChildren>
    {
        protected FakeRootNode(string name)
            : base(name)
        {
        }

        internal override void UpdateIsIncludedBasedOnChildren()
        {

        }
    }
    
    public class TreeNode : ModelElement 
    {
        

        private bool? _isIncluded;

        private string _name;

        private TreeNode _parent;

        protected TreeNode(TreeNode parent, string name, bool? isIncluded)
        {
            _parent = parent;
            _name = name;
            _isIncluded = isIncluded;
        }

        public TreeNode Parent
        {
            get
            {
                return _parent;
            }
        }

        public string Name
        {
            set
            {
                _name = value;
                RaisePropertyChanged(() => Name);
            }
            get
            {
                return _name;
            }
        }
        private BetterObservableCollection<TreeNode> _children;

        protected BetterObservableCollection<TreeNode> Children
        {
            set
            {
                if (_children != value)
                {
                    _children = value;
                    RaisePropertyChanged(() => Children);
                }
            }
            get
            {
                return _children;
            }
        }
      
        public bool? IsIncluded
        {
            get
            {
                return _isIncluded;
            }
            set
            {
                SetIsIncluded(value, updateChildren: true, updateParent: true);
            }
        }


        private void SetIsIncluded(bool? value, bool updateChildren, bool updateParent)
        {
            if (value != _isIncluded)
            {
                _isIncluded = value;

                if (updateChildren && _isIncluded != null)
                {
                    foreach (var child in Children)
                    {
                        child.SetIsIncluded(_isIncluded, true, false);
                    }
        
                }

                if (updateParent && _parent != null)
                {
                    _parent.UpdateIsIncludedBasedOnChildren();
                }

                RaisePropertyChanged(() => IsIncluded);
            } 
        }

        private void UpdateIsIncludedBasedOnChildren()
        {
            bool? state = Children.Select(n => n.IsIncluded)
                .Aggregate((one, two) => one != null && one == two ? one : null);


            SetIsIncluded(state, updateChildren: false, updateParent:true);
        }
    }


    public static class Mixin
    {
        public static void UpdateChildrenInternal<TThis, TChildren>(
            this IHasChildren<TThis, TChildren> @this, bool? value) 
            where TThis : IHasChildren<TThis, TChildren> 
            where TChildren : Node, IHasParent<TThis, TChildren>
        {
    
            foreach (var child in @this.Children)
            {
                child.SetIsIncluded(value, true, false);
            }
        }
    }

}