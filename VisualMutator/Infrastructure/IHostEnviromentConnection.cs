

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
        HostClosed,
        BuildBegin,
        BuildDone
    }
    public interface IHostEnviromentConnection
    {
        void Initialize();

        IObservable<EventType> Events { get; }

        IEnumerable<FilePathAbsolute> GetProjectAssemblyPaths();
        string GetTempPath();
    
        void Test();
        bool GetCurrentClassAndMethod(out MethodIdentifier methodIdentifier);
        void Build();
    }
}
