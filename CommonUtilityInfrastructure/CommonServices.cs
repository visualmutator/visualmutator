namespace CommonUtilityInfrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using CommonUtilityInfrastructure.FileSystem;
    using CommonUtilityInfrastructure.Threading;
    using CommonUtilityInfrastructure.WpfUtils;

    public class CommonServices
    {
        private readonly IMessageService _message;

        private readonly IEventService _event;

        private readonly IThreading _threading;

        private readonly ISettingsManager _settings;

        private readonly IFileSystem _fileSystem;

        public CommonServices(
            IMessageService message,
            IEventService @event,
            IThreading threading,
            ISettingsManager settings,
            IFileSystem fileSystem)
        {
            _message = message;
            _event = @event;
            _threading = threading;
            _settings = settings;
            _fileSystem = fileSystem;
        }
        public IFileSystem FileSystem
        {
            get
            {
                return _fileSystem;
            }
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