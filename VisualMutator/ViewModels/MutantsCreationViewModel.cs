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

    public class MutantsCreationViewModel : ViewModel<IMutantsCreationView>
    {
        public MutantsCreationViewModel(IMutantsCreationView view)
            : base(view)
        {
            
        }

        private bool _createMoreMutants;

        public bool CreateMoreMutants
        {
            get
            {
                return _createMoreMutants;
            }
            set
            {
                SetAndRise(ref _createMoreMutants, value, () => CreateMoreMutants);
            }
        }



        private BetterObservableCollection<string> _additionalFileToCopy;

        public BetterObservableCollection<string> AdditionalFileToCopy
        {
            get
            {
                return _additionalFileToCopy;
            }
            set
            {
                SetAndRise(ref _additionalFileToCopy, value, () => AdditionalFileToCopy);
            }
        }

        private BasicCommand _commandAdditionalFileToCopy;
        public BasicCommand CommandAdditionalFileToCopy
        {
            get
            {
                return _commandAdditionalFileToCopy;
            }
            set
            {
                SetAndRise(ref _commandAdditionalFileToCopy, value, () => CommandAdditionalFileToCopy);
            }
        }

        private int _timeoutSeconds;

        public int TimeoutSeconds
        {
            get
            {
                return _timeoutSeconds;
            }
            set
            {
                SetAndRise(ref _timeoutSeconds, value, () => TimeoutSeconds);
            }
        }
        private BasicCommand _commandCreateMutants;
        public BasicCommand CommandCreateMutants
        {
            get
            {
                return _commandCreateMutants;
            }
            set
            {
                SetAndRise(ref _commandCreateMutants, value, () => CommandCreateMutants);
            }
        }


        private ReadOnlyCollection<AssemblyNode> _assemblies;
        public ReadOnlyCollection<AssemblyNode> Assemblies
        {
            set
            {
                SetAndRise(ref _assemblies, value, () => Assemblies);
            }
            get
            {
                return _assemblies;
            }
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

        public void ShowDialog()
        {
            View.ShowDialog();
        }

        public void Close()
        {
            View.Close();
        }
    }
}