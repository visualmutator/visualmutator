namespace CommonUtilityInfrastructure.WpfUtils
{
    #region Usings

    using System.Collections.Generic;
    using System.Collections.Specialized;

    #endregion

    public class NotifyingCollection<T> : System.Collections.ObjectModel.ObservableCollection<T>
    {
        public NotifyingCollection()
        {
        }

        public NotifyingCollection(IEnumerable<T> collection)
            : base(collection)
        {
        }

   
        public void AddRange(IEnumerable<T> collection)
        {
            foreach (T i in collection)
            {
                Items.Add(i);
            }
            OnCollectionChanged(
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

    
        public void RemoveRange(IEnumerable<T> collection)
        {
            foreach (T i in collection)
            {
                Items.Remove(i);
            }
            OnCollectionChanged(
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public void Replace(T item)
        {
            ReplaceRange(new[] { item });
        }

   
        public void ReplaceRange(IEnumerable<T> collection)
        {
        
            Items.Clear();
            foreach (T i in collection)
            {
                Items.Add(i);
            }
            OnCollectionChanged(
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
    }
}