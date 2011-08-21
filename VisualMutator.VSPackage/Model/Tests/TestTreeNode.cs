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
        private ICommand _commandRunTest;

        private string _name;

        private TestResult _result;

        private TestStatus _status;

        protected TestTreeNode()
        {
            CommandRunTest = new DelegateCommand(Comm);
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

        public TestResult Result
        {
            set
            {
                _result = value;
            }
            get
            {
                if (!HasResults)
                {
                    throw new InvalidOperationException("No results");
                }
                if (_result == null)
                {
                    throw new ArgumentException("Result not set");
                }
                return _result;
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

        public TestStatus Status
        {
            set
            {
                if (_status != value)
                {
                    _status = value;
                    RaisePropertyChanged(() => Status);
                }
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