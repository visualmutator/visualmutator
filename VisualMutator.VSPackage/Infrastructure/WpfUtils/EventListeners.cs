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
        private readonly ICollection<CollectionChangedEventListener> collectionListeners;

        private readonly ICollection<PropertyChangedEventListener> propertyListeners;

        public EventListeners()
        {
            propertyListeners = new List<PropertyChangedEventListener>();
            collectionListeners = new List<CollectionChangedEventListener>();
        }

        public void Add(INotifyPropertyChanged source, PropertyChangedEventHandler handler)
        {
            var listener = new PropertyChangedEventListener(source, handler);
            propertyListeners.Add(listener);
            PropertyChangedEventManager.AddListener(source, listener, "");
        }

        public void Remove(INotifyPropertyChanged source, PropertyChangedEventHandler handler)
        {
            PropertyChangedEventListener listener = propertyListeners
                .LastOrDefault(x => x.Source == source && x.Handler == handler);

            if (listener != null)
            {
                propertyListeners.Remove(listener);
                PropertyChangedEventManager.RemoveListener(source, listener, "");
            }
        }

        public void Add(
            INotifyCollectionChanged source, NotifyCollectionChangedEventHandler handler)
        {
            var listener = new CollectionChangedEventListener(source, handler);
            collectionListeners.Add(listener);
            CollectionChangedEventManager.AddListener(source, listener);
        }

        public void Remove(
            INotifyCollectionChanged source, NotifyCollectionChangedEventHandler handler)
        {
            CollectionChangedEventListener listener = collectionListeners
                .LastOrDefault(x => x.Source == source && x.Handler == handler);

            if (listener != null)
            {
                collectionListeners.Remove(listener);
                CollectionChangedEventManager.RemoveListener(source, listener);
            }
        }
    }

    internal class PropertyChangedEventListener : IWeakEventListener
    {
        private readonly PropertyChangedEventHandler handler;

        private readonly INotifyPropertyChanged source;

        public PropertyChangedEventListener(
            INotifyPropertyChanged source, PropertyChangedEventHandler handler)
        {
            this.source = source;
            this.handler = handler;
        }

        public INotifyPropertyChanged Source
        {
            get
            {
                return source;
            }
        }

        public PropertyChangedEventHandler Handler
        {
            get
            {
                return handler;
            }
        }

        public bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            handler(sender, (PropertyChangedEventArgs)e);
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