namespace VisualMutator.Model.Tests
{
    public class MyTestResult
    {
        public MyTestResult()
        {
            Message = "";
            StackTrace = "";
        }

        public string Message { get; set; }
        public string StackTrace { get; set; }
        public bool Success { get; set; }
      //  public string StackTrace { get; set; }
    }
}