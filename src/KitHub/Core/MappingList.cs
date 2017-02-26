using System;
using System.Collections;
using System.Collections.Generic;

namespace KitHub.Core
{
    internal sealed class MappingList<TSource, TTarget> : IList<TTarget>, IReadOnlyList<TTarget>, IList
    {
        private IList<TSource> _source;

        private Func<TSource, TTarget> _map;

        internal MappingList(IList<TSource> source, Func<TSource, TTarget> map)
        {
            _source = source;
            _map = map;
        }

        /// <inheritdoc/>
        public int Count => _source.Count;

        /// <inheritdoc/>
        bool ICollection<TTarget>.IsReadOnly => true;

        /// <inheritdoc/>
        bool IList.IsReadOnly => true;

        /// <inheritdoc/>
        bool IList.IsFixedSize => false;

        /// <inheritdoc/>
        bool ICollection.IsSynchronized => false;

        /// <inheritdoc/>
        object ICollection.SyncRoot => this;

        /// <inheritdoc/>
        public TTarget this[int index]
        {
            get => _map(_source[index]);
            set => throw new NotSupportedException();
        }

        /// <inheritdoc/>
        object IList.this[int index]
        {
            get => _map(_source[index]);
            set => throw new NotSupportedException();
        }

        /// <inheritdoc/>
        void ICollection<TTarget>.Add(TTarget item)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        int IList.Add(object value)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        void ICollection<TTarget>.Clear()
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        void IList.Clear()
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public bool Contains(TTarget item)
        {
            for (int i = 0; i < Count; i++)
            {
                if (Equals(item, this[i]))
                {
                    return true;
                }
            }

            return false;
        }

        /// <inheritdoc/>
        public bool Contains(object value)
        {
            for(int i = 0; i < Count; i++)
            {
                if (Equals(value, this[i]))
                {
                    return true;
                }
            }

            return false;
        }

        /// <inheritdoc/>
        public void CopyTo(TTarget[] array, int arrayIndex)
        {
            for (int i = 0; i < Count; i++)
            {
                array[i + arrayIndex] = this[i];
            }
        }

        /// <inheritdoc/>
        void ICollection.CopyTo(Array array, int index)
        {
            CopyTo(array as TTarget[], index);
        }

        /// <inheritdoc/>
        public IEnumerator<TTarget> GetEnumerator()
        {
            for (int i = 0; i < Count; i++)
            {
                yield return this[i];
            }
        }

        /// <inheritdoc/>
        public int IndexOf(TTarget item)
        {
            for (int i = 0; i < Count; i++)
            {
                if (Equals(item, this[i]))
                {
                    return i;
                }
            }

            return -1;
        }

        /// <inheritdoc/>
        public int IndexOf(object value)
        {
            for (int i = 0; i < Count; i++)
            {
                if (Equals(value, this[i]))
                {
                    return i;
                }
            }

            return -1;
        }

        /// <inheritdoc/>
        void IList<TTarget>.Insert(int index, TTarget item)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        void IList.Insert(int index, object value)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        bool ICollection<TTarget>.Remove(TTarget item)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        void IList.Remove(object value)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        void IList<TTarget>.RemoveAt(int index)
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
    }
}
