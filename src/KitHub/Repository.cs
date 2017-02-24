// <copyright file="Repository.cs" company="Niklas Karl">
// Copyright (c) Niklas Karl. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;

namespace KitHub
{
    public sealed class Repository : ModelBase
    {
        private static readonly IDictionary<RepositoryKey, Repository> Cache = new Dictionary<RepositoryKey, Repository>();

        private RepositoryKey _key;

        private Repository(RepositoryKey key)
            : base(key.Session)
        {
            _key = key;
        }

        [ModelProperty("owner", Initializer = typeof(User.DefaultInitializer))]
        public User Owner { get => _key.Owner; }

        public string Name { get => _key.Name; }

        [ModelProperty("id")]
        public int? Id
        {
            get => GetProperty() as int?;
            private set => SetProperty(value);
        }

        [ModelProperty("description")]
        public string Description
        {
            get => GetProperty() as string;
            private set => SetProperty(value);
        }

        [ModelProperty("private")]
        public bool? IsPrivate
        {
            get => GetProperty() as bool?;
            private set => SetProperty(value);
        }

        [ModelProperty("fork")]
        public bool? IsFork
        {
            get => GetProperty() as bool?;
            private set => SetProperty(value);
        }

        [ModelProperty("language")]
        public string Language
        {
            get => GetProperty() as string;
            private set => SetProperty(value);
        }

        [ModelProperty("forks_count")]
        public int? ForksCount
        {
            get => GetProperty() as int?;
            private set => SetProperty(value);
        }

        [ModelProperty("stargazers_count")]
        public int? StargazersCount
        {
            get => GetProperty() as int?;
            private set => SetProperty(value);
        }

        [ModelProperty("watchers_count")]
        public int? WatchersCount
        {
            get => GetProperty() as int?;
            private set => SetProperty(value);
        }

        [ModelProperty("size")]
        public int? Size
        {
            get => GetProperty() as int?;
            private set => SetProperty(value);
        }

        [ModelProperty("default_branch")]
        public string DefaultBranch
        {
            get => GetProperty() as string;
            private set => SetProperty(value);
        }

        [ModelProperty("open_issues_count")]
        public int? OpenIssuesCount
        {
            get => GetProperty() as int?;
            private set => SetProperty(value);
        }

        [ModelProperty("has_issues")]
        public bool? HasIssues
        {
            get => GetProperty() as bool?;
            private set => SetProperty(value);
        }

        [ModelProperty("has_wiki")]
        public bool? HasWiki
        {
            get => GetProperty() as bool?;
            private set => SetProperty(value);
        }

        [ModelProperty("has_pages")]
        public bool? HasPages
        {
            get => GetProperty() as bool?;
            private set => SetProperty(value);
        }

        [ModelProperty("has_downloads")]
        public bool? HasDownloads
        {
            get => GetProperty() as bool?;
            private set => SetProperty(value);
        }

        [ModelProperty("pushed_at")]
        public DateTime? PushedAt
        {
            get => GetProperty() as DateTime?;
            private set => SetProperty(value);
        }

        [ModelProperty("created_at")]
        public DateTime? CreatedAt
        {
            get => GetProperty() as DateTime?;
            private set => SetProperty(value);
        }

        [ModelProperty("updated_at")]
        public DateTime? UpdatedAt
        {
            get => GetProperty() as DateTime?;
            private set => SetProperty(value);
        }

        protected override Uri RefreshUri => new Uri($"/repos/{Owner.Login}/{Name}", UriKind.Relative);

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

        internal sealed class DefaultInitializer : IModelPropertyInitializer<Repository>, IListModelItemInitializer<Repository>
        {
            public Repository InitializeItem(ListModelBase<Repository> self, JToken data)
            {
                Repository repository = Create(self.Session, data);
                repository?.SetFromData(data);

                return repository;
            }

            public Repository InitializeProperty(ModelBase self, JToken data)
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
