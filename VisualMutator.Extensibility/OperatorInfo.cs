namespace VisualMutator.Extensibility
{
    public class OperatorInfo
    {
        private readonly string _id;
        private readonly string _name;
        private readonly string _description;

        public OperatorInfo(string id, string name, string description)
        {
            _id = id;
            _name = name;
            _description = description;
        }

        public string Name
        {
            get { return _name; }
        }

        public string Id
        {
            get { return _id; }
        }

        public string Description
        {
            get { return _description; }
        }
    }
}