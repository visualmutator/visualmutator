namespace PiotrTrzpil.VisualMutator_VSPackage.Infrastructure
{
    using System.Collections.Generic;

    public interface IVisualStudioConnection
    {
        IEnumerable<string> GetProjectPaths();
    }
}