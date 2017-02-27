// -----------------------------------------------------------------------
// <copyright file="User.cs" company="Niklas Karl">
// Copyright (c) Niklas Karl. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using KitHub.Client;
using KitHub.Core;
using Newtonsoft.Json.Linq;

namespace KitHub
{
    /// <summary>
    /// A GitHub user.
    /// </summary>
    public sealed class User : RefreshableModelBase
    {
        private static readonly IDictionary<UserKey, User> Cache = new Dictionary<UserKey, User>();

        private UserKey _key;

        private RepositoryList _repositories;

        private UserList _followers;

        private UserList _following;

        private User(UserKey key)
            : base(key.Session)
        {
            _key = key;
        }

        /// <summary>
        /// Gets the login name of the user.
        /// </summary>
        public string Login => _key.Login;

        /// <summary>
        /// Gets the id of the user.
        /// </summary>
        [ModelProperty("id")]
        public long? Id
        {
            get => GetProperty() as long?;
            internal set => SetProperty(value);
        }

        /// <summary>
        /// Gets the name of the user.
        /// </summary>
        [ModelProperty("name")]
        public string Name
        {
            get => GetProperty() as string;
            private set => SetProperty(value);
        }

        /// <summary>
        /// Gets the company of the user.
        /// </summary>
        [ModelProperty("company")]
        public string Company
        {
            get => GetProperty() as string;
            private set => SetProperty(value);
        }

        /// <summary>
        /// Gets the location of the user.
        /// </summary>
        [ModelProperty("location")]
        public string Location
        {
            get => GetProperty() as string;
            private set => SetProperty(value);
        }

        /// <summary>
        /// Gets the email address of the user.
        /// If the email address is not publicly visible, this property is null.
        /// </summary>
        [ModelProperty("email")]
        public string Email
        {
            get => GetProperty() as string;
            private set => SetProperty(value);
        }

        /// <summary>
        /// Gets a value indicating, whether the user is hireable or not.
        /// </summary>
        [ModelProperty("hireable")]
        public bool? IsHireable
        {
            get => GetProperty() as bool?;
            private set => SetProperty(value);
        }

        /// <summary>
        /// Gets the bio of the user.
        /// </summary>
        [ModelProperty("bio")]
        public string Bio
        {
            get => GetProperty() as string;
            private set => SetProperty(value);
        }

        /// <summary>
        /// Gets the timestamp at which the user profile was created.
        /// </summary>
        [ModelProperty("created_at")]
        public DateTimeOffset? CreatedAt
        {
            get => GetProperty() as DateTime?;
            private set => SetProperty(value);
        }

        /// <summary>
        /// Gets the timestamp at which the user was last updated.
        /// </summary>
        [ModelProperty("updated_at")]
        public DateTimeOffset? UpdatedAt
        {
            get => GetProperty() as DateTime?;
            private set => SetProperty(value);
        }

        /// <summary>
        /// Gets the url of the user's avatar image.
        /// </summary>
        [ModelProperty("avatar_url")]
        public Uri AvatarUrl
        {
            get => GetProperty() as Uri;
            internal set => SetProperty(value);
        }

        /// <summary>
        /// Gets the browser url to the user profile.
        /// </summary>
        public Uri HtmlUrl
        {
            get => new Uri(Session.Client.BaseUri, new Uri($"/{Login}", UriKind.Relative));
        }

        /// <inheritdoc/>
        protected override Uri Uri => new Uri($"/users/{Login}", UriKind.Relative);
        
        private UserKey Key => _key;

        /// <summary>
        /// Gets the repositories of the user.
        /// </summary>
        /// <param name="update">A value indicating whether the list should be updated if it already exists.</param>
        /// <param name="cancellationToken">An optional <see cref="CancellationToken"/> to cancel the operation.</param>
        /// <returns>A <see cref="Task{RepositoryList}"/> representing the asynchronous operation.</returns>
        public async Task<RepositoryList> GetRepositoriesAsync(bool update = false, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (_repositories == null)
            {
                _repositories = await RepositoryList.CreateAsync(Session, new Uri($"/users/{Login}/repos", UriKind.Relative), cancellationToken);
            }
            else if (update)
            {
                await _repositories.RefreshAsync(cancellationToken);
            }

            return _repositories;
        }

        /// <summary>
        /// Gets the followers of the user.
        /// </summary>
        /// <param name="update">A value indicating whether the list should be updated if it already exists.</param>
        /// <param name="cancellationToken">An optional <see cref="CancellationToken"/> to cancel the operation.</param>
        /// <returns>A <see cref="Task{RepositoryList}"/> representing the asynchronous operation.</returns>
        public async Task<UserList> GetFollowersAsync(bool update = false, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (_followers == null)
            {
                _followers = await UserList.CreateAsync(Session, new Uri($"/users/{Login}/followers", UriKind.Relative), cancellationToken);
            }
            else if (update)
            {
                await _followers.RefreshAsync(cancellationToken);
            }

            return _followers;
        }

        /// <summary>
        /// Gets the users following this user.
        /// </summary>
        /// <param name="update">A value indicating whether the list should be updated if it already exists.</param>
        /// <param name="cancellationToken">An optional <see cref="CancellationToken"/> to cancel the operation.</param>
        /// <returns>A <see cref="Task{RepositoryList}"/> representing the asynchronous operation.</returns>
        public async Task<UserList> GetFollowingAsync(bool update = false, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (_following == null)
            {
                _following = await UserList.CreateAsync(Session, new Uri($"/users/{Login}/followers", UriKind.Relative), cancellationToken);
            }
            else if (update)
            {
                await _following.RefreshAsync(cancellationToken);
            }

            return _following;
        }

        internal static async Task<User> GetAuthenticatedUserAsync(KitHubSession session, CancellationToken cancellationToken)
        {
            KitHubRequest request = new KitHubRequest()
            {
                Uri = new Uri("/user", UriKind.Relative)
            };

            KitHubResponse response = await session.Client.GetAsync(request, cancellationToken);
            string login = response.Content.Value<string>("login");
            if (string.IsNullOrEmpty(login))
            {
                throw new ArgumentException("The login of a user must not be null or empty.");
            }

            User user = GetOrCreate(session, login);
            user.SetFromResponse(response);

            return user;
        }

        internal static async Task<User> GetUserAsync(KitHubSession session, string login, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(login))
            {
                throw new ArgumentException("The login of a user must not be null or empty.");
            }

            User user = GetOrCreate(session, login);
            await user.RefreshAsync(cancellationToken);

            return user;
        }

        internal static User Create(KitHubSession session, JToken data)
        {
            if (data == null)
            {
                return null;
            }

            string login = data.Value<string>("login");
            if (string.IsNullOrEmpty(login))
            {
                throw new ArgumentException("The login of a user must not be null or empty.");
            }

            User user = GetOrCreate(session, login);
            user.SetFromData(data);

            return user;
        }

        internal static User GetOrCreate(KitHubSession session, string login)
        {
            if (string.IsNullOrEmpty(login))
            {
                throw new ArgumentException("The login of a user must not be null or empty.");
            }

            UserKey key = new UserKey(session, login);
            lock (Cache)
            {
                if (Cache.TryGetValue(key, out User existing))
                {
                    return existing;
                }
                else
                {
                    User result = new User(key);
                    Cache[key] = result;

                    return result;
                }
            }
        }

        internal sealed class DefaultInitializer : IModelInitializer
        {
            public object InitializeModel(BindableBase self, JToken data)
            {
                return Create(self.Session, data);
            }
        }

        private sealed class UserKey
        {
            public UserKey(KitHubSession session, string login)
            {
                Session = session;
                Login = login;
            }

            public KitHubSession Session { get; }

            public string Login { get; }

            public override bool Equals(object obj)
            {
                if (obj is UserKey other)
                {
                    return Session == other.Session && Login == other.Login;
                }

                return false;
            }

            public override int GetHashCode()
            {
                return Session.GetHashCode() ^ Login.GetHashCode();
            }
        }
    }
}
