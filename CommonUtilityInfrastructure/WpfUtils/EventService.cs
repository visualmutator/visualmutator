namespace CommonUtilityInfrastructure.WpfUtils
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    #endregion

    public interface IHandler
    {
    }

    public interface IHandler<in TMessage> : IHandler
    {
        void Handle(TMessage message);
    }

    public interface IEventService
    {
        void Subscribe(object instance);

        void Unsubscribe(object instance);

        void Publish(object message);
    }

    public class EventService : IEventService
    {
        private readonly List<Handler> _handlers;

        public EventService()
        {
            _handlers = new List<Handler>();
        }

        public virtual void Subscribe(object instance)
        {
            lock (_handlers)
            {
                if (_handlers.Any(x => x.Matches(instance)))
                {
                    return;
                }

                _handlers.Add(new Handler(instance));
            }
        }

        public virtual void Unsubscribe(object instance)
        {
            lock (_handlers)
            {
                var found = _handlers.FirstOrDefault(x => x.Matches(instance));

                if (found != null)
                {
                    _handlers.Remove(found);
                }
            }
        }

        public virtual void Publish(object message)
        {
            Handler[] toNotify;
            lock (_handlers)
            {
                toNotify = _handlers.ToArray();
            }

            var messageType = message.GetType();

            var dead = toNotify.Where(handler => !handler.Handle(messageType, message)).ToList();

            if (dead.Any())
            {
                lock (_handlers)
                {
                    foreach (Handler handler in dead)
                    {
                        _handlers.Remove(handler);
                    }
                }
            }
        }

        protected class Handler
        {
            private readonly WeakReference _reference;

            private readonly Dictionary<Type, MethodInfo> _supportedHandlers = new Dictionary<Type, MethodInfo>();

            public Handler(object handler)
            {
                _reference = new WeakReference(handler);

                var interfaces = handler.GetType().GetInterfaces()
                    .Where(x => typeof(IHandler).IsAssignableFrom(x) && x.IsGenericType);

                foreach (Type interfaceType in interfaces)
                {
                    var type = interfaceType.GetGenericArguments()[0];
                    var method = interfaceType.GetMethod("Handle");
                    _supportedHandlers[type] = method;
                }
            }

            public bool Matches(object instance)
            {
                return _reference.Target == instance;
            }

            public bool Handle(Type messageType, object message)
            {
                var target = _reference.Target;
                if (target == null)
                {
                    return false;
                }

                foreach (var pair in _supportedHandlers
                    .Where(pair => pair.Key.IsAssignableFrom(messageType)))
                {
                    pair.Value.Invoke(target, new[] { message });
                    return true;
                }

                return true;
            }
        }
    }
}