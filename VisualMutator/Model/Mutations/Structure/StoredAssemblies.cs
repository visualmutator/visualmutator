namespace VisualMutator.Model.Mutations.Structure
{
    using System.Collections.Generic;
    using System.IO;

    public class StoredAssemblies
    {
        private readonly IList<byte[]> _streams;

        public StoredAssemblies(IList<byte[]> streams)
        {
            _streams = streams;
        }

        public IList<byte[]> Streams
        {
            get
            {
                return _streams;
            }
        }
    }
}