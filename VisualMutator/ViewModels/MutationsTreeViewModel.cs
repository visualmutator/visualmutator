namespace VisualMutator.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text;

    using CommonUtilityInfrastructure.WpfUtils;

    using VisualMutator.Model.Mutations.Operators;
    using VisualMutator.Model.Mutations.Types;
    using VisualMutator.Views;

    public class MutationsTreeViewModel : ViewModel<IMutationsTreeView>
    {
        public MutationsTreeViewModel(IMutationsTreeView view)
            : base(view)
        {
           
        }


        private ReadOnlyCollection<PackageNode> _mutationPackages;
        public ReadOnlyCollection<PackageNode> MutationPackages
        {
            set
            {
                SetAndRise(ref _mutationPackages, value, () => MutationPackages);
            }
            get
            {
                return _mutationPackages;
            }
        }
    }
}