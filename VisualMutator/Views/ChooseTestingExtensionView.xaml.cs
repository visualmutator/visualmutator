namespace VisualMutator.Views
{
    #region

    using System.Windows;
    using UsefulTools.Core;
    using UsefulTools.Wpf;

    #endregion

    public interface IChooseTestingExtensionView : IWindow
    {

    }
    public partial class ChooseTestingExtensionView : Window, IChooseTestingExtensionView
    {
        private readonly IOwnerWindowProvider _windowProvider;

        public ChooseTestingExtensionView(IOwnerWindowProvider windowProvider)
        {
            _windowProvider = windowProvider;
            InitializeComponent();
        }

        public bool? ShowDialog(IWindow owner)
        {
            Owner = (Window)owner;
            return ShowDialog();
        }

        public bool? SetDefaultOwnerAndShowDialog()
        {
            _windowProvider.SetOwnerFor(this);
            return ShowDialog();
        }

    
    }
}
