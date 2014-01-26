namespace VisualMutator.Views
{
    #region

    using System.Windows;
    using UsefulTools.Core;
    using UsefulTools.Wpf;

    #endregion

    public interface IMutantsSavingView : IWindow
    {
      

    }

    public partial class MutantsSavingView : Window, IMutantsSavingView
    {
        private IOwnerWindowProvider _windowProvider;

        public MutantsSavingView(IOwnerWindowProvider windowProvider)
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
