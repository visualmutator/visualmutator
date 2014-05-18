namespace VisualMutator.Infrastructure
{
    using System.Windows;
    using UsefulTools.Core;
    using Views;

    public interface IReporting
    {
        void LogWarning(string warn);
        void LogError(string err);
    }
    public class Reporting : IReporting
    {
        private readonly IMessageService _messageService;
        private readonly IWindow _view;

        public Reporting(IMessageService messageService, IView view)
        {
            _messageService = messageService;
            if (view is Window && view is IWindow)
            {
                _view = view as IWindow;
            }
        }

        public void LogWarning(string str)
        {
            _messageService.ShowWarning(str, _view);
        }

        public void LogError(string err)
        {
            _messageService.ShowError(err, _view);
        }
    }
}