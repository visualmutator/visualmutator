namespace VisualMutator.ViewModels
{
    #region

    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Model.Tests.TestsTree;
    using UsefulTools.Wpf;
    using Views;

    #endregion

    public class TestsSelectableTreeViewModel : ViewModel<ITestsSelectableTree>
    {
        public TestsSelectableTreeViewModel(ITestsSelectableTree view)
            : base(view)
        {
            _namespaces = new ReadOnlyCollection<TestNodeNamespace>(new List<TestNodeNamespace>());
        }

        private ReadOnlyCollection<TestNodeNamespace> _namespaces;

        public ReadOnlyCollection<TestNodeNamespace> Namespaces
        {
            get
            {
                return _namespaces;
            }
            set
            {
                SetAndRise(ref _namespaces, value, () => Namespaces);
            }
        }
       
    }
}