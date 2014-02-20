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
            
        }

        private ReadOnlyCollection<TestNodeAssembly> _testAssemblies;

        public ReadOnlyCollection<TestNodeAssembly> TestAssemblies
        {
            get
            {
                return _testAssemblies;
            }
            set
            {
                SetAndRise(ref _testAssemblies, value, () => TestAssemblies);
            }
        }
        
    }
}