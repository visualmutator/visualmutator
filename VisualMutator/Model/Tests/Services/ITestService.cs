

namespace VisualMutator.Model.Tests.Services
{
    using System.Collections.Generic;

    using VisualMutator.Model.Tests.TestsTree;

    public interface ITestService
    {
        IEnumerable<TestNodeClass> LoadTests(IEnumerable<string> assemblies, MutantTestSession mutantTestSession);

        List<TestNodeMethod> RunTests(MutantTestSession mutantTestSession);

        void UnloadTests();

        void Cancel();
        void CreateTestFilter(ICollection<TestId> selectedTests);
    }

  
}
