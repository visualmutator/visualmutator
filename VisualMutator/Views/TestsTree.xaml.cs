namespace VisualMutator.Views
{
    #region Usings

    using System.Windows;
    using System.Windows.Controls;

    using System.Windows.Input;
    using System.Windows.Media;

    using CommonUtilityInfrastructure.WpfUtils;

    using VisualMutator.Model.Tests.TestsTree;

    using ContextMenu = System.Windows.Controls.ContextMenu;
    using MenuItem = System.Windows.Controls.MenuItem;
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



        private void MenuItemMessage_Click(object sender, RoutedEventArgs e)
        {
            
            string message = ((TestNodeMethod)Tree.SelectedItem).Message;
            MessageBox.Show(message);
        }


       private void trv_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
       {
           var method = (TestNodeMethod)((TreeViewItem)sender).DataContext;
           ((TreeViewItem)sender).IsSelected = true;
           if (method.State == TestNodeState.Failure)
           {
               
               ((TreeViewItem)sender).ContextMenu = (ContextMenu)Tree.Resources["FailedTestContextMenu"];
           }
           else
           {
               ((TreeViewItem)sender).ContextMenu = null;
               Tree.ContextMenu = null;
           }
       }

       private void MenuItem_Click(object sender, RoutedEventArgs e)
       {

       }
    }
}