namespace CommonUtilityInfrastructure.WpfUtils
{
    #region Usings

    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Forms;

    using log4net;

    using MessageBox = System.Windows.MessageBox;
    using MessageBoxOptions = System.Windows.MessageBoxOptions;

    #endregion
       public interface IMessageService
    {
        void ShowMessage(object owner, string message);

        void ShowWarning(object owner, string message);

        void ShowFatalError(object owner, string message);

        bool? ShowQuestion(object owner, string message);

        bool ShowYesNoQuestion(object owner, string message);

           void ShowError(object owner, string message);
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

        private static MessageBoxResult MessageBoxResult
        {
            get
            {
                return MessageBoxResult.None;
            }
        }

        private static MessageBoxOptions MessageBoxOptions
        {
            get
            {
                return (CultureInfo.CurrentUICulture.TextInfo.IsRightToLeft)
                           ? MessageBoxOptions.RtlReading
                           : MessageBoxOptions.None;
            }
        }

        public void ShowMessage(object owner, string message)
        {
            var ownerWindow = owner as IWin32Window ?? _provider.GetWindow();

            System.Windows.Forms.MessageBox.Show(
                   ownerWindow,
                   message,
                   TitlePart() + "Information",
                   MessageBoxButtons.OK,
                   MessageBoxIcon.Information);

        }

        public void ShowWarning(object owner, string message)
        {
            var ownerWindow = owner as IWin32Window ?? _provider.GetWindow();

            System.Windows.Forms.MessageBox.Show(
                   ownerWindow,
                   message,
                   TitlePart() + "Warning",
                   MessageBoxButtons.OK,
                   MessageBoxIcon.Exclamation);
        }

        private string TitlePart()
        {
            return _titleProvider.GetTitle()+" - ";
        }
        public void ShowFatalError(object owner, string message)
        {
            var ownerWindow = owner as IWin32Window ?? _provider.GetWindow();

            System.Windows.Forms.MessageBox.Show(
                              ownerWindow,
                              message,
                              TitlePart()+"Error",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Error);

        }
        public void ShowError(object owner, string message)
        {
            var ownerWindow = owner as Window;
            if (ownerWindow != null)
            {
                MessageBox.Show(
                    ownerWindow,
                    message,
                    "",
                    MessageBoxButton.OK,
                    MessageBoxImage.Exclamation,
                    MessageBoxResult,
                    MessageBoxOptions);
            }
            else
            {
                MessageBox.Show(
                    message,
                    "",
                    MessageBoxButton.OK,
                    MessageBoxImage.Exclamation,
                    MessageBoxResult,
                    MessageBoxOptions);
            }
        }
        public bool? ShowQuestion(object owner, string message)
        {
            var ownerWindow = owner as Window;
            MessageBoxResult result;
            if (ownerWindow != null)
            {
                result = MessageBox.Show(
                    ownerWindow,
                    message,
                    "",
                    MessageBoxButton.YesNoCancel,
                    MessageBoxImage.Question,
                    MessageBoxResult.Cancel,
                    MessageBoxOptions);
            }
            else
            {
                result = MessageBox.Show(
                    message,
                    "",
                    MessageBoxButton.YesNoCancel,
                    MessageBoxImage.Question,
                    MessageBoxResult.Cancel,
                    MessageBoxOptions);
            }

            if (result == MessageBoxResult.Yes)
            {
                return true;
            }
            else if (result == MessageBoxResult.No)
            {
                return false;
            }

            return null;
        }

        public bool ShowYesNoQuestion(object owner, string message)
        {
            var ownerWindow = owner as Window;
            MessageBoxResult result;
            if (ownerWindow != null)
            {
                result = MessageBox.Show(
                    ownerWindow,
                    message,
                    "",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question,
                    MessageBoxResult.No,
                    MessageBoxOptions);
            }
            else
            {
                result = MessageBox.Show(
                    message,
                    "",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question,
                    MessageBoxResult.No,
                    MessageBoxOptions);
            }

            return result == MessageBoxResult.Yes;
        }

        
    }

    public interface IApplicationTitleProvider
    {
        string GetTitle();
    }

    public static class MessageServiceExtensions
    {
        public static void ShowMessage(this IMessageService service, string message)
        {
            if (service == null)
            {
                throw new ArgumentNullException("service");
            }
            service.ShowMessage(null, message);
        }

        public static void ShowWarning(this IMessageService service, string message)
        {
            if (service == null)
            {
                throw new ArgumentNullException("service");
            }
            service.ShowWarning(null, message);
        }
        public static void ShowWarning(this IMessageService service, string message, ILog log)
        {
            if (service == null)
            {
                throw new ArgumentNullException("service");
            }
            log.Warn(message);

            service.ShowWarning(null, message);
        }

        public static void ShowFatalError(this IMessageService service, string message, ILog log)
        {
            if (service == null)
            {
                throw new ArgumentNullException("service");
            }
            log.Error(message);
            service.ShowFatalError(null, message);
        }

        public static bool? ShowQuestion(this IMessageService service, string message)
        {
            if (service == null)
            {
                throw new ArgumentNullException("service");
            }
            return service.ShowQuestion(null, message);
        }

        public static bool ShowYesNoQuestion(this IMessageService service, string message)
        {
            if (service == null)
            {
                throw new ArgumentNullException("service");
            }
            return service.ShowYesNoQuestion(null, message);
        }

        public static void ShowFatalError(this IMessageService service, Exception exception, ILog log)
        {
            if (service == null)
            {
                throw new ArgumentNullException("service");
            }


            log.Error("Fatal error occurred.", exception);
            service.ShowFatalError(null, exception.ToString());


        }
        public static void ShowError(this IMessageService service, string message)
        {
            if (service == null)
            {
                throw new ArgumentNullException("service");
            }
            service.ShowError(null, message);
        }
        public static void ShowError(this IMessageService service, Exception exception, ILog log)
        {
            if (service == null)
            {
                throw new ArgumentNullException("service");
            }
            log.Error("Error occurred.", exception);
            service.ShowError(null, exception.Message);
        }
        public static void ShowError(this IMessageService service, string message, ILog log)
        {
            if (service == null)
            {
                throw new ArgumentNullException("service");
            }
            log.Error(message);
            service.ShowError(null, message);
        }
    }
}