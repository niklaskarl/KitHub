// -----------------------------------------------------------------------
// <copyright file="ListBase{T}.cs" company="Niklas Karl">
// Copyright (c) Niklas Karl. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace KitHub
{
    public abstract class ListBase<T> : BindableBase, IList<T>, IReadOnlyList<T>, IList, INotifyPropertyChanged, INotifyCollectionChanged
    {
        private List<T> _list;

        internal ListBase()
        {
            _list = new List<T>();
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public int Count => _list.Count;

        bool ICollection<T>.IsReadOnly => true;

        bool IList.IsReadOnly => true;

        bool IList.IsFixedSize => false;

        bool ICollection.IsSynchronized => false;

        object ICollection.SyncRoot => this;

        public T this[int index]
        {
            get => _list[index];
            set => throw new NotSupportedException();
        }

        object IList.this[int index]
        {
            get => _list[index];
            set => throw new NotSupportedException();
        }

        void ICollection<T>.Add(T item)
        {
            throw new NotSupportedException();
        }

        int IList.Add(object value)
        {
            throw new NotSupportedException();
        }

        void ICollection<T>.Clear()
        {
            throw new NotSupportedException();
        }

        void IList.Clear()
        {
            throw new NotSupportedException();
        }

        public bool Contains(T item)
        {
            return _list.Contains(item);
        }

        public bool Contains(object value)
        {
            return (_list as IList).Contains(value);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        public void CopyTo(Array array, int index)
        {
            (_list as IList).CopyTo(array, index);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        public int IndexOf(T item)
        {
            return _list.IndexOf(item);
        }

        public int IndexOf(object value)
        {
            return (_list as IList).IndexOf(value);
        }

        void IList<T>.Insert(int index, T item)
        {
            throw new NotSupportedException();
        }

        void IList.Insert(int index, object value)
        {
            throw new NotSupportedException();
        }

        bool ICollection<T>.Remove(T item)
        {
            throw new NotSupportedException();
        }

        void IList.Remove(object value)
        {
            throw new NotSupportedException();
        }

        void IList<T>.RemoveAt(int index)
        {
            throw new NotSupportedException();
        }

        void IList.RemoveAt(int index)
        {
            throw new NotSupportedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        protected void AddRangeInternal(IEnumerable<T> items)
        {
            foreach (T item in items)
            {
                AddInternal(item);
            }
        }

        protected void AddInternal(T item)
        {
            _list.Add(item);
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, _list.Count - 1));
        }

        protected void InsertInternal(int index, T item)
        {
            _list.Insert(index, item);
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
        }

        protected void RemoveAtInternal(int index)
        {
            T item = this[index];
            _list.RemoveAt(index);
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index));
        }

        protected void RemoveRangeInternal(int index, int count)
        {
            T[] items = _list.Skip(index).Take(count).ToArray();
            _list.RemoveRange(index, count);
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, items, index));
        }

        protected void ClearInternal()
        {
            _list.Clear();
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
    }
}
