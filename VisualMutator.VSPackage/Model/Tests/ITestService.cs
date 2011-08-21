

namespace PiotrTrzpil.VisualMutator_VSPackage.Model.Tests
{

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface ITestService
    {
        Task<IEnumerable<TestNodeNamespace>> LoadTests(IEnumerable<string> assemblies);

        Task RunTests();
    }

    public abstract class AbstractTestService : ITestService
    {
        public abstract Task<IEnumerable<TestNodeNamespace>> LoadTests(IEnumerable<string> assemblies);

        public abstract Task RunTests();

        protected IDictionary<string, TestTreeNode> TestMap
        {
            get;
            set;
        }

    }
}
