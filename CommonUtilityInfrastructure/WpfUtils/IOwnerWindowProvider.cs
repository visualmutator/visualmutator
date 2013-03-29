namespace CommonUtilityInfrastructure.WpfUtils
{
    using System.Windows;
    using System.Windows.Forms;

    public interface IOwnerWindowProvider
    {
        IWin32Window GetWindow();

        void SetOwnerFor(Window window);

        string GetWindowTitle();
    }
}