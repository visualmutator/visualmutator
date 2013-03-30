namespace VisualMutator.GUI
{
    using System;
    using System.Collections.Generic;

    using CommonUtilityInfrastructure.Paths;
    using CommonUtilityInfrastructure.WpfUtils;
    using Infrastructure;
    using IWin32Window = System.Windows.Forms.IWin32Window;

    public class EnvironmentConnection : IHostEnviromentConnection
    {
        public void Initialize()
        {
            throw new NotImplementedException();
        }

        public IObservable<EventType> Events { get; private set; }
        public NativeWindowInfo WindowInfo { get; private set; }
        public IWin32Window GetWindow()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<FilePathAbsolute> GetProjectAssemblyPaths()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<DirectoryPathAbsolute> GetProjectPaths()
        {
            throw new NotImplementedException();
        }
    }
}