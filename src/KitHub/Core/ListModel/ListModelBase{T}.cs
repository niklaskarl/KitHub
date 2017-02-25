// -----------------------------------------------------------------------
// <copyright file="ListModelBase{T}.cs" company="Niklas Karl">
// Copyright (c) Niklas Karl. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace KitHub
{
    /// <summary>
    /// Base class for all lists.
    /// </summary>
    /// <typeparam name="T">The type of the list items.</typeparam>
    public abstract class ListModelBase<T> : ListBase<T>
    {
        private SemaphoreSlim _sync;

        private bool _refreshing;

        private string _entityTag;

        private DateTime? _lastModified;

        internal ListModelBase(KitHubSession session)
            : base(session)
        {
            _sync = new SemaphoreSlim(1);
        }

        /// <summary>
        /// Gets the url of the api endpoint from which to refresh the list.
        /// </summary>
        protected abstract Uri RefreshUri { get; }

        /// <summary>
        /// Refreshes the list and reloads all items.
        /// </summary>
        /// <param name="cancellationToken">An optional <see cref="CancellationToken"/> to cancel the operation.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public Task RefreshAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.Run(() => RefreshInternalAsync(cancellationToken), cancellationToken);
        }

        private static IModelInitializer<T> CreateInitializer(ListModelAttribute attribute)
        {
            ConstructorInfo constructor = attribute.Initializer?.GetTypeInfo()?.GetConstructor(new Type[0]);
            return constructor?.Invoke(null) as IModelInitializer<T>;
        }

        private async Task RefreshInternalAsync(CancellationToken cancellationToken)
        {
            _refreshing = true;

            await _sync.WaitAsync(cancellationToken);
            try
            {
                if (_refreshing)
                {
                    KitHubRequest request = new KitHubRequest()
                    {
                        Uri = RefreshUri,
                        EntityTag = _entityTag,
                        LastModified = _lastModified
                    };

                    KitHubResponse response = await Session.Client.GetAsync(request, cancellationToken);

                    _entityTag = response.EntityTag;
                    _lastModified = response.LastModified;

                    if (response.HasChanged)
                    {
                        await Session.DispatchAsync(() => SetItemsFromData(response.Content));
                    }

                    _refreshing = false;
                }
            }
            finally
            {
                _sync.Release();
            }
        }

        private void SetItemsFromData(JToken data)
        {
            if (data is JArray array)
            {
                IReadOnlyList<T> items = null;

                TypeInfo type = GetType().GetTypeInfo();
                ListModelAttribute attribute = type.GetCustomAttribute<ListModelAttribute>(true);
                if (attribute != null)
                {
                    IModelInitializer<T> initializer = CreateInitializer(attribute);
                    if (initializer != null)
                    {
                        items = InitializeItemsFromData(array, initializer);
                    }
                }

                if (items == null)
                {
                    items = InitializeItemsFromData(array);
                }

                UpdateList(items);
            }
            else
            {
                throw new InvalidDataException();
            }
        }

        private IReadOnlyList<T> InitializeItemsFromData(JArray data)
        {
            List<T> items = new List<T>(data.Count);
            foreach (JToken item in data)
            {
                items.Add(item.ToObject<T>(KitHubSession.Serializer));
            }

            return items;
        }

        private IReadOnlyList<T> InitializeItemsFromData(JArray data, IModelInitializer<T> initializer)
        {
            List<T> items = new List<T>(data.Count);
            foreach (JToken item in data)
            {
                items.Add(initializer.InitializeModel(this, item));
            }

            return items;
        }

        private void UpdateList(IEnumerable<T> items)
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
