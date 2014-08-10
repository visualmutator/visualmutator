namespace VisualMutator.Model.Tests.Services
{
    using System.Threading.Tasks;

    public interface ITestsRunContext
    {
        Task<MutantTestResults> RunTests();
        void CancelRun();
        MutantTestResults TestResults { get; }
    }
}