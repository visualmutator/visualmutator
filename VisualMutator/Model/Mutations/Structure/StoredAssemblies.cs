namespace VisualMutator.Model.Mutations
{
    using System.Collections.Generic;
    using System.IO;

    public class StoredAssemblies
    {
        private readonly IList<Stream> _streams;

        public StoredAssemblies(IList<Stream> streams)
        {
            _streams = streams;
        }

        public IList<Stream> Streams
        {
            get
            {
                return _streams;
            }
        }
    }
}