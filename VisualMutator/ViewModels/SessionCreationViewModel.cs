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

    public class SessionCreationViewModel : ViewModel<ISessionCreationView>
    {
   


        public SessionCreationViewModel(
            ISessionCreationView view,
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