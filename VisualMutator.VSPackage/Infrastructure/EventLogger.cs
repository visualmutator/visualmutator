namespace PiotrTrzpil.VisualMutator_VSPackage.Infrastructure
{
    #region Usings

    using System.Windows.Controls;

    #endregion

    public interface IEventLogger
    {
        void Log(string message);

        void Error(string message);
    }

    public class EventLogger : IEventLogger
    {
        private readonly RichTextBox _control;

        public EventLogger(RichTextBox control)
        {
            _control = control;
        }

        public void Log(string message)
        {
            _control.AppendText(message);
        }

        public void Error(string message)
        {
            _control.AppendText(message);
        }
    }
}