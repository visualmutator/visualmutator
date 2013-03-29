

namespace VisualMutator.Infrastructure
{

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows.Forms;

    using CommonUtilityInfrastructure.Paths;
    using CommonUtilityInfrastructure.WpfUtils;
    public enum EventType
    {
        HostOpened,
        HostClosed
    }
    public interface IHostEnviromentConnection
    {
        
        void Initialize();

        IObservable<EventType> Events { get; }

        NativeWindowInfo WindowInfo
        {
            get;
        }
        IWin32Window GetWindow();

        IEnumerable<FilePathAbsolute> GetProjectAssemblyPaths();
    
        IEnumerable<DirectoryPathAbsolute> GetProjectPaths();
    }
}
