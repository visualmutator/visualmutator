

namespace PiotrTrzpil.VisualMutator_VSPackage.Model.Tests
{

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface ITestService
    {
        IEnumerable<TestNodeClass> LoadTests(IEnumerable<string> assemblies);

        void RunTests();
    }

    public abstract class AbstractTestService : ITestService
    {
        public abstract IEnumerable<TestNodeClass> LoadTests(IEnumerable<string> assemblies);

        public abstract void RunTests();

        protected AbstractTestService()
        {
            TestMap = new Dictionary<string, TestTreeNode>();
        }

        protected IDictionary<string, TestTreeNode> TestMap
        {
            get;
            set;
        }

    }
}
