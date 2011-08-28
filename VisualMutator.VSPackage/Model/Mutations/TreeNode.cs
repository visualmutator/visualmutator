namespace PiotrTrzpil.VisualMutator_VSPackage.Model.Mutations
{
    #region Usings

    using System.Collections.Generic;

    using PiotrTrzpil.VisualMutator_VSPackage.Infrastructure;
    using PiotrTrzpil.VisualMutator_VSPackage.Infrastructure.WpfUtils;
    using System.Linq;
    #endregion
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
    public interface IHasChildren<TThis, TChild>
        where TChild :  Node,IHasParent<TThis, TChild>
        where TThis : IHasChildren<TThis, TChild>
    {
  
        IList<TChild> Children { get; }

    }
    public interface IHasParent<out TParent, TThis>
        where TParent : IHasChildren<TParent, TThis>
        where TThis : Node, IHasParent<TParent, TThis>

    {
      
        TParent Parent { get; }

    }
    public abstract class Node : ModelElement
    {
        protected bool? _isIncluded;
        public bool? IsIncluded
        {
            get
            {
                return _isIncluded;
            }
            set
            {
                SetIsIncluded(value, true, true);
            }
        }

        private string _name;

        protected Node(string name)
        {
            _name = name;
        }

        public string Name
        {
            set
            {
                if (_name != value)
                {
                    _name = value;
                    RaisePropertyChanged(() => Name);
                }
            }
            get
            {
                return _name;
            }
        }
     //  public abstract void SetIsIncluded(bool? value, bool updateChildren, bool updateParent);
        public void SetIsIncluded(bool? value, bool updateChildren, bool updateParent)
        {
            if (value != _isIncluded)
            {
                _isIncluded = value;

                if (updateChildren && _isIncluded != null)
                {
                    UpdateChildren();
                    
                }

                if (updateParent)
                {
                    UpdateParent();
                }

                RaisePropertyChanged(() => IsIncluded);
            }
        }
        protected abstract void UpdateParent();
        protected abstract void UpdateChildren();
        // /  protected abstract void SetIsIncluded(bool? value);
    }
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
            Children.Each(c => c.SetIsIncluded(_isIncluded, true, false));
        }
        
        internal virtual void UpdateIsIncludedBasedOnChildren()
        {
      
            bool? state = Children.Select(n => n.IsIncluded)
                .Aggregate((one, two) => one != null && one == two ? one : null);


            SetIsIncluded(state, updateChildren: false, updateParent:true);
        }
    }
  

    public class RecursiveNode : Node
  
    {
        private BetterObservableCollection<RecursiveNode> _children;

        public RecursiveNode(RecursiveNode parent, string name)
            : base(name)
        {
            _parent = parent;
            _children = new BetterObservableCollection<RecursiveNode>();
        }

        private RecursiveNode _parent;

      
        public RecursiveNode Parent
        {
            get
            {
                return this._parent;
            }
        }

        public IList<RecursiveNode> Children
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
            Children.Each(c => 
                c.SetIsIncluded(_isIncluded, updateChildren: true, updateParent: false));
        }

        private void UpdateIsIncludedBasedOnChildren()
        {

            bool? state = Children.Select(n => n.IsIncluded)
                .Aggregate((one, two) => one != null && one == two ? one : null);


            SetIsIncluded(state, updateChildren: false, updateParent: true);
        }
    }






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
                    Children.Each(c => c.SetIsIncluded(_isIncluded, true, false));
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
            @this.Children.Each(c => c.SetIsIncluded(value, true, false));
        }
    }

}