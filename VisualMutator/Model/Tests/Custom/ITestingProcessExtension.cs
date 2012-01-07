namespace VisualMutator.Model.Tests.Custom
{
    using System.Collections.Generic;

    using CommonUtilityInfrastructure.Paths;

    public interface ITestingProcessExtension
    {
        void Initialize(string parameter, IList<DirectoryPathAbsolute> projectPaths);

        void PrepareForMutant(string mutantDestination, List<string> mutantFilePaths);

        void OnTestingCancelled();

        string Name { get; }

        void OnSessionFinished();
    }
}