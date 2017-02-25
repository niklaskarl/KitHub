// -----------------------------------------------------------------------
// <copyright file="Page{T}.cs" company="Niklas Karl">
// Copyright (c) Niklas Karl. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace KitHub
{
    /// <summary>
    /// A page in a <see cref="PagedListModelBase{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the items in the page.</typeparam>
    public class Page<T> : ListBase<T>
    {
        private PagedListModelBase<T> _container;

        private Page(PagedListModelBase<T> container, int number, IEnumerable<T> items)
            : base(container.Session, items)
        {
            _container = container;
            Number = number;
        }

        internal Page(PagedListModelBase<T> container, int number)
            : base(container.Session)
        {
            _container = container;
            Number = number;
        }

        /// <summary>
        /// The page number.
        /// </summary>
        public int Number { get; }
        
        internal string EntityTag { get; set; }

        internal DateTime? LastModified { get; set; }

        /// <summary>
        /// Refreshes the page and reloads all items.
        /// </summary>
        /// <param name="cancellationToken">An optional <see cref="CancellationToken"/> to cancel the operation.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public Task RefreshAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return _container.RefreshPageAsync(this, cancellationToken);
        }

        internal Page<T> Clone()
        {
            return new Page<T>(_container, Number, this)
            {
                EntityTag = EntityTag,
                LastModified = LastModified
            };
        }
        
        internal void UpdateList(IEnumerable<T> items)
        {
            int i = 0;
            foreach (T item in items)
            {
                bool found = false;
                for (int j = i; j < Count && !found; j++)
                {
                    if (Equals(this[j], item))
                    {
                        if (j > i)
                        {
                            RemoveRangeInternal(i, j - i);
                        }

                        found = true;
                    }
                }

                // clear tail as it no longer exists and add item to the end of the list
                if (!found)
                {
                    if (Count > i)
                    {
                        if (i == 0)
                        {
                            ClearInternal();
                        }
                        else
                        {
                            while (i < Count + 1)
                            {
                                RemoveAtInternal(Count - 1);
                            }
                        }
                    }

                    AddInternal(item);
                }

                i++;
            }
        }
    }
}
