namespace VisualMutator.Controllers
{
    #region

    using System;

    #endregion

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


    }
}