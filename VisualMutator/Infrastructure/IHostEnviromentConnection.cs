

namespace VisualMutator.Infrastructure
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Windows.Forms;
    using Model;
    using UsefulTools.Paths;
    using UsefulTools.Wpf;

    #endregion

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
        string GetTempPath();
    
        IEnumerable<DirectoryPathAbsolute> GetProjectPaths();
        void Test();
        bool GetCurrentClassAndMethod(out MethodIdentifier methodIdentifier);
    }
}
