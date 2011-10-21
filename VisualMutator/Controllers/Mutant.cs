namespace VisualMutator.Controllers
{
    using System;
    using System.Collections.Generic;

    using CommonUtilityInfrastructure;
    using CommonUtilityInfrastructure.WpfUtils;

    using Mono.Cecil;

    using VisualMutator.Extensibility;
    using VisualMutator.Model.Mutations;
    using VisualMutator.Model.Tests;
    using VisualMutator.Model.Tests.TestsTree;

    public class Mutant : MutationNode
    {
        private readonly int _id;

        private readonly MutationResult _mutationResult;

        public MutationResult MutationResult
        {
            get
            {
                return _mutationResult;
            }
        }

        public Mutant(int id, MutationNode parent, MutationResult mutationResult)
            : base(parent, "Mutant", false)
        {
            _id = id;
            _mutationResult = mutationResult;
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
                value == MutantResultState.Killed ? "Killed by {0} mutants".Formatted(NumberOfTestsThatKilled) :
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