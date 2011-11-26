namespace CommonUtilityInfrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using CommonUtilityInfrastructure.Threading;
    using CommonUtilityInfrastructure.WpfUtils;

    public class CommonServices
    {
        private readonly IMessageService _message;

        private readonly IEventService _event;

        private readonly IThreading _threading;

        private readonly ISettingsManager _settings;

        public CommonServices(
            IMessageService message,
            IEventService @event,
            IThreading threading,
            ISettingsManager settings)
        {
            _message = message;
            _event = @event;
            _threading = threading;
            _settings = settings;
        }

        public IMessageService Logging
        {
            get
            {
                return _message;
            }
        }

        public IEventService EventPassing
        {
            get
            {
                return _event;
            }
        }

        public IThreading Threading
        {
            get
            {
                return _threading;
            }
        }

        public ISettingsManager Settings
        {
            get
            {
                return _settings;
            }
        }
    }
}