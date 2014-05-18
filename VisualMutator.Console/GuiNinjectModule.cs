namespace VisualMutator.Console
{
    using System.Windows;
    using UsefulTools.Wpf;
    using IWin32Window = System.Windows.Forms.IWin32Window;

    public class FakeOwnerWindowProvider : IOwnerWindowProvider
    {

        public string GetWindowTitle()
        {
            return "";
        }

        public IWin32Window GetWindow()
        {
            return null;
        }

        public void SetOwnerFor(Window window)
        {
        }
    }
}