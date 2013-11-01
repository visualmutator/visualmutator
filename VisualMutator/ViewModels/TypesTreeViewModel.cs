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
            _assemblies = new ReadOnlyCollection<AssemblyNode>(new List<AssemblyNode>());
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
       

        

    }
}