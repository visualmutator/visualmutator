namespace VisualMutator.Controllers
{
    using System;
    using System.Collections.Generic;
    using Model.Mutations.MutantsTree;
    using Model.Mutations.Types;
    using Model.StoringMutants;

    public class MutationFinishedEventArgs : SessionEventArgs
    {
        public MutationFinishedEventArgs(OperationsState eventType)
            : base(eventType)
        {
        }

        public IList<AssemblyNode> MutantsGrouped
        {
            get;
            set;
        }
    }

    internal enum RequestedHaltState
    {
        Pause,

        Stop
    }

    internal enum SessionState
    {
        NotStarted,

        Paused,

        Running,

        Finished
    }

    public class SessionEventArgs : EventArgs
    {
        private OperationsState _eventType;

        public SessionEventArgs(OperationsState eventType)
        {
            _eventType = eventType;
        }

        public OperationsState EventType
        {
            get
            {
                return _eventType;
            }
        }
    }

    public class MutantVerifiedEvent : SessionEventArgs
    {
        public Mutant Mutant { get; set; }
        public bool VerificationResult { get; set; }

        public MutantVerifiedEvent()
            : base(OperationsState.PreCheck)
        {
        }

        public MutantVerifiedEvent(Mutant mutant, bool verificationResult)
            : base(OperationsState.PreCheck)
        {
            Mutant = mutant;
            VerificationResult = verificationResult;
        }
    }

    public class MutantTestedEvent : SessionEventArgs
    {
        private readonly MutantResultState _state;

        public MutantTestedEvent(MutantResultState state)
            : base(OperationsState.Testing)
        {
            _state = state;
        }

        public MutantResultState State
        {
            get { return _state; }
        }
    }

    public class MutantStoredEventArgs : SessionEventArgs
    {
        private readonly StoredMutantInfo _storedMutantInfo;

        public MutantStoredEventArgs(StoredMutantInfo storedMutantInfo)
            : base(OperationsState.None)
        {
            _storedMutantInfo = storedMutantInfo;
        }

        public StoredMutantInfo StoredMutantInfo
        {
            get { return _storedMutantInfo; }
        }
    }
    public enum ProgressUpdateMode
    {
        SetValue,
        Indeterminate,
        PreserveValue
    }
    public class MinorSessionUpdateEventArgs : SessionEventArgs
    {

        public MinorSessionUpdateEventArgs(OperationsState eventType, int progress = 0)
            : base(eventType)
        {
            _progressUpdateMode = ProgressUpdateMode.SetValue;
            _percentCompleted = progress;
        }
        public MinorSessionUpdateEventArgs(OperationsState eventType, ProgressUpdateMode progressUpdateMode)
            : base(eventType)
        {
            _progressUpdateMode = progressUpdateMode;
            _percentCompleted = 0;
        }

        private readonly ProgressUpdateMode _progressUpdateMode;

        private int _percentCompleted;

        public ProgressUpdateMode ProgressUpdateMode
        {
            get
            {
                return _progressUpdateMode;
            }
        }

        public int PercentCompleted
        {
            get
            {
                return _percentCompleted;
            }

        }
    }
    public class TestingProgressEventArgs : SessionEventArgs
    {
        public TestingProgressEventArgs(OperationsState eventType)
            : base(eventType)
        {
        }

        public int NumberOfMutantsKilled
        {
            get;
            set;
        }

        public int NumberOfAllMutantsTested
        {
            get;
            set;
        }

        public double MutationScore
        {
            get;
            set;
        }

        public int NumberOfAllMutants
        {
            get;
            set;
        }
    }
}