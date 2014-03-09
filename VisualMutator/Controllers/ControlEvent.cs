namespace VisualMutator.Controllers
{
    public enum ControlEventType
    {
        Resume,
        Pause,
        Stop,
        SaveResults
    }
    public class ControlEvent
    {
        private readonly ControlEventType _type;

        public ControlEvent(ControlEventType type)
        {
            _type = type;
        }

        public ControlEventType Type
        {
            get { return _type; }
        }
    }
}