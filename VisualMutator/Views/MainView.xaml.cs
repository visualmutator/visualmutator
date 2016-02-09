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

            string message = ((Mutant)Tree.SelectedItem).MutantTestSession.Exception.ToString();
            MessageBox.Show(message);
        }


        private void trv_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var item = ((TreeViewItem)sender);
                var mutant = (Mutant)((TreeViewItem)sender).DataContext;
                item.IsSelected = true;
                item.ContextMenu = (ContextMenu)Tree.Resources["ErrorMutantContextMenu"];
                var messageItem = (item.ContextMenu.Items[0] as MenuItem);
                var markEquivalent = (item.ContextMenu.Items[1] as MenuItem);
                var unmarkEquivalent = (item.ContextMenu.Items[2] as MenuItem);

                messageItem.Visibility = mutant.State == MutantResultState.Error ?
                    Visibility.Visible : Visibility.Collapsed;

                if (mutant.State == MutantResultState.Live)
                {
                    if (mutant.IsEquivalent)
                    {
                        unmarkEquivalent.Visibility = Visibility.Visible;
                        markEquivalent.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        markEquivalent.Visibility = Visibility.Visible;
                        unmarkEquivalent.Visibility = Visibility.Collapsed;
                    }
                }
                else
                {
                    markEquivalent.Visibility = Visibility.Collapsed;
                    unmarkEquivalent.Visibility = Visibility.Collapsed;
                }

            }
            catch
            {
                
            }
        }

        private void MenuItemMarkEquivalent_Click(object sender, RoutedEventArgs e)
        {
            var mutant = ((Mutant) Tree.SelectedItem);
            mutant.IsEquivalent = true;
        }

        private void MenuItemUnmarkEquivalent_Click(object sender, RoutedEventArgs e)
        {
            var mutant = ((Mutant)Tree.SelectedItem);
            mutant.IsEquivalent = false;
        }
    }

   

}
