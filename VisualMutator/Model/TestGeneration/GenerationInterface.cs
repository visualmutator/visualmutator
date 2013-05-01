namespace VisualMutator.Model.TestGeneration
{
    public abstract class GenerationInterface : System.MarshalByRefObject
    {
        public abstract string TestRun(int s);
    }
}