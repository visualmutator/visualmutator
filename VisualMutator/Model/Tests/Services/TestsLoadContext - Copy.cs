namespace VisualMutator.Model.Tests.Services
{
    using System.Collections.Generic;
    using TestsTree;

    public class TestsRunContext
    {
        private readonly List<TestNodeClass> _classNodes;
        //public TestNodeAssembly TestNodeAssembly { get; set; }

        public TestsRunContext()
        {
            _classNodes = new List<TestNodeClass>();
        }

        public string AssemblyPath { get; set; }
//        public IDictionary<string, TestNodeMethod> TestMap
//        {
//            get
//            {
//                return _testMap;
//            }
//        }

        public List<TestNodeClass> ClassNodes
        {
            get { return _classNodes; }
        }

        public SelectedTests SelectedTests { get; set; }
        public MutantTestResults TestResults { get; set; }
    }
}