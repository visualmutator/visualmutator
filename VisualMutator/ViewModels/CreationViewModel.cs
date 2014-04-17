namespace VisualMutator.ViewModels
{
    #region

    using System.Collections.Generic;
    using UsefulTools.Core;
    using UsefulTools.Paths;
    using UsefulTools.Wpf;
    using Views;

    #endregion

    public class CreationViewModel : ViewModel<ISessionCreationView>
    {
        public CreationViewModel(
            ISessionCreationView view,
            TypesTreeViewModel typesTreeToMutate,
            TestsSelectableTreeViewModel typesTreeToTest,
            MutationsTreeViewModel mutationsTree,
            MutantsCreationOptionsViewModel mutantsCreation,
             MutantsTestingOptionsViewModel mutantsTesting)
            : base(view)
        {
            MutationsTree = mutationsTree;
            TypesTreeMutate = typesTreeToMutate;
            TypesTreeToTest = typesTreeToTest;
            MutantsCreation = mutantsCreation;
            MutantsTesting = mutantsTesting;
            MutantsTesting.Initialize(View);

        }
        public List<DirectoryPathAbsolute> ProjectPaths
        {
            get;
            set;
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

        private TestsSelectableTreeViewModel _typesTreeToTest;

        public TestsSelectableTreeViewModel TypesTreeToTest
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
   

        private SmartCommand _commandCreateMutants;

   
        public SmartCommand CommandCreateMutants
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

        private SmartCommand _commandWriteMutants;


        public SmartCommand CommandWriteMutants
        {
            get
            {
                return _commandWriteMutants;
            }
            set
            {
                SetAndRise(ref _commandWriteMutants, value, () => CommandWriteMutants);
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
        public string MutantsGenerationPath
        {
            get;
            set;
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