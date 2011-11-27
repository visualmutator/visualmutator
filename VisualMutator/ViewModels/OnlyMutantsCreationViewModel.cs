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

    public class OnlyMutantsCreationViewModel : ViewModel<IOnlyMutantsCreationView>
    {



        public OnlyMutantsCreationViewModel(
            IOnlyMutantsCreationView view,
            TypesTreeViewModel typesTree,
            MutationsTreeViewModel mutationsTree,
            MutantsCreationOptionsViewModel mutantsCreation,
            MutantsTestingOptionsViewModel mutantsTesting)
            : base(view)
        {
            MutationsTree = mutationsTree;
            TypesTree = typesTree;
            MutantsCreation = mutantsCreation;
            MutantsTesting = mutantsTesting;
        }

        private MutationsTreeViewModel _mutationsTree;

        public MutationsTreeViewModel MutationsTree
        {
            get
            {
                return _mutationsTree;
            }
            set
            {
                SetAndRise(ref _mutationsTree, value, () => MutationsTree);
            }
        }
        private MutantsTestingOptionsViewModel _mutantsTesting;

        public MutantsTestingOptionsViewModel MutantsTesting
        {
            get
            {
                return _mutantsTesting;
            }
            set
            {
                SetAndRise(ref _mutantsTesting, value, () => MutantsTesting);
            }
        }

        private MutantsCreationOptionsViewModel _mutantsCreation;

        public MutantsCreationOptionsViewModel MutantsCreation
        {
            get
            {
                return _mutantsCreation;
            }
            set
            {
                SetAndRise(ref _mutantsCreation, value, () => MutantsCreation);
            }
        }

        private TypesTreeViewModel _typesTree;

        public TypesTreeViewModel TypesTree
        {
            get
            {
                return _typesTree;
            }
            set
            {
                SetAndRise(ref _typesTree, value, () => TypesTree);
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


        private BasicCommand _commandBrowseForMutantFolder;

        public BasicCommand CommandBrowseForMutantFolder
        {
            get
            {
                return _commandBrowseForMutantFolder;
            }
            set
            {
                SetAndRise(ref _commandBrowseForMutantFolder, value, () => CommandBrowseForMutantFolder);
            }
        }

        private string _mutantsFolderPath;

        public string MutantsFolderPath
        {
            get
            {
                return _mutantsFolderPath;
            }
            set
            {
                SetAndRise(ref _mutantsFolderPath, value, () => MutantsFolderPath);
            }
        }

        public void ShowDialog()
        {
            View.SetOwnerAndShowDialog();
        }

        public void Close()
        {
            View.Close();
        }
    }
}