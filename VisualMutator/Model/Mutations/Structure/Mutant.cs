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
            : base( "Mutant", false)
        {
            _id = id;
            _mutationTarget = mutationTarget;
            _storedAssemblies = storedAssemblies;
            _testSession  = new TestSession();

            Parent = parent;
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
                Switch.Into<string>().From(value)
                //Functional.ValuedSwitch<MutantResultState, string>(value)
                .Case(MutantResultState.Untested, "Untested")
                .Case(MutantResultState.Tested, "Executing tests...")
                .Case(MutantResultState.Killed, () =>
                {
                    return Switch.Into<string>().From(KilledSubstate)
                        .Case(MutantKilledSubstate.Normal, "Killed by {0} tests".Formatted(NumberOfFailedTests))
                        .Case(MutantKilledSubstate.Inconclusive, "Killed by {0} tests".Formatted(NumberOfFailedTests))
                        .Case(MutantKilledSubstate.TimedOut, "Killed by {0} tests (timed out)".Formatted(NumberOfFailedTests))
                        .GetResult();
                })
                .Case(MutantResultState.Live, "Live")
                .Case(MutantResultState.Error, () => TestSession.ErrorDescription)
                .GetResult();

            DisplayedText = "#{0} {1}".Formatted(Id, stateText);
            base.SetState(value, updateChildren, updateParent);
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

        private TestSession _testSession;

        public TestSession TestSession
        {
            get
            {
                return _testSession;
            }

        }

        public MutantKilledSubstate KilledSubstate { get; set; }

    }

    public enum MutantKilledSubstate
    {
        Normal,
        Inconclusive,
        TimedOut,
    }
}