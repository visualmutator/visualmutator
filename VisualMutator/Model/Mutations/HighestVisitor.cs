namespace VisualMutator.Model.Mutations
{
    using Extensibility;
    using Microsoft.Cci;

    public class HighestVisitor : VisualCodeVisitor
    {
        public HighestVisitor(IOperatorCodeVisitor visitor, IModule module, string id) : base(id, visitor, module)
        {

        }
    }
}