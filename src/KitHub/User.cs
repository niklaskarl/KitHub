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
using Newtonsoft.Json.Linq;

namespace KitHub
{
    /// <summary>
    /// A GitHub user.
    /// </summary>
    public sealed class User : ModelBase
    {
        private static readonly IDictionary<UserKey, User> Cache = new Dictionary<UserKey, User>();

        private UserKey _key;

        private Task<RepositoryList> _repositories;

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
        public DateTime? CreatedAt
        {
            get => GetProperty() as DateTime?;
            private set => SetProperty(value);
        }

        /// <summary>
        /// Gets the timestamp at which the user was last updated.
        /// </summary>
        [ModelProperty("updated_at")]
        public DateTime? UpdatedAt
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
            private set => SetProperty(value);
        }

        /// <summary>
        /// Gets the browser url to the user profile.
        /// </summary>
        public Uri HtmlUrl
        {
            get => new Uri(Session.Client.BaseUri, new Uri(Login, UriKind.Relative));
        }

        /// <summary>
        /// Gets the repositories of the user.
        /// </summary>
        public Task<RepositoryList> Repositories => _repositories = _repositories ?? RepositoryList.CreateAsync(Session, new Uri($"/users/{Login}/repos", UriKind.Relative), default(CancellationToken));

        /// <summary>
        /// Gets the followers of the user.
        /// </summary>
        public UserList Followers => _followers = _followers ?? new UserList(Session, new Uri($"/users/{Login}/followers", UriKind.Relative));

        /// <summary>
        /// Gets the users following this user.
        /// </summary>
        public UserList Following => _following = _following ?? new UserList(Session, new Uri($"/users/{Login}/following", UriKind.Relative));

        /// <inheritdoc/>
        protected override Uri RefreshUri => new Uri($"/users/{Login}", UriKind.Relative);

        /// <inheritdoc/>
        protected override object Key => _key;

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

        internal sealed class DefaultInitializer : IModelPropertyInitializer<User>, IListModelItemInitializer<User>
        {
            public User InitializeItem(ListModelBase<User> self, JToken data)
            {
                return Create(self.Session, data);
            }

            public User InitializeProperty(ModelBase self, JToken data)
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
