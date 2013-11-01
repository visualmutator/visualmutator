namespace VisualMutator.ViewModels
{
    #region

    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Model.Mutations.Operators;
    using UsefulTools.ExtensionMethods;
    using UsefulTools.Wpf;
    using Views;

    #endregion

    public class MutationsTreeViewModel : ViewModel<IMutationsTreeView>
    {
        public MutationsTreeViewModel(IMutationsTreeView view)
            : base(view)
        {
            _mutationPackages = new List<PackageNode>().ToReadonly();
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