namespace VisualMutator.Model.Tests
{
    using TestsTree;

    public class TmpTestNodeMethod
    {
        private readonly string _fullName;

        public TmpTestNodeMethod(string fullName)
        {
            _fullName = fullName;
        }

        public string Message { get; set; }
        public TestNodeState State
        {
            get;
            set;
        }

        public string Name
        {
            get { return _fullName; }
        }
    }
}