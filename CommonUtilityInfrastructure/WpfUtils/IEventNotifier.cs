

namespace CommonUtilityInfrastructure.WpfUtils
{
    using System.ComponentModel;

    public interface IEventNotifier : INotifyPropertyChanged
    {
        EventListeners EventListeners { get; }
    }
}
