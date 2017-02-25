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

namespace KitHub.Core
{
    /// <summary>
    /// A readonly implementation of <see cref="IList{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the list elements</typeparam>
    public abstract class ListBase<T> : BindableBase, IList<T>, IReadOnlyList<T>, IList, INotifyPropertyChanged, INotifyCollectionChanged
    {
        private List<T> _list;

        internal ListBase(KitHubSession session)
            : base(session)
        {
            _list = new List<T>();
        }

        internal ListBase(KitHubSession session, IEnumerable<T> items)
            : base(session)
        {
            _list = new List<T>(items);
        }

        /// <inheritdoc/>
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        /// <inheritdoc/>
        public int Count => _list.Count;

        /// <inheritdoc/>
        bool ICollection<T>.IsReadOnly => true;

        /// <inheritdoc/>
        bool IList.IsReadOnly => true;

        /// <inheritdoc/>
        bool IList.IsFixedSize => false;

        /// <inheritdoc/>
        bool ICollection.IsSynchronized => false;

        /// <inheritdoc/>
        object ICollection.SyncRoot => this;

        /// <inheritdoc/>
        public T this[int index]
        {
            get => _list[index];
            set => throw new NotSupportedException();
        }

        /// <inheritdoc/>
        object IList.this[int index]
        {
            get => _list[index];
            set => throw new NotSupportedException();
        }

        /// <inheritdoc/>
        void ICollection<T>.Add(T item)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        int IList.Add(object value)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        void ICollection<T>.Clear()
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        void IList.Clear()
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public bool Contains(T item)
        {
            return _list.Contains(item);
        }

        /// <inheritdoc/>
        public bool Contains(object value)
        {
            return (_list as IList).Contains(value);
        }

        /// <inheritdoc/>
        public void CopyTo(T[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        /// <inheritdoc/>
        public void CopyTo(Array array, int index)
        {
            (_list as IList).CopyTo(array, index);
        }

        /// <inheritdoc/>
        public IEnumerator<T> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        /// <inheritdoc/>
        public int IndexOf(T item)
        {
            return _list.IndexOf(item);
        }

        /// <inheritdoc/>
        public int IndexOf(object value)
        {
            return (_list as IList).IndexOf(value);
        }

        /// <inheritdoc/>
        void IList<T>.Insert(int index, T item)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        void IList.Insert(int index, object value)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        bool ICollection<T>.Remove(T item)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        void IList.Remove(object value)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        void IList<T>.RemoveAt(int index)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        void IList.RemoveAt(int index)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Adds a range of items to the list.
        /// </summary>
        /// <param name="items">The items to add to the list.</param>
        protected void AddRangeInternal(IEnumerable<T> items)
        {
            foreach (T item in items)
            {
                AddInternal(item);
            }
        }

        /// <summary>
        /// Adds an item to the list.
        /// </summary>
        /// <param name="item">The item to add to the list.</param>
        protected void AddInternal(T item)
        {
            _list.Add(item);
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, _list.Count - 1));
        }

        /// <summary>
        /// Inserts an item into the list.
        /// </summary>
        /// <param name="index">The index at which to insert the item.</param>
        /// <param name="item">The item to insert into the list.</param>
        protected void InsertInternal(int index, T item)
        {
            _list.Insert(index, item);
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
        }

        /// <summary>
        /// Removes an item from the list at a given position.
        /// </summary>
        /// <param name="index">The index of the item to remove.</param>
        protected void RemoveAtInternal(int index)
        {
            T item = this[index];
            _list.RemoveAt(index);
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index));
        }

        /// <summary>
        /// Remove a range of items from the list.
        /// </summary>
        /// <param name="index">The index of the first item to remove.</param>
        /// <param name="count">The number of items to remove.</param>
        protected void RemoveRangeInternal(int index, int count)
        {
            T[] items = _list.Skip(index).Take(count).ToArray();
            _list.RemoveRange(index, count);
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, items, index));
        }

        /// <summary>
        /// Clears all items from the list.
        /// </summary>
        protected void ClearInternal()
        {
            _list.Clear();
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
    }
}
