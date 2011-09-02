namespace PiotrTrzpil.VisualMutator_VSPackage.Model.Tests
{
    #region Usings

    using System;
    using System.Waf.Applications;
    using System.Windows.Input;

    using NUnit.Core;

    using PiotrTrzpil.VisualMutator_VSPackage.Infrastructure.WpfUtils;

    #endregion

    public abstract class TestTreeNode : ModelElement
    {
        private readonly TestTreeNode _parent;

        private ICommand _commandRunTest;

        private string _name;

        private string _message;

        private TestStatus _status;

        protected TestTreeNode(TestTreeNode parent)
        {
            _parent = parent;
            CommandRunTest = new BasicCommand(Comm);
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

        public bool HasResults
        {
            get
            {
                return (Status == TestStatus.Failure || Status == TestStatus.Success);
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


        public void SetStatus(TestStatus value)
        {
            if (_status != value)
            {




                _status = value;
                RaisePropertyChanged(() => Status);
            }
        }

        public TestStatus Status
        {
            set
            {
                SetStatus(value);
            }
            get
            {
                return _status;
            }
        }

        public void Comm()
        {
            Name += "!";
        }
    }
}