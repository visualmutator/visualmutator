namespace VisualMutator.Extensibility
{
    using System.Collections.Generic;

    using Mono.Cecil;

    public interface IMutationElement
    {
    }

    public interface IMutationElement<out T> :IMutationElement where T: IMemberDefinition
    {
        T FindIn(ICollection<AssemblyDefinition> assemblies);

    
    }
}