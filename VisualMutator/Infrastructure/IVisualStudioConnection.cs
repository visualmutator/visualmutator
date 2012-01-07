

namespace VisualMutator.Infrastructure
{

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows.Forms;

    using CommonUtilityInfrastructure.Paths;

    using PiotrTrzpil.VisualMutator_VSPackage.Model;

    public interface IVisualStudioConnection
    {
  

        string InstallPath
        {
            get;
        }

        bool IsSolutionOpen { get; }

        NativeWindowInfo WindowInfo { get; }

        IEnumerable<FilePathAbsolute> GetProjectAssemblyPaths();

        string GetMutantsRootFolderPath();

        IEnumerable<string> GetReferencedAssemblies();

        event Action OnBuildBegin;

        event Action OnBuildDone;

        event Action SolutionOpened;

        event Action SolutionAfterClosing;

        void Initialize();

        void OpenFile(string className);

        IWin32Window GetWindow();

        IEnumerable<DirectoryPathAbsolute> GetProjectPaths();
    }
}
