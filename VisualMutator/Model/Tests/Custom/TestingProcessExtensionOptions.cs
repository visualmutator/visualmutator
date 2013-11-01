namespace VisualMutator.Model.Tests.Custom
{
    public class TestingProcessExtensionOptions
    {
        public ITestingProcessExtension TestingProcessExtension { get; set; }

        public string Parameter { get; set; }

        public static TestingProcessExtensionOptions Default
        {
            get
            {
                return new TestingProcessExtensionOptions
                {
                    Parameter = "",
                    TestingProcessExtension = new DefaultTestingProcessExtension()
                };
            }
            
        }
    }
}