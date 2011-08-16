namespace PiotrTrzpil.VisualMutator_VSPackage.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using PiotrTrzpil.VisualMutator_VSPackage.Views;
    using PiotrTrzpil.VisualMutator_VSPackage.Views.Abstract;

    public class MainWindowViewModel : PiotrTrzpil.VisualMutator_VSPackage.Infrastructure.WpfUtils.ViewModel<IMainControl>
    {

        private object _iLMutationsView;

        private object _unitTestsView;

        public MainWindowViewModel(IMainControl view)
            : base(view)
        {
        }

        public object ILMutationsView
        {
            set
            {
                _iLMutationsView = value;
                this.RaisePropertyChanged(() => ILMutationsView);
            }
            get
            {
                return _iLMutationsView;
            }
        }


        public object UnitTestsView
        {
            set
            {
                _unitTestsView = value;
                this.RaisePropertyChanged(() => UnitTestsView);
            }
            get
            {
                return _unitTestsView;
            }
        }
    }
}