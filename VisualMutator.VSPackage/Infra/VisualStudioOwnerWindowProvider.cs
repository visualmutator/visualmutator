namespace PiotrTrzpil.VisualMutator_VSPackage.Infra
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Windows.Interop;
    using System.Windows.Media;

    using CommonUtilityInfrastructure.WpfUtils;

    using PiotrTrzpil.VisualMutator_VSPackage.Model;

    using VisualMutator.Infrastructure;

    using IWin32Window = System.Windows.Forms.IWin32Window;

    public class VisualStudioOwnerWindowProvider : IOwnerWindowProvider
    {
        private readonly IVisualStudioConnection _visualStudio;

        public VisualStudioOwnerWindowProvider(IVisualStudioConnection visualStudio)
        {
            _visualStudio = visualStudio;
        }

        public IWin32Window GetWindow()
        {
            return _visualStudio.GetWindow();
        }

        public void SetOwner(Window window)
        {
            NativeWindowInfo vsWindow = _visualStudio.WindowInfo;
            WindowInteropHelper helper = new WindowInteropHelper(window);
            helper.Owner = vsWindow.Handle;
        }

       
    }
}