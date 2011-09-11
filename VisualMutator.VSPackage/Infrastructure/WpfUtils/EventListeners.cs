namespace PiotrTrzpil.VisualMutator_VSPackage.Infrastructure.WpfUtils
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows;

    #endregion

    public class EventListeners
    {
        private readonly ICollection<CollectionChangedEventListener> _collectionListeners;

        private readonly ICollection<PropertyChangedEventListener> _propertyListeners;

        public EventListeners()
        {
            _propertyListeners = new List<PropertyChangedEventListener>();
            _collectionListeners = new List<CollectionChangedEventListener>();
        }

        public void Add(INotifyPropertyChanged source, PropertyChangedEventHandler handler)
        {
            var listener = new PropertyChangedEventListener(source, handler);
            _propertyListeners.Add(listener);
            PropertyChangedEventManager.AddListener(source, listener, "");
        }

        public void Add(INotifyPropertyChanged source, string name, Action action)
        {
            var listener = new PropertyChangedEventListener(source, (sender, args) => action());
            _propertyListeners.Add(listener);
            PropertyChangedEventManager.AddListener(source, listener, name);
        }
        public void AddCollectionChangedEventHandler(INotifyCollectionChanged source,  Action action)
        {
            var handler = new NotifyCollectionChangedEventHandler((sender, args) => action());
            Add(source, handler);
        }
        public void Remove(INotifyPropertyChanged source, PropertyChangedEventHandler handler)
        {
            PropertyChangedEventListener listener = _propertyListeners
                .LastOrDefault(x => x.Source == source && x.Handler == handler);

            if (listener != null)
            {
                _propertyListeners.Remove(listener);
                PropertyChangedEventManager.RemoveListener(source, listener, "");
            }
        }

        public void Add(
            INotifyCollectionChanged source, NotifyCollectionChangedEventHandler handler)
        {
            var listener = new CollectionChangedEventListener(source, handler);
            _collectionListeners.Add(listener);
            CollectionChangedEventManager.AddListener(source, listener);
        }

        public void Remove(
            INotifyCollectionChanged source, NotifyCollectionChangedEventHandler handler)
        {
            CollectionChangedEventListener listener = _collectionListeners
                .LastOrDefault(x => x.Source == source && x.Handler == handler);

            if (listener != null)
            {
                _collectionListeners.Remove(listener);
                CollectionChangedEventManager.RemoveListener(source, listener);
            }
        }
    }

    internal class PropertyChangedEventListener : IWeakEventListener
    {
        private readonly PropertyChangedEventHandler _handler;

        private readonly INotifyPropertyChanged _source;

        public PropertyChangedEventListener(
            INotifyPropertyChanged source, PropertyChangedEventHandler handler)
        {
            this._source = source;
            this._handler = handler;
        }

        public INotifyPropertyChanged Source
        {
            get
            {
                return _source;
            }
        }

        public PropertyChangedEventHandler Handler
        {
            get
            {
                return _handler;
            }
        }

        public bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            _handler(sender, (PropertyChangedEventArgs)e);
            return true;
        }
    }

    internal class CollectionChangedEventListener : IWeakEventListener
    {
        private readonly NotifyCollectionChangedEventHandler handler;

        private readonly INotifyCollectionChanged source;

        public CollectionChangedEventListener(
            INotifyCollectionChanged source, NotifyCollectionChangedEventHandler handler)
        {
            this.source = source;
            this.handler = handler;
        }

        public INotifyCollectionChanged Source
        {
            get
            {
                return source;
            }
        }

        public NotifyCollectionChangedEventHandler Handler
        {
            get
            {
                return handler;
            }
        }

        public bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            handler(sender, (NotifyCollectionChangedEventArgs)e);
            return true;
        }
    }
}