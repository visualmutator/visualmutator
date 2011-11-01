namespace VisualMutator.Views
{
    #region Usings

    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Forms;
    using System.Windows.Input;
    using System.Windows.Media;

    using CommonUtilityInfrastructure.WpfUtils;

    using MouseEventArgs = System.Windows.Input.MouseEventArgs;
    using UserControl = System.Windows.Controls.UserControl;

    #endregion

   public interface ITestsTreeView : IView
   {
       
   }
   public partial class TestsTree : UserControl, ITestsTreeView
    {
        public TestsTree()
        {
            InitializeComponent();
        }

        private void OnPreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            TreeViewItem treeViewItem = VisualUpwardSearch<TreeViewItem>((DependencyObject)e.OriginalSource) as TreeViewItem;

            if (treeViewItem != null)
            {
                treeViewItem.Focus();
                e.Handled = true;
            }
        }

        static DependencyObject VisualUpwardSearch<T>(DependencyObject source)
        {
            while (source != null && source.GetType() != typeof(T))
                source = VisualTreeHelper.GetParent(source);

            return source;
        }

    }
}