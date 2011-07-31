namespace VisualMutator.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows.Controls;

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