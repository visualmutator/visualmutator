namespace VisualMutator.Console
{
    using System.Linq;
    using UsefulTools.Core;

    public class ConsoleMessageService : IMessageService
    {
        public void ShowMessage(IWindow owner, string message)
        {
            System.Console.WriteLine("=================== MESSAGE ====================");
            System.Console.WriteLine(message);
            System.Console.WriteLine("================================================");
        }

        public void ShowWarning(IWindow owner, string message)
        {
            System.Console.WriteLine("=================== WARNING ====================");
            System.Console.WriteLine(message);
            System.Console.WriteLine("================================================");
        }

        public void ShowFatalError(IWindow owner, string message)
        {
            System.Console.WriteLine("=================== FATAL ======================");
            System.Console.WriteLine(message);
            System.Console.WriteLine("================================================");
        }

        public bool ShowYesNoQuestion(IWindow owner, string message)
        {
            System.Console.WriteLine("=================== QUESTION ===================");
            System.Console.WriteLine(message);
            System.Console.WriteLine("================================================");
            System.Console.Write("Y/N? ");
            string input;
            while(!new[] {"Y", "N"}.Contains(input = System.Console.ReadLine()) )
            {
                System.Console.Write("Y/N? ");
            }
            return input == "Y";
        }

        public void ShowError(IWindow owner, string message)
        {
            System.Console.WriteLine("==================== ERROR =====================");
            System.Console.WriteLine(message);
            System.Console.WriteLine("================================================");
        }
    }
}