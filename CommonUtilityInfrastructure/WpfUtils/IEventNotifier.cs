

namespace PiotrTrzpil.VisualMutator_VSPackage.Infrastructure.WpfUtils
{

    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;


    public interface IEventNotifier : INotifyPropertyChanged
    {
        EventListeners EventListeners { get; }
    }
}
