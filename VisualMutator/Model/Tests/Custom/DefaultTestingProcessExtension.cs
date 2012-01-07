namespace VisualMutator.Model.Tests.Custom
{
    using System.Collections.Generic;
    using System.ComponentModel.Composition;

    using CommonUtilityInfrastructure.Paths;

    [Export(typeof(ITestingProcessExtension))]
    class DefaultTestingProcessExtension : ITestingProcessExtension
    {
        public const string ConstName = "None";


        public void Initialize(string parameter, IList<DirectoryPathAbsolute> projectPaths)
        {
      
        }

        public void PrepareForMutant(string mutantDestination, List<string> mutantFilePaths)
        {
            
        }

        public void OnTestingCancelled()
        {
           
        }

        public string Name
        {
            get
            {
                return ConstName;
            }
        }

        public void OnSessionFinished()
        {
            
        }
    }
}