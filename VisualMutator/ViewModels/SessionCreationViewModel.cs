namespace VisualMutator.ViewModels
{
    #region

    using System.Collections.Generic;
    using NUnit.Framework;
    using UsefulTools.Paths;
    using Views;

    #endregion

    public class SessionCreationViewModel : CreationViewModel<ISessionCreationView>
    {
   


        private MutantsTestingOptionsViewModel _mutantsTesting;

        public SessionCreationViewModel(
            ISessionCreationView view,
            TypesTreeViewModel typesTreeToMutate, 
            TestsSelectableTreeViewModel typesTreeToTest, 
            MutationsTreeViewModel mutationsTree, 
            MutantsCreationOptionsViewModel mutantsCreation,
             MutantsTestingOptionsViewModel mutantsTesting)
            : base(view, typesTreeToMutate, typesTreeToTest, mutationsTree, mutantsCreation)
        {
            MutantsTesting = mutantsTesting;
            MutantsTesting.Initialize(View);

        }


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

        
    }
}