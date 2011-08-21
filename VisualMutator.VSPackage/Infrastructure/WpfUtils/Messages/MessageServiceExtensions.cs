namespace PiotrTrzpil.VisualMutator_VSPackage.Infrastructure.WpfUtils.Messages
{
    #region Usings

    using System;

    #endregion

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

        public static void ShowError(this IMessageService service, string message)
        {
            if (service == null)
            {
                throw new ArgumentNullException("service");
            }
            service.ShowError(null, message);
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

        public static void ShowError(this IMessageService service, Exception exception)
        {
            if (service == null)
            {
                throw new ArgumentNullException("service");
            }
            service.ShowError(null, exception);
        }
    }
}