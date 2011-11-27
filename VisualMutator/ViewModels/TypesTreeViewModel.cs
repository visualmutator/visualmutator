namespace VisualMutator.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text;

    using CommonUtilityInfrastructure.WpfUtils;

    using VisualMutator.Model.Mutations.Types;
    using VisualMutator.Views;

    public class TypesTreeViewModel : ViewModel<ITypesTreeView>
    {
        public TypesTreeViewModel(ITypesTreeView view) :base(view)
        {
           
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