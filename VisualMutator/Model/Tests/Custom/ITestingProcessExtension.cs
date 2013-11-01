namespace VisualMutator.Model.Tests.Custom
{
    #region

    using System.Collections.Generic;

    #endregion

    public interface ITestingProcessExtension
    {
        void OnSessionStarting(string parameter, IList<string> projectPaths);

        void OnTestingOfMutantStarting(string mutantDestination, IList<string> mutantFilePaths);

        void OnTestingCancelled();

        string Name { get; }

        void OnSessionFinished();
    }
}