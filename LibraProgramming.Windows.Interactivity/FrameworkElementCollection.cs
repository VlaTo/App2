using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Windows.UI.Xaml;

namespace LibraProgramming.Windows.Interactivity
{
    public class FrameworkElementCollection<T> : FrameworkElement, IList<T>, INotifyCollectionChanged where T : FrameworkElement
    {
        private readonly ObservableCollection<T> objects;

        public int Count => objects.Count;

        public bool IsReadOnly => false;

        public event NotifyCollectionChangedEventHandler CollectionChanged
        {
            add => objects.CollectionChanged += value;
            remove => objects.CollectionChanged -= value;
        }

        public T this[int index]
        {
            get => objects[index];
            set => objects[index] = value;
        }

        protected FrameworkElementCollection()
        {
            objects = new ObservableCollection<T>();
        }

        public void Add(T item) => objects.Add(item);

        public void Clear() => objects.Clear();

        public bool Contains(T item) => objects.Contains(item);

        public void CopyTo(T[] array, int index) => objects.CopyTo(array, index);

        public IEnumerator<T> GetEnumerator() => objects.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public bool Remove(T item) => objects.Remove(item);

        public int IndexOf(T item) => objects.IndexOf(item);

        public void Insert(int index, T item) => objects.Insert(index, item);

        public void RemoveAt(int index) => objects.RemoveAt(index);
    }
}
