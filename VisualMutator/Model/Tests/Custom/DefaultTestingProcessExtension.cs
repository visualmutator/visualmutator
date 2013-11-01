namespace VisualMutator.Model.Tests.Custom
{
    #region

    using System.Collections.Generic;
    using System.ComponentModel.Composition;

    #endregion

    [Export(typeof(ITestingProcessExtension))]
    class DefaultTestingProcessExtension : ITestingProcessExtension
    {
        public const string ConstName = "None";


        public void OnSessionStarting(string parameter, IList<string> projectPaths)
        {
      
        }

        public void OnTestingOfMutantStarting(string mutantDestination, IList<string> mutantFilePaths)
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