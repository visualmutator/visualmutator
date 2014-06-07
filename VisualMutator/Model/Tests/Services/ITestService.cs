

namespace VisualMutator.Model.Tests.Services
{
    #region

    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Strilanc.Value;
    using TestsTree;

    #endregion

    public interface ITestsService
    {
        May<TestsLoadContext> LoadTests(string assemblyPath);


        void Cancel();
    }

  
}
