namespace PiotrTrzpil.VisualMutator_VSPackage.Model
{
    #region

    using System.Windows;
    using System.Windows.Interop;
    using Infra.UsefulTools.Wpf;
    using UsefulTools.Core;
    using UsefulTools.Wpf;
    using VisualMutator.Infrastructure;
    using IWin32Window = System.Windows.Forms.IWin32Window;

    #endregion

    public class VisualStudioOwnerWindowProvider : IOwnerWindowProvider
    {
        private readonly VisualStudioConnection _hostEnviroment;

        public VisualStudioOwnerWindowProvider(VisualStudioConnection hostEnviroment)
        {
            _hostEnviroment = hostEnviroment;
        }


        public object GetWindow()
        {
            return _hostEnviroment.GetWindow();
        }

        public void SetOwnerFor(IWindow window)
        {
            NativeWindowInfo vsWindow = _hostEnviroment.WindowInfo;
            WindowInteropHelper helper = new WindowInteropHelper((Window) window);
            helper.Owner = vsWindow.Handle;
        }

        public string GetWindowTitle()
        {
            return "VisualMutator";
        }
    }
}