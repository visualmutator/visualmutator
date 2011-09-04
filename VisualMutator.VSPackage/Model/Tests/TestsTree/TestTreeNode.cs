namespace PiotrTrzpil.VisualMutator_VSPackage.Model.Tests
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Input;

    using PiotrTrzpil.VisualMutator_VSPackage.Infrastructure.WpfUtils;
    using PiotrTrzpil.VisualMutator_VSPackage.Model.Mutations;

    #endregion

    public abstract class TestTreeNode : RecursiveNode
    {
        private ICommand _commandRunTest;

        private string _message;

        private TestNodeState _state;

        protected TestTreeNode(TestTreeNode parent, string name, bool hasChildren)
            : base(parent, name, hasChildren)
        {
            CommandRunTest = new BasicCommand(Comm);
        }

        public bool HasResults
        {
            get
            {
                return (State == TestNodeState.Failure || State == TestNodeState.Success
                    || State == TestNodeState.Inconclusive);
            }
        }

        public string Message
        {
            set
            {
                _message = value;
            }
            get
            {
                if (!HasResults)
                {
                    throw new InvalidOperationException("No results");
                }
                if (_message == null)
                {
                    throw new ArgumentException("Result not set");
                }
                return _message;
            }
        }

        public ICommand CommandRunTest
        {
            get
            {
                return _commandRunTest;
            }
            set
            {
                if (_commandRunTest != value)
                {
                    _commandRunTest = value;
                    RaisePropertyChanged(() => CommandRunTest);
                }
            }
        }

        public TestNodeState State
        {
            set
            {
                SetStatus(value, true, true);
            }
            get
            {
                return _state;
            }
        }

        public void SetStatus(TestNodeState value, bool updateChildren, bool updateParent)
        {
            if (_state != value)
            {
                _state = value;

                if (updateChildren && Children != null)
                {

                    if (!(value == TestNodeState.Inactive || value == TestNodeState.Running))
                    {
                        throw new InvalidOperationException("Tried to set invalid state: " + value);
                    }

                    Children.Cast<TestTreeNode>()
                        .Each(c => c.SetStatus(value, updateChildren: true, updateParent: false));
                    
                }
                
                if (updateParent && Parent != null)
                {
                    if (!(value == TestNodeState.Success || value == TestNodeState.Failure 
                        || value == TestNodeState.Inconclusive))
                    {
                        throw new InvalidOperationException("Tried to set invalid state: " + value);
                    }

                    ((TestTreeNode)Parent).UpdateStateBasedOnChildren();
                }
                RaisePropertyChanged(() => State);
            }
        }

     
        private void UpdateStateBasedOnChildren()
        {
           // var values = new List<TestNodeState> { TestNodeState.Failure, TestNodeState.Inconclusive, TestNodeState.Success };

          //  TestNodeState state = Children.Cast<TestTreeNode>().Select(n => n.State)
          //      .Aggregate((one, two) =>  values.IndexOf(one) != -1 &&  values.IndexOf(one) < values.IndexOf(two) ? one : two);
            var children = Children.Cast<TestTreeNode>();

            if(children.All(_ => _.HasResults))
            {
                TestNodeState state;
                if (children.Any(n => n.State == TestNodeState.Failure))
                {
                    state = TestNodeState.Failure;
                }
                else if (children.Any(n => n.State == TestNodeState.Inconclusive))
                {
                    state = TestNodeState.Inconclusive;
                }
                else 
                {
                    state = TestNodeState.Success;
                }
                SetStatus(state, updateChildren: false, updateParent: true);
            }
  

            
           
        }

        public void Comm()
        {
            Name += "!";
        }
    }
}