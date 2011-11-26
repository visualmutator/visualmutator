namespace VisualMutator.Model.Mutations.Structure
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

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



        public int SizeInKilobytes()
        {
            int sizeBytes = Streams.Aggregate(0, (all, next) => all + next.Length);
            return (int)(((double)sizeBytes) / 1024);
        }

    }
}