namespace VisualMutator.Controllers
{
    using System;

    [Flags]
    public enum OperationsState
    {
        None,
        Mutating,
        Testing,
        TestingPaused,
        Pausing,
        Stopping,
        Finished,

        PreCheck,

        Error,

        MutationFinished,

        SavingMutants,

    }
}