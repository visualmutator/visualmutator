

namespace VisualMutator.Model.Tests.Services
{
    #region

    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Strilanc.Value;
    using TestsTree;

    #endregion

    public interface ITestService
    {
        May<TestsLoadContext> LoadTests(IList<string> assemblies);

        Task RunTests(TestsLoadContext context);

        void UnloadTests();

        void Cancel();
        void CreateTestFilter(ICollection<TestId> selectedTests);
    }

  
}
