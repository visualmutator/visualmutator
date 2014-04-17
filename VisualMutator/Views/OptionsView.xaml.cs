namespace VisualMutator.Views
{
    #region

    using System.Windows;
    using UsefulTools.Core;
    using UsefulTools.Wpf;

    #endregion

    public interface IOptionsView : IWindow
    {
      

    }

    public partial class OptionsView : Window, IOptionsView
    {
        private IOwnerWindowProvider _windowProvider;

        public OptionsView(IOwnerWindowProvider windowProvider)
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
