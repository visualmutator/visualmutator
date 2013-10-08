namespace VisualMutator.Extensibility
{
    using System.Collections.Generic;
    using Microsoft.Cci;

    public interface IVisualCodeVisitor
    {
        void MarkMutationTarget<T>(T obj, IList<MutationVariant> variants );
        void MarkSharedTarget<T>(T o);
        IMethodDefinition CurrentMethod { get; }
    }
}