using System;
using System.Collections;
using System.Collections.Generic;

namespace KoiCatalog.Util
{
    [Serializable]
    internal class AutoSizingList<T> : IList<T>
    {
        private List<T> Items { get; } = new List<T>();

        public int IndexOf(T item) => Items.IndexOf(item);
        public void Insert(int index, T item) => Items.Insert(index, item);
        public void RemoveAt(int index) => Items.RemoveAt(index);
        public void Add(T item) => Items.Add(item);
        public void Clear() => Items.Clear();

        public T this[int index]
        {
            get
            {
                if (index < 0) throw new IndexOutOfRangeException();
                return index < Items.Count ? Items[index] : default(T);
            }

            set
            {
                if (index < 0) throw new IndexOutOfRangeException();
                while (Items.Count <= index)
                    Items.Add(default(T));
                Items[index] = value;
            }
        }

        public IEnumerator<T> GetEnumerator() => Items.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public bool Contains(T item) => Items.Contains(item);

        public void CopyTo(T[] array, int arrayIndex) => Items.CopyTo(array, arrayIndex);
        public bool Remove(T item) => Items.Remove(item);

        public int Count => Items.Count;
        public bool IsReadOnly => ((ICollection<T>)Items).IsReadOnly;
    }
}
