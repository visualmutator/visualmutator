namespace PiotrTrzpil.VisualMutator_VSPackage.Infrastructure.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Waf.Applications;
    using System.Windows.Input;

    using NUnit.Core;

    using VisualMutator.Domain;

    public abstract class TestTreeNode : ExtModel
    {

        private string _name;

        public string Name
        {
            set
            {
                if (_name != value)
                {
                    _name = value;
                    this.RaisePropertyChangedExt(() => Name);
                }
            }
            get
            {
                return _name;
            }
        }


        public ITest Test { get; set; }


        protected TestTreeNode()
        {
            CommandRunTest = new DelegateCommand(Comm);
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
                    this.RaisePropertyChangedExt(() => CommandRunTest);
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
                    this.RaisePropertyChangedExt(() => Status);
                }
            }
            get
            {
                return _status;
            }
        }
      
    }
}