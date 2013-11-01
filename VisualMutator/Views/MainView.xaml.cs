namespace VisualMutator.Views
{
    #region

    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using Model.Mutations.MutantsTree;
    using UsefulTools.Core;
    using ViewModels;

    #endregion

    public interface IMutationResultsView : IView
    {
        Visibility Visibility { set; }
    }
    public partial class MutationResultsView : UserControl, IMutationResultsView
    {
        public MutationResultsView()
        {
            InitializeComponent();
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var vm = (MainViewModel)DataContext;
            vm.SelectedMutationTreeItem = e.NewValue;
        }



        private void MenuItemMessage_Click(object sender, RoutedEventArgs e)
        {

            string message = ((Mutant)Tree.SelectedItem).MutantTestSession.ErrorMessage;
            MessageBox.Show(message);
        }


        private void trv_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var mutant = (Mutant)((TreeViewItem)sender).DataContext;
            ((TreeViewItem)sender).IsSelected = true;
            if (mutant.State == MutantResultState.Error)
            {
                ((TreeViewItem)sender).ContextMenu = (ContextMenu)Tree.Resources["ErrorMutantContextMenu"];
            }
            else
            {
                ((TreeViewItem)sender).ContextMenu = null;
                Tree.ContextMenu = null;
            }
        }
    }

   

}
