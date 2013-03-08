namespace VisualMutator.Model.Tests.TestsTree
{
    using System.Diagnostics;
    using System.Windows;

    using CommonUtilityInfrastructure.WpfUtils;

    public class TestNodeMethod : TestTreeNode
    {
        public TestNodeMethod(TestNodeClass parent, string name)
            : base(parent, name, false)
        {
            CommandShowMessage = new BasicCommand(ShowMessage);
        }
        public TestNodeClass ParentClass
        {
            get
            {
                return (TestNodeClass)Parent;
            }
        }
        private string _message;

        public string Message
        {
            set
            {
                if (_message != value)
                {
                    _message = value;
                    RaisePropertyChanged(() => Message);
                }
            }
            get
            {
                return _message;
            }
        }

        private BasicCommand _commandShowMessage;

        public BasicCommand CommandShowMessage
        {
            get
            {
                return _commandShowMessage;
            }
            set
            {
                SetAndRise(ref _commandShowMessage, value, () => CommandShowMessage);
            }
        }

        public TestId TestId { get; set; }

        public void ShowMessage()
        {
            Debug.Assert(!string.IsNullOrEmpty(Message));
            MessageBox.Show(Message);
        }
    }
}