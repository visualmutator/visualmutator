namespace VisualMutator.Model.Mutations.Structure
{
    using System;

    using CommonUtilityInfrastructure;

    using VisualMutator.Extensibility;
    using VisualMutator.Model.Tests;

    public class Mutant : MutationNode
    {
        private readonly int _id;

        private readonly MutationTarget _mutationTarget;

        private readonly StoredAssemblies _storedAssemblies;

        public StoredAssemblies StoredAssemblies
        {
            get
            {
                return _storedAssemblies;
            }
        }

        public MutationTarget MutationTarget
        {
            get
            {
                return _mutationTarget;
            }
        }

        public Mutant(int id, MutationNode parent, MutationTarget mutationTarget, StoredAssemblies storedAssemblies)
            : base(parent, "Mutant", false)
        {
            _id = id;
            _mutationTarget = mutationTarget;
            _storedAssemblies = storedAssemblies;
        }

        public int Id
        {
            get
            {
                return _id;
            }
        }
        protected override void SetState(MutantResultState value, bool updateChildren, bool updateParent)
        {
            string stateText =
                value == MutantResultState.Waiting ? "Waiting..." :
                value == MutantResultState.Tested ? "Executing tests..." :
                value == MutantResultState.Killed ? "Killed by {0} tests".Formatted(NumberOfTestsThatKilled) :
                value == MutantResultState.Live ? "Live" : null;
            if (stateText == null)
            {
                throw new InvalidOperationException();
            }

            DisplayedText = "#{0} {1}".Formatted(Id, stateText);


            base.SetState(value, updateChildren, updateParent);
        }

        private int _numberOfTestsThatKilled;

        public int NumberOfTestsThatKilled
        {
            get
            {
                return _numberOfTestsThatKilled;
            }
            set
            {
                SetAndRise(ref _numberOfTestsThatKilled, value, () => NumberOfTestsThatKilled);
            }
        }

        private string _displayedText;

        public string DisplayedText  
        {
            get
            {
                return _displayedText;
            }
            set
            {
                SetAndRise(ref _displayedText, value, () => DisplayedText);
            }
        }

        private TestSession _testSession;

        public TestSession TestSession
        {
            get
            {
                return _testSession;
            }
            set
            {
                SetAndRise(ref _testSession, value, () => TestSession);
            }
        }

    }
}