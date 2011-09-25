namespace CommonUtilityInfrastructure.WpfUtils.Messages
{
    #region Usings

    using System;
    using System.Globalization;
    using System.Windows;

    #endregion

    public class MessageService : IMessageService
    {
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
            var ownerWindow = owner as Window;
            if (ownerWindow != null)
            {
                MessageBox.Show(
                    ownerWindow,
                    message,
                    "",
                    MessageBoxButton.OK,
                    MessageBoxImage.None,
                    MessageBoxResult,
                    MessageBoxOptions);
            }
            else
            {
                MessageBox.Show(
                    message,
                    "",
                    MessageBoxButton.OK,
                    MessageBoxImage.None,
                    MessageBoxResult,
                    MessageBoxOptions);
            }
        }

        public void ShowWarning(object owner, string message)
        {
            var ownerWindow = owner as Window;
            if (ownerWindow != null)
            {
                MessageBox.Show(
                    ownerWindow,
                    message,
                    "",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning,
                    MessageBoxResult,
                    MessageBoxOptions);
            }
            else
            {
                MessageBox.Show(
                    message,
                    "",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning,
                    MessageBoxResult,
                    MessageBoxOptions);
            }
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
                    MessageBoxImage.Error,
                    MessageBoxResult,
                    MessageBoxOptions);
            }
            else
            {
                MessageBox.Show(
                    message,
                    "",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error,
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

        public void ShowError(object owner, Exception ex)
        {
            var ownerWindow = owner as Window;
            if (ownerWindow != null)
            {
                MessageBox.Show(
                    ownerWindow,
                    ex.ToString(),
                    "",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error,
                    MessageBoxResult,
                    MessageBoxOptions);
            }
            else
            {
                MessageBox.Show(
                    ex.ToString(),
                    "",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error,
                    MessageBoxResult,
                    MessageBoxOptions);
            }
        }

     
    }
}