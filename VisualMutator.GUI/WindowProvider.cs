namespace VisualMutator.GUI
{
    using System.Windows;
    using System.Windows.Interop;
    using UsefulTools.Wpf;
    using IWin32Window = System.Windows.Forms.IWin32Window;

    public class WindowProvider : IOwnerWindowProvider
    {
        private Window _window;

        public void Initialize(Window window)
        {
            _window = window;
        }


        public IWin32Window GetWindow()
        {
            return new NativeWindowInfo(new WindowInteropHelper(_window).Handle);
        }

        public void SetOwnerFor(Window window)
        {
            window.Owner = _window;
        }

        public string GetWindowTitle()
        {
            return "VisualMutator";
        }
    }
}