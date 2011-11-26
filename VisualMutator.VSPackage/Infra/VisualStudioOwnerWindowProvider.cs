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

        public void SetOwnerAndCenter(Window window)
        {
            NativeWindowInfo vsWindow = _visualStudio.WindowInfo;

            // Set the owned WPF window’s owner with the non-WPF owner window

            WindowInteropHelper helper = new WindowInteropHelper(window);

            helper.Owner = vsWindow.Handle;

            // Center window

            // Note - Need to use HwndSource to get handle to WPF owned window,

            //        and the handle only exists when SourceInitialized has been

            //        raised

            window.SourceInitialized += delegate

            {
                // Get WPF size and location for non-WPF owner window

                int nonWPFOwnerLeft = vsWindow.Left; // Get non-WPF owner’s Left

                int nonWPFOwnerWidth = vsWindow.Width; // Get non-WPF owner’s Width

                int nonWPFOwnerTop = vsWindow.Top; // Get non-WPF owner’s Top

                int nonWPFOwnerHeight = vsWindow.Top; // Get non-WPF owner’s Height

                // Get transform matrix to transform non-WPF owner window

                // size and location units into device-independent WPF

                // size and location units

                HwndSource source = HwndSource.FromHwnd(helper.Handle);

                if (source == null)
                {
                    return;
                }

                Matrix matrix = source.CompositionTarget.TransformFromDevice;

                Point ownerWPFSize = matrix.Transform(new Point(nonWPFOwnerWidth, nonWPFOwnerHeight));

                Point ownerWPFPosition = matrix.Transform(new Point(nonWPFOwnerLeft, nonWPFOwnerTop));

                // Center WPF window

                window.WindowStartupLocation = WindowStartupLocation.Manual;

                window.Left = ownerWPFPosition.X + (ownerWPFSize.X - window.Width) / 2;

                window.Top = ownerWPFPosition.Y + (ownerWPFSize.Y - window.Height) / 2;
            };

            // Show WPF owned window

         //   window.Show();
        }
    }
}