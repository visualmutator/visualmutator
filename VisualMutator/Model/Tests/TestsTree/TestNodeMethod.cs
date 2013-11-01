namespace VisualMutator.Model.Tests.TestsTree
{
    #region

    using System.Diagnostics;
    using System.Windows;
    using UsefulTools.Wpf;

    #endregion

    public class TestNodeMethod : TestTreeNode
    {
        public TestNodeMethod(TestNodeClass parent, string name)
            : base(parent, name, false)
        {
            CommandShowMessage = new SmartCommand(ShowMessage);
        }
        public TestNodeClass ContainingClass
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

        private SmartCommand _commandShowMessage;

        public SmartCommand CommandShowMessage
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