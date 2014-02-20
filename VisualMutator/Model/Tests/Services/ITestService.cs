

namespace VisualMutator.Model.Tests.Services
{
    #region

    using System.Collections.Generic;
    using System.Threading.Tasks;
    using TestsTree;

    #endregion

    public interface ITestService
    {
        TestsLoadContext LoadTests(IList<string> assemblies);

        Task RunTests(TestsLoadContext context);

        void UnloadTests();

        void Cancel();
        void CreateTestFilter(ICollection<TestId> selectedTests);
    }

  
}
