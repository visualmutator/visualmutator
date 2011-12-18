namespace CommonUtilityInfrastructure.WpfUtils
{
    #region Usings

    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Forms;
    using System.Windows.Interop;
    using System.Windows.Media;

    using log4net;

    using IWin32Window = System.Windows.Forms.IWin32Window;
    using MessageBox = System.Windows.MessageBox;
    using MessageBoxOptions = System.Windows.MessageBoxOptions;

    #endregion
       public interface IMessageService
    {
           void ShowMessage(IDialogView owner, string message);

           void ShowWarning(IDialogView owner, string message);

           void ShowFatalError(IDialogView owner, string message);

 
           bool ShowYesNoQuestion(IDialogView owner, string message);

           void ShowError(IDialogView owner, string message);
    }
    public class MessageService : IMessageService
    {
        private readonly IApplicationTitleProvider _titleProvider;

        private readonly IOwnerWindowProvider _provider;

        public MessageService(
            IApplicationTitleProvider titleProvider,
            IOwnerWindowProvider provider)
        {
            _titleProvider = titleProvider;
            _provider = provider;
        }
        public static IWin32Window GetIWin32Window(Visual visual)
        {
            HwndSource source = PresentationSource.FromVisual(visual).CastTo<HwndSource>();
           IWin32Window win = new OldWindow(source.Handle);
            return win;
        }

        private class OldWindow : IWin32Window
        {
            private readonly IntPtr _handle;
            public OldWindow(IntPtr handle)
            {
                _handle = handle;
            }


            IntPtr IWin32Window.Handle
            {
                get
                {
                    return _handle;
                }
            }

        }

        private IWin32Window TransformWindow(IDialogView view)
        {
            return GetIWin32Window((Window)view);
        }

        private IWin32Window GetWindow(IDialogView owner)
        {
            return owner == null ? _provider.GetWindow() : TransformWindow(owner);
        }


        public void ShowMessage(IDialogView owner, string message)
        {
            System.Windows.Forms.MessageBox.Show(
                   GetWindow(owner),
                   message,
                   TitlePart() + "Information",
                   MessageBoxButtons.OK,
                   MessageBoxIcon.Information);

        }


  
        public void ShowWarning(IDialogView owner, string message)
        {
            System.Windows.Forms.MessageBox.Show(
                   GetWindow(owner),
                   message,
                   TitlePart() + "Warning",
                   MessageBoxButtons.OK,
                   MessageBoxIcon.Exclamation);
        }

        private string TitlePart()
        {
            return _titleProvider.GetTitle()+" - ";
        }
        public void ShowFatalError(IDialogView owner, string message)
        {

            System.Windows.Forms.MessageBox.Show(
                              GetWindow(owner),
                              message,
                              TitlePart()+"Unhandled Exception",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Error);

        }
        public void ShowError(IDialogView owner, string message)
        {

            System.Windows.Forms.MessageBox.Show(
                              GetWindow(owner),
                              message,
                              TitlePart() + "Error",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Error);
        }
        /*
        public bool? ShowQuestion(IDialogView owner, string message)
        {
            MessageBoxResult result = MessageBox.Show( GetWindow(owner),
                message,
                "",
                MessageBoxButton.YesNoCancel,
                MessageBoxImage.Question,
                MessageBoxResult.Cancel,
                MessageBoxOptions);
           
           

            if (result == MessageBoxResult.Yes)
            {
                return true;
            }
            else if (result == MessageBoxResult.No)
            {
                return false;
            }

            return null;
        }*/

        public bool ShowYesNoQuestion(IDialogView owner, string message)
        {
            DialogResult result = System.Windows.Forms.MessageBox.Show(
                              GetWindow(owner),
                              message,
                              TitlePart() + "Error",
                              MessageBoxButtons.YesNo,
                              MessageBoxIcon.Question,
                              MessageBoxDefaultButton.Button2);

            return result == DialogResult.Yes;

           
        }

        
    }

    public interface IApplicationTitleProvider
    {
        string GetTitle();
    }

    public static class MessageServiceExtensions
    {
        public static void ShowMessage(this IMessageService service, string message, IDialogView view = null)
        {
            if (service == null)
            {
                throw new ArgumentNullException("service");
            }
            service.ShowMessage(view, message);
        }

  
        public static void ShowWarning(this IMessageService service, string message, ILog log = null, IDialogView view= null)
        {
            if (service == null)
            {
                throw new ArgumentNullException("service");
            }

            if(log!= null)  log.Warn(message);

            service.ShowWarning(view, message);
        }

        public static void ShowFatalError(this IMessageService service, string message, ILog log = null, IDialogView view = null)
        {
            if (service == null)
            {
                throw new ArgumentNullException("service");
            }
            if (log != null)
            {
                log.Error(message);
            }
            service.ShowFatalError(view, message);
        }



        public static bool ShowYesNoQuestion(this IMessageService service, string message, IDialogView view = null)
        {
            if (service == null)
            {
                throw new ArgumentNullException("service");
            }
            return service.ShowYesNoQuestion(view, message);
        }

        public static void ShowFatalError(this IMessageService service, Exception exception, ILog log = null, IDialogView view = null)
        {
            if (service == null)
            {
                throw new ArgumentNullException("service");
            }

            if (log != null)
            {
                log.Error("Fatal error occurred.", exception);
            }
            service.ShowFatalError(view, exception.ToString());


        }

        public static void ShowError(this IMessageService service, Exception exception, ILog log = null, IDialogView view = null)
        {
            if (service == null)
            {
                throw new ArgumentNullException("service");
            }
            if (log != null)
            {
                log.Error("Error occurred.", exception);
            }
            service.ShowError(view, exception.Message);
        }
        public static void ShowError(this IMessageService service, string message, ILog log = null, IDialogView view = null)
        {
            if (service == null)
            {
                throw new ArgumentNullException("service");
            }
            if (log != null)
            {
                log.Error(message);
            }
            service.ShowError(view, message);
        }
    }
}