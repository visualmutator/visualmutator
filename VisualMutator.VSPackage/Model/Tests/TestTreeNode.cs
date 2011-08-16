namespace PiotrTrzpil.VisualMutator_VSPackage.Model.Tests
{
    using System;
    using System.Waf.Applications;
    using System.Windows.Input;

    using NUnit.Core;

    public abstract class TestTreeNode : PiotrTrzpil.VisualMutator_VSPackage.Infrastructure.WpfUtils.ModelElement
    {
        protected TestTreeNode()
        {
            CommandRunTest = new DelegateCommand(Comm);
        }
        public ITest Test
        {
            get;
            set;
        }

        private string _name;

        public string Name
        {
            set
            {
                if (_name != value)
                {
                    _name = value;
                    this.RaisePropertyChanged(() => Name);
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

        private TestResult _result;


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

        

        public void Comm()
        {
            Name += "!";
        }
        private ICommand _commandRunTest;

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
                    this.RaisePropertyChanged(() => CommandRunTest);
                }
            }
        }

        private TestStatus _status;

        public TestStatus Status
        {
            set
            {
                if (_status != value)
                {
                    _status = value;
                    this.RaisePropertyChanged(() => Status);
                }
            }
            get
            {
                return _status;
            }
        }
      
    }
}