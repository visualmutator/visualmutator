

namespace VisualMutator.Model.Tests.Services
{
    using System.Collections.Generic;

    using VisualMutator.Model.Tests.TestsTree;

    public interface ITestService
    {
        IEnumerable<TestNodeClass> LoadTests(IEnumerable<string> assemblies, TestSession testSession);

        List<TestNodeMethod> RunTests(TestSession testSession);

        void UnloadTests();

        void Cancel();
    }

  
}
