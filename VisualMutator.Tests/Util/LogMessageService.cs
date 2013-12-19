namespace VisualMutator.Tests.Util
{
    using System;
    using System.Diagnostics;
    using UsefulTools.Core;

    public class LogMessageService : IMessageService
    {
        public void ShowMessage(IWindow owner, string message)
        {
            Console.WriteLine(message);
        }

        public void ShowWarning(IWindow owner, string message)
        {
            Console.WriteLine(message);
        }

        public void ShowFatalError(IWindow owner, string message)
        {
            Console.WriteLine(message);
        }

        public bool ShowYesNoQuestion(IWindow owner, string message)
        {
            Console.WriteLine(message);
            return true;
        }

        public void ShowError(IWindow owner, string message)
        {
            Console.WriteLine(message);
        }
    }
}