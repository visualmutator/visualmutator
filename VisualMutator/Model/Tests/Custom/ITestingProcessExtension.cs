namespace VisualMutator.Model.Tests.Custom
{
    using System.Collections.Generic;

    using CommonUtilityInfrastructure.Paths;

    public interface ITestingProcessExtension
    {
        void OnSessionStarting(string parameter, IList<string> projectPaths);

        void OnTestingOfMutantStarting(string mutantDestination, IList<string> mutantFilePaths);

        void OnTestingCancelled();

        string Name { get; }

        void OnSessionFinished();
    }
}