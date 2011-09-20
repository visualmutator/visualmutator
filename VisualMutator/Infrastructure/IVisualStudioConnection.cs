

namespace VisualMutator.Infrastructure
{

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

 
    public interface IVisualStudioConnection
    {
  

        string InstallPath
        {
            get;
        }

     
        IEnumerable<string> GetProjectPaths();

        string GetMutantsRootFolderPath();

        IEnumerable<string> GetReferencedAssemblies();

        event Action OnBuildBegin;

        event Action OnBuildDone;

        event Action SolutionOpened;

        event Action SolutionAfterClosing;

        void Initialize();

        void OpenFile(string className);
    }
}
