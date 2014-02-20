namespace VisualMutator.Model.Tests
{
    public class MyTestResult
    {
        private readonly string _name;

        public MyTestResult(string name)
        {
            _name = name;
            Message = "";
            StackTrace = "";
        }

        public string Name
        {
            get { return _name; }
        }

        public string Message { get; set; }
        public string StackTrace { get; set; }
        public bool Success { get; set; }
      //  public string StackTrace { get; set; }
    }
}