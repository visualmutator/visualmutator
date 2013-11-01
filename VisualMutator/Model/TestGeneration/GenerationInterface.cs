namespace VisualMutator.Model.TestGeneration
{
    #region

    using System;

    #endregion

    public abstract class GenerationInterface : MarshalByRefObject
    {
        public abstract string TestRun(int s);
    }
}