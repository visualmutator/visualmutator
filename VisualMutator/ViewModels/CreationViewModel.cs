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

    public class CreationViewModel<TView> : ViewModel<TView>
        where TView : class, IWindow
    {



        public CreationViewModel(
            TView view,
            TypesTreeViewModel typesTreeToMutate,
            TypesTreeViewModel typesTreeToTest,
            MutationsTreeViewModel mutationsTree,
            MutantsCreationOptionsViewModel mutantsCreation)
            : base(view)
        {
            MutationsTree = mutationsTree;
            TypesTreeMutate = typesTreeToMutate;
            TypesTreeToTest = typesTreeToTest;
            MutantsCreation = mutantsCreation;

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

        private TypesTreeViewModel _typesTreeMutate;

        public TypesTreeViewModel TypesTreeMutate
        {
            get
            {
                return _typesTreeMutate;
            }
            set
            {
                SetAndRise(ref _typesTreeMutate, value, () => TypesTreeMutate);
            }
        }

        private TypesTreeViewModel _typesTreeToTest;

        public TypesTreeViewModel TypesTreeToTest
        {
            get
            {
                return _typesTreeToTest;
            }
            set
            {
                SetAndRise(ref _typesTreeToTest, value, () => TypesTreeToTest);
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
            View.SetDefaultOwnerAndShowDialog();
        }

        public void Close()
        {
            View.Close();
        }
    }
}