namespace VisualMutator.Views
{
    #region

    using System.Windows.Controls;
    using UsefulTools.Core;

    #endregion

    public interface ITestsSelectableTree : IView
    {
         
    }
    public partial class TestsSelectableTree : UserControl, ITestsSelectableTree
    {
        public TestsSelectableTree()
        {
            InitializeComponent();
        }
    }
}
