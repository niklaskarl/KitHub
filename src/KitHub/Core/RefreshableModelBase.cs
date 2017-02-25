// -----------------------------------------------------------------------
// <copyright file="RefreshableModelBase.cs" company="Niklas Karl">
// Copyright (c) Niklas Karl. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using KitHub.Client;

namespace KitHub.Core
{
    /// <summary>
    /// The base class of all entities.
    /// </summary>
    public abstract class RefreshableModelBase : SerializableModelBase
    {
        private IDictionary<string, object> _properties;

        private SemaphoreSlim _sync;

        private bool _refreshing;

        private string _entityTag;

        private DateTime? _lastModified;

        internal RefreshableModelBase(KitHubSession session)
            : base(session)
        {
            _properties = new Dictionary<string, object>();

            _sync = new SemaphoreSlim(1);
        }

        /// <summary>
        /// Gets the url of the GitHub API endpoint from which to refresh the entity.
        /// </summary>
        protected abstract Uri Uri { get; }

        /// <summary>
        /// Refreshes the properties of the entity from the GitHub Api.
        /// </summary>
        /// <param name="cancellationToken">An optional <see cref="CancellationToken"/> to cancel the operation.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public Task RefreshAsync(CancellationToken cancellationToken)
        {
            return Task.Run(() => RefreshInternalAsync(cancellationToken), cancellationToken);
        }

        internal void SetFromResponse(KitHubResponse response)
        {
            _entityTag = response.EntityTag;
            _lastModified = response.LastModified;

            if (response.HasChanged)
            {
                SetFromData(response.Content);
            }
        }

        /// <summary>
        /// Gets a property from the property store and checks whether it has already been loaded.
        /// </summary>
        /// <param name="propertyName">The name of the property to get.</param>
        /// <returns>The value of the property.</returns>
        protected object GetProperty([CallerMemberName]string propertyName = null)
        {
            if (_properties.TryGetValue(propertyName, out object value))
            {
                return value;
            }
            else
            {
                RefreshAsync(CancellationToken.None);
                return null;
            }
        }

        /// <summary>
        /// Saves a property to the property store, marks it as loaded and raises a PropertyChanged event.
        /// </summary>
        /// <param name="value">The value of the property.</param>
        /// <param name="propertyName">The name of the property</param>
        protected void SetProperty(object value, [CallerMemberName]string propertyName = null)
        {
            if (_properties.TryGetValue(propertyName, out object previous))
            {
                if (!Equals(previous, value))
                {
                    _properties[propertyName] = value;
                    OnPropertyChanged(propertyName);
                }
            }
            else
            {
                _properties[propertyName] = value;
                OnPropertyChanged(propertyName);
            }
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
                        Uri = Uri,
                        EntityTag = _entityTag,
                        LastModified = _lastModified
                    };

                    KitHubResponse response = await Session.Client.GetAsync(request, cancellationToken);
                    if (response.HasChanged)
                    {
                        await Session.DispatchAsync(() => SetFromResponse(response));
                    }

                    _refreshing = false;
                }
            }
            finally
            {
                _sync.Release();
            }
        }
    }
}
