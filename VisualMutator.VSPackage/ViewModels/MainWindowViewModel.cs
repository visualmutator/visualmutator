namespace PiotrTrzpil.VisualMutator_VSPackage.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using PiotrTrzpil.VisualMutator_VSPackage.Views.Abstract;

    public class MainWindowViewModel : ExtViewModel<IMainControl>
    {

        private object _iLMutationsView;

        public MainWindowViewModel(IMainControl view)
            : base(view)
        {
        }

        public object ILMutationsView
        {
            set
            {
                _iLMutationsView = value;
                this.RaisePropertyChangedExt(() => ILMutationsView);
            }
            get
            {
                return _iLMutationsView;
            }
        }



    }
}