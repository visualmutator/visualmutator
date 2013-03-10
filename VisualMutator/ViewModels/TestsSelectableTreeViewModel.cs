namespace VisualMutator.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text;

    using CommonUtilityInfrastructure.WpfUtils;
    using Model.Tests.TestsTree;
    using VisualMutator.Model.Mutations.Types;
    using VisualMutator.Views;

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