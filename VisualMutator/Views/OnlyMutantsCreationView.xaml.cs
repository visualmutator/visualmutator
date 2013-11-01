namespace VisualMutator.Views
{
    #region

    using System.Windows;
    using UsefulTools.Core;
    using UsefulTools.Wpf;

    #endregion

    public interface IOnlyMutantsCreationView : IWindow
    {

    }
    public partial class OnlyMutantsCreationView : Window, IOnlyMutantsCreationView
    {
        private readonly IOwnerWindowProvider _windowProvider;

        public OnlyMutantsCreationView(IOwnerWindowProvider windowProvider)
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
