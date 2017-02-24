﻿// -----------------------------------------------------------------------
// <copyright file="KitHubSession.cs" company="Niklas Karl">
// Copyright (c) Niklas Karl. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;

namespace KitHub
{
    /// <summary>
    /// Represents a session with the GitHub Api.
    /// This is the entry point to the KitHub library.
    /// </summary>
    public class KitHubSession
    {
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
        {
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

        internal Task DispatchAsync(Action action)
        {
            action();
            return Task.CompletedTask;
        }
    }
}
