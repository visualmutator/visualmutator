namespace VisualMutator.Model.Tests.Custom
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

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