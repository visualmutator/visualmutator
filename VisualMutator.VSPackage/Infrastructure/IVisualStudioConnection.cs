namespace VisualMutator.Domain
{
    using System.Collections.Generic;

    public interface IVisualStudioConnection
    {
        IEnumerable<string> GetProjectPaths();
    }
}