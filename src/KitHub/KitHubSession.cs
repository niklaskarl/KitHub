// <copyright file="KitHubSession.cs" company="Niklas Karl">
// Copyright (c) Niklas Karl. All rights reserved.
// </copyright>

using System;
using System.Threading;
using System.Threading.Tasks;

namespace KitHub
{
    public class KitHubSession
    {
        public KitHubSession()
            : this(null)
        {
        }

        public KitHubSession(string token)
        {
            Client = new KitHubClient(token);
        }

        internal KitHubClient Client { get; }

        public Task<User> GetAuthenticatedUserAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return User.GetAuthenticatedUserAsync(this, cancellationToken);
        }

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
