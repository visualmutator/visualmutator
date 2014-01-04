namespace VisualMutator.ViewModels
{
    #region

    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Model.Mutations.Types;
    using UsefulTools.Wpf;
    using Views;

    #endregion

    public class TypesTreeViewModel : ViewModel<ITypesTreeView>
    {
        public TypesTreeViewModel(ITypesTreeView view) :base(view)
        {
           // _assemblies = new ReadOnlyCollection<AssemblyNode>(new List<AssemblyNode>());
            IsExpanded = true;
        }

        private ReadOnlyCollection<AssemblyNode> _assemblies;

        public ReadOnlyCollection<AssemblyNode> Assemblies
        {
            get
            {
                return _assemblies;
            }
            set
            {
                SetAndRise(ref _assemblies, value, () => Assemblies);
            }
        }
        private bool _isExpanded;

        public bool IsExpanded
        {
            get
            {
                return _isExpanded;
            }
            set
            {
                SetAndRise(ref _isExpanded, value, () => IsExpanded);
            }
        }

        public List<string> AssembliesPaths { get; set; }
    }
}