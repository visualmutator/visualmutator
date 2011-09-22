namespace PiotrTrzpil.VisualMutator_VSPackage.Infrastructure.WpfUtils
{
    public abstract class Controller
    {
        private readonly EventListeners _eventListeners;

        protected Controller()
        {
            _eventListeners = new EventListeners();
        }

        protected EventListeners EventListeners
        {
            get
            {
                return _eventListeners;
            }
        }
    }
}