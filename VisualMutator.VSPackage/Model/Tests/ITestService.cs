

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

        List<TestNodeMethod> RunTests();
    }

    public abstract class AbstractTestService : ITestService
    {
        public abstract IEnumerable<TestNodeClass> LoadTests(IEnumerable<string> assemblies);

        public abstract List<TestNodeMethod> RunTests();

        protected AbstractTestService()
        {
            TestMap = new Dictionary<string, TestNodeMethod>();
        }

        public IDictionary<string, TestNodeMethod> TestMap
        {
            get;
            set;
        }

    }
}
