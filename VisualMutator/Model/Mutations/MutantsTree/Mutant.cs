namespace VisualMutator.Model.Mutations.MutantsTree
{
    #region

    using System.Collections.Generic;
    using Extensibility;
    using Tests;
    using UsefulTools.ExtensionMethods;
    using UsefulTools.Switches;

    #endregion

    public class Mutant : MutationNode
    {
        private readonly string _id;

        private readonly MutationTarget _mutationTarget;
        private readonly List<MutationTarget> _commonTargets;

        public List<MutationTarget> CommonTargets
        {
            get { return _commonTargets; }
        }

     
        public MutationTarget MutationTarget
        {
            get
            {
                return _mutationTarget;
            }
        }
        public ExecutedOperator ExecutedOperator
        {
            get
            {
                return Parent.CastTo<MutantGroup>().Parent.CastTo<ExecutedOperator>();
            }
        }
        public Mutant(string id, MutantGroup parent, MutationTarget mutationTarget, List<MutationTarget> commonTargets)
            : base( "Mutant", false)
        {
            _id = id;
            _mutationTarget = mutationTarget;
            _commonTargets = commonTargets;

            _mutantTestSession  = new MutantTestSession();

            Parent = parent;
        }

        public string Id
        {
            get
            {
                return _id;
            }
        }
        protected override void SetState(MutantResultState value, bool updateChildren, bool updateParent)
        {
            base.SetState(value, updateChildren, updateParent);
            UpdateDisplayedText();
        }

        public void UpdateDisplayedText()
        {
            string stateText =
             Switch.Into<string>().From(State)
             .Case(MutantResultState.Untested, "Untested")
             .Case(MutantResultState.Creating, "Creating mutant...")
             .Case(MutantResultState.Tested, "Executing tests...")
             .Case(MutantResultState.Killed, () =>
             {
                 return Switch.Into<string>().From(KilledSubstate)
                     .Case(MutantKilledSubstate.Normal, () => "Killed by {0} tests".Formatted(NumberOfFailedTests))
                     .Case(MutantKilledSubstate.Inconclusive, () => "Killed by {0} tests".Formatted(NumberOfFailedTests))
                     .Case(MutantKilledSubstate.Cancelled, () => "Cancelled")
                     .GetResult();
             })
             .Case(MutantResultState.Live, "Live")
             .Case(MutantResultState.Error, () => MutantTestSession.ErrorDescription)
             .GetResult();


            DisplayedText = "#{0} - {1} - {2}".Formatted(Id, MutationTarget.Variant.Signature, stateText);
        }
        private int _numberOfFailedTests;

        public int NumberOfFailedTests
        {
            get
            {
                return _numberOfFailedTests;
            }
            set
            {
                SetAndRise(ref _numberOfFailedTests, value, () => NumberOfFailedTests);
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

        public override string ToString()
        {
            return MutationTarget.Variant.Signature;
        }

        public string Description
        {
            get
            {
                return MutationTarget.Variant.Signature;
            }
        }
        private MutantTestSession _mutantTestSession;

        public MutantTestSession MutantTestSession
        {
            get
            {
                return _mutantTestSession;
            }

        }

        public MutantKilledSubstate KilledSubstate { get; set; }

        public MutantGroup MutantGroup
        {
            get { return (MutantGroup) Parent; }
        }
    }

    public enum MutantKilledSubstate
    {
        Normal,
        Inconclusive,
        Cancelled,
    }
}