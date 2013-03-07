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

    public class OnlyMutantsCreationViewModel : CreationViewModel<IOnlyMutantsCreationView>
    {





        private BasicCommand _commandBrowseForMutantFolder;

        public OnlyMutantsCreationViewModel(
            IOnlyMutantsCreationView view, 
            TypesTreeViewModel typesTree, 
            TypesTreeViewModel typesTreeTest, 
            MutationsTreeViewModel mutationsTree, 
            MutantsCreationOptionsViewModel mutantsCreation)
            : base(view, typesTree, typesTreeTest, mutationsTree, mutantsCreation)
        {
            MutationsTree = mutationsTree;
            TypesTreeMutate = typesTree;
            MutantsCreation = mutantsCreation;
        }

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

    }
}