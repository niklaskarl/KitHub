// -----------------------------------------------------------------------
// <copyright file="Repository.cs" company="Niklas Karl">
// Copyright (c) Niklas Karl. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;

namespace KitHub
{
    /// <summary>
    /// A repository hosted on GitHub.
    /// </summary>
    public sealed class Repository : ModelBase
    {
        private static readonly IDictionary<RepositoryKey, Repository> Cache = new Dictionary<RepositoryKey, Repository>();

        private RepositoryKey _key;

        private Repository(RepositoryKey key)
            : base(key.Session)
        {
            _key = key;
        }

        /// <summary>
        /// Gets the owner of the repository.
        /// </summary>
        [ModelProperty("owner", Initializer = typeof(User.DefaultInitializer))]
        public User Owner { get => _key.Owner; }

        /// <summary>
        /// Gets the name of the repository.
        /// </summary>
        public string Name { get => _key.Name; }

        /// <summary>
        /// Gets the id of the repository.
        /// </summary>
        [ModelProperty("id")]
        public int? Id
        {
            get => GetProperty() as int?;
            private set => SetProperty(value);
        }

        /// <summary>
        /// Gets the description of the repository.
        /// </summary>
        [ModelProperty("description")]
        public string Description
        {
            get => GetProperty() as string;
            private set => SetProperty(value);
        }

        /// <summary>
        /// Gets a value indicating whether the repository is private or not.
        /// </summary>
        [ModelProperty("private")]
        public bool? IsPrivate
        {
            get => GetProperty() as bool?;
            private set => SetProperty(value);
        }

        /// <summary>
        /// Gets a value indicating whether the repository is fork of another repository or not..
        /// </summary>
        [ModelProperty("fork")]
        public bool? IsFork
        {
            get => GetProperty() as bool?;
            private set => SetProperty(value);
        }

        /// <summary>
        /// Gets the main programming language of the repository.
        /// </summary>
        [ModelProperty("language")]
        public string Language
        {
            get => GetProperty() as string;
            private set => SetProperty(value);
        }

        /// <summary>
        /// Gets the number of forks of the repository.
        /// </summary>
        [ModelProperty("forks_count")]
        public int? ForksCount
        {
            get => GetProperty() as int?;
            private set => SetProperty(value);
        }

        /// <summary>
        /// Gets the number of stargazers of the repository.
        /// </summary>
        [ModelProperty("stargazers_count")]
        public int? StargazersCount
        {
            get => GetProperty() as int?;
            private set => SetProperty(value);
        }

        /// <summary>
        /// Gets the number of users watching this repository.
        /// </summary>
        [ModelProperty("watchers_count")]
        public int? WatchersCount
        {
            get => GetProperty() as int?;
            private set => SetProperty(value);
        }

        /// <summary>
        /// Gets the size of the repository.
        /// </summary>
        [ModelProperty("size")]
        public int? Size
        {
            get => GetProperty() as int?;
            private set => SetProperty(value);
        }

        /// <summary>
        /// Gets the name of the default branch.
        /// </summary>
        [ModelProperty("default_branch")]
        public string DefaultBranch
        {
            get => GetProperty() as string;
            private set => SetProperty(value);
        }

        /// <summary>
        /// Gets the number of open issues.
        /// </summary>
        [ModelProperty("open_issues_count")]
        public int? OpenIssuesCount
        {
            get => GetProperty() as int?;
            private set => SetProperty(value);
        }

        /// <summary>
        /// Gets a value indicating whether the repository has an issues section or not.
        /// </summary>
        [ModelProperty("has_issues")]
        public bool? HasIssues
        {
            get => GetProperty() as bool?;
            private set => SetProperty(value);
        }

        /// <summary>
        /// Gets a value indicating whether the repository has a wiki or not.
        /// </summary>
        [ModelProperty("has_wiki")]
        public bool? HasWiki
        {
            get => GetProperty() as bool?;
            private set => SetProperty(value);
        }

        /// <summary>
        /// Gets a value indicating whether the repository has github.io pages or not.
        /// </summary>
        [ModelProperty("has_pages")]
        public bool? HasPages
        {
            get => GetProperty() as bool?;
            private set => SetProperty(value);
        }

        /// <summary>
        /// Gets a value indicating whether the repository has downloads or not.
        /// </summary>
        [ModelProperty("has_downloads")]
        public bool? HasDownloads
        {
            get => GetProperty() as bool?;
            private set => SetProperty(value);
        }

        /// <summary>
        /// Gets the timestamp at which the repository was last pushed.
        /// </summary>
        [ModelProperty("pushed_at")]
        public DateTime? PushedAt
        {
            get => GetProperty() as DateTime?;
            private set => SetProperty(value);
        }

        /// <summary>
        /// Gets the timestamp at which the repository was created.
        /// </summary>
        [ModelProperty("created_at")]
        public DateTime? CreatedAt
        {
            get => GetProperty() as DateTime?;
            private set => SetProperty(value);
        }

        /// <summary>
        /// Gets the timestamp at which the repository was last updated.
        /// </summary>
        [ModelProperty("updated_at")]
        public DateTime? UpdatedAt
        {
            get => GetProperty() as DateTime?;
            private set => SetProperty(value);
        }

        /// <inheritdoc/>
        protected override Uri Uri => new Uri($"/repos/{Owner.Login}/{Name}", UriKind.Relative);

        /// <inheritdoc/>
        protected override object Key { get => _key; }

        internal static Repository Create(KitHubSession session, JToken data)
        {
            if (data == null)
            {
                return null;
            }

            if (data is JObject repository && repository.TryGetValue("owner", out JToken owner))
            {
                return Create(session, User.Create(session, owner), data);
            }

            throw new InvalidDataException();
        }

        internal static Repository Create(KitHubSession session, User owner, JToken data)
        {
            if (data == null)
            {
                return null;
            }

            if (owner == null)
            {
                throw new ArgumentException("The owner of a repository must not be null.");
            }

            string name = data.Value<string>("name");
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("The name of a repository must not be null or empty.");
            }

            RepositoryKey key = new RepositoryKey(session, owner, name);
            lock (Cache)
            {
                if (Cache.TryGetValue(key, out Repository existing))
                {
                    existing.SetFromData(data);
                    return existing;
                }
                else
                {
                    Repository result = new Repository(key);
                    Cache[key] = result;

                    result.SetFromData(data);
                    return result;
                }
            }
        }

        internal sealed class DefaultInitializer : IModelInitializer<Repository>
        {
            public Repository InitializeModel(BindableBase self, JToken data)
            {
                Repository repository = Create(self.Session, data);
                repository?.SetFromData(data);

                return repository;
            }
        }

        private sealed class RepositoryKey
        {
            public RepositoryKey(KitHubSession session, User owner, string name)
            {
                Session = session;
                Owner = owner;
                Name = name;
            }

            public KitHubSession Session { get; }

            public User Owner { get; }

            public string Name { get; }

            public override bool Equals(object obj)
            {
                if (obj is RepositoryKey other)
                {
                    return Session == other.Session && Owner == other.Owner && Name == other.Name;
                }

                return false;
            }

            public override int GetHashCode()
            {
                return Session.GetHashCode() ^ Owner.GetHashCode() ^ Name.GetHashCode();
            }
        }
    }
}
