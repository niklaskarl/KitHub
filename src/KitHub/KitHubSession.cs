// -----------------------------------------------------------------------
// <copyright file="KitHubSession.cs" company="Niklas Karl">
// Copyright (c) Niklas Karl. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using KitHub.Client;
using Newtonsoft.Json;

namespace KitHub
{
    /// <summary>
    /// A delegate to dispatch notifications to a UI thread.
    /// </summary>
    /// <param name="action">The action to perform on the UI thread.</param>
    /// <returns>A <see cref="Task{User}"/> representing the asynchronous operation.</returns>
    public delegate Task NotificationDispatcher(Action action);

    /// <summary>
    /// Represents a session with the GitHub Api.
    /// This is the entry point to the KitHub library.
    /// </summary>
    public class KitHubSession
    {
        internal static readonly JsonSerializer Serializer = new JsonSerializer();

        private NotificationDispatcher _dispatcher;

        /// <summary>
        /// Initializes a new instance of the <see cref="KitHubSession"/> class.
        /// The session will not be authenticated.
        /// </summary>
        public KitHubSession()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="KitHubSession"/> class.
        /// The session will be authenticated with the provided access token.
        /// </summary>
        /// <param name="accessToken">The access token from which to authenticate the session.</param>
        public KitHubSession(string accessToken)
            : this(accessToken, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="KitHubSession"/> class.
        /// The session will be authenticated with the provided access token and all <see cref="INotifyPropertyChanged"/>
        /// notifications will be dispatched with the given dispatcher.
        /// </summary>
        /// <param name="accessToken">The access token from which to authenticate the session.</param>
        /// <param name="dispatcher">The <see cref="NotificationDispatcher"/> to dispatch notifications to a UI thread.</param>
        public KitHubSession(string accessToken, NotificationDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
            Client = new KitHubClient(accessToken);
        }

        internal KitHubClient Client { get; }

        /// <summary>
        /// Gets the user authenticated with this session.
        /// </summary>
        /// <param name="cancellationToken">An optional <see cref="CancellationToken"/> to cancel the operation.</param>
        /// <returns>A <see cref="Task{User}"/> representing the asynchronous operation.</returns>
        public Task<User> GetAuthenticatedUserAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return User.GetAuthenticatedUserAsync(this, cancellationToken);
        }

        /// <summary>
        /// Gets a user by his login name.
        /// </summary>
        /// <param name="login">The login name of the user to get.</param>
        /// <param name="cancellationToken">An optional <see cref="CancellationToken"/> to cancel the operation.</param>
        /// <returns>A <see cref="Task{User}"/> representing the asynchronous operation.</returns>
        public Task<User> GetUserAsync(string login, CancellationToken cancellationToken = default(CancellationToken))
        {
            return User.GetUserAsync(this, login, cancellationToken);
        }

        /// <summary>
        /// Gets the public events of all GitHub in a paged list.
        /// </summary>
        /// <param name="cancellationToken">An optional <see cref="CancellationToken"/> to cancel the operation.</param>
        /// <returns>A <see cref="Task{PagedActivityList}"/> representing the asynchronous operation.</returns>
        public Task<PagedEventList> GetPublicEventsAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return PagedEventList.CreateAsync(this, new Uri("/events", UriKind.Relative), cancellationToken);
        }

        internal Task DispatchAsync(Action action)
        {
            if (_dispatcher != null)
            {
                return _dispatcher(action);
            }

            action();
            return Task.FromResult((object)null);
        }
    }
}
