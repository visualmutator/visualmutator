namespace PiotrTrzpil.VisualMutator_VSPackage.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using PiotrTrzpil.VisualMutator_VSPackage.Infrastructure.WpfUtils;

    using VisualMutator.Views;

    public class MutantsManagementViewModel : ViewModel<IMutantsManagementView>
    {
        public MutantsManagementViewModel(IMutantsManagementView view):base(view)
        {
            
        }


        public void Show()
        {
            View.ShowDialog();
        }
    }
}