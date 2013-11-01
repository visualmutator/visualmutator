namespace PiotrTrzpil.VisualMutator_VSPackage.Model
{
    #region

    using System.Windows;
    using System.Windows.Interop;
    using VisualMutator.Infrastructure;
    using IWin32Window = System.Windows.Forms.IWin32Window;

    #endregion

    public class VisualStudioOwnerWindowProvider : IOwnerWindowProvider
    {
        private readonly IHostEnviromentConnection _hostEnviroment;

        public VisualStudioOwnerWindowProvider(IHostEnviromentConnection hostEnviroment)
        {
            _hostEnviroment = hostEnviroment;
        }

        public IWin32Window GetWindow()
        {
            return _hostEnviroment.GetWindow();
        }

        public void SetOwnerFor(Window window)
        {
            NativeWindowInfo vsWindow = _hostEnviroment.WindowInfo;
            WindowInteropHelper helper = new WindowInteropHelper(window);
            helper.Owner = vsWindow.Handle;
        }

        public string GetWindowTitle()
        {
            return "VisualMutator";
        }
    }
}