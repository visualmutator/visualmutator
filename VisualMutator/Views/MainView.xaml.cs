using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace VisualMutator.Views
{
    using CommonUtilityInfrastructure.WpfUtils;

    using VisualMutator.Model.Mutations.Structure;
    using VisualMutator.Model.Tests.TestsTree;
    using VisualMutator.ViewModels;

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
            var vm = (MainViewModel)this.DataContext;
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
