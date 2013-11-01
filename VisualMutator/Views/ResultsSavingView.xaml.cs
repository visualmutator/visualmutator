namespace VisualMutator.Views
{
    #region

    using System.Windows;
    using UsefulTools.Core;
    using UsefulTools.Wpf;

    #endregion

    public interface IResultsSavingView : IWindow
    {
      

    }

    public partial class ResultsSavingView : Window, IResultsSavingView
    {
        private IOwnerWindowProvider _windowProvider;

        public ResultsSavingView(IOwnerWindowProvider windowProvider)
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
