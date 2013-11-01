namespace VisualMutator.Views
{
    #region

    using System.Windows.Controls;
    using UsefulTools.Core;

    #endregion

    public interface IMutationsTreeView : IView
    {
         
    }
    public partial class MutationsTree : UserControl, IMutationsTreeView
    {
        public MutationsTree()
        {
            InitializeComponent();
        }
    }
}
