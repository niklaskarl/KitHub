// -----------------------------------------------------------------------
// <copyright file="Commit.cs" company="Niklas Karl">
// Copyright (c) Niklas Karl. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using KitHub.Core;
using Newtonsoft.Json.Linq;

namespace KitHub
{
    /// <summary>
    /// A commit on GitHub.
    /// </summary>
    public sealed class Commit : RefreshableModelBase
    {
        private static readonly IDictionary<CommitKey, Commit> Cache = new Dictionary<CommitKey, Commit>();

        private CommitKey _key;

        private Commit(CommitKey key)
            : base(key.Session)
        {
            _key = key;
        }

        /// <summary>
        /// Gets the repository the commit belongs to.
        /// </summary>
        public Repository Repository { get => _key.Repository; }

        /// <summary>
        /// Gets the sha of the commit.
        /// </summary>
        [ModelProperty("sha")]
        public string Sha { get => _key.Sha; }

        /// <summary>
        /// Gets the message of the commit.
        /// </summary>
        [ModelProperty("commit.message", Initializer = typeof(User.DefaultInitializer))]
        public string Message
        {
            get => GetProperty() as string;
            internal set => SetProperty(value);
        }

        /// <summary>
        /// Gets the author of the commit.
        /// </summary>
        [ModelProperty("author", Initializer = typeof(User.DefaultInitializer))]
        public User Author
        {
            get => GetProperty() as User;
            internal set => SetProperty(value);
        }

        /// <summary>
        /// Gets the author of the commit.
        /// </summary>
        [ModelProperty("committer", Initializer = typeof(User.DefaultInitializer))]
        public User Committer
        {
            get => GetProperty() as User;
            internal set => SetProperty(value);
        }

        // TODO parents

        /// <summary>
        /// Gets the browser url to the commit.
        /// </summary>
        public Uri HtmlUrl
        {
            get => new Uri(Session.Client.BaseUri, new Uri($"{Repository.Owner.Login}/{Repository.Name}/commit/{Sha}", UriKind.Relative));
        }

        /// <inheritdoc/>
        protected override Uri Uri => new Uri($"/repos/{Repository.Owner.Login}/{Repository.Name}/commits/{Sha}", UriKind.Relative);

        private CommitKey Key { get => _key; }

        internal static Commit Create(KitHubSession session, Repository repository, JToken data)
        {
            if (data == null || data.Type == JTokenType.Null)
            {
                return null;
            }

            if (data is JObject obj && obj.TryGetValue("sha", out JToken shaToken) && shaToken is JValue shaValue && shaValue.Value is string sha)
            {
                Commit commit = GetOrCreate(session, repository, sha);
                commit.SetFromData(data);

                return commit;
            }

            throw new KitHubDataException("The commit object is invalid.", data);
        }

        internal static Commit GetOrCreate(KitHubSession session, Repository repository, string sha)
        {
            if (repository == null)
            {
                throw new ArgumentException("The repository of a commit must not be null.");
            }

            if (string.IsNullOrEmpty(sha))
            {
                throw new ArgumentException("The sha of a commit must not be null or empty.");
            }

            CommitKey key = new CommitKey(session, repository, sha);
            lock (Cache)
            {
                if (Cache.TryGetValue(key, out Commit existing))
                {
                    return existing;
                }
                else
                {
                    Commit result = new Commit(key);
                    Cache[key] = result;

                    return result;
                }
            }
        }

        private sealed class CommitKey
        {
            public CommitKey(KitHubSession session, Repository repository, string sha)
            {
                Session = session;
                Repository = repository;
                Sha = sha;
            }

            public KitHubSession Session { get; }

            public Repository Repository { get; }

            public string Sha { get; }

            public override bool Equals(object obj)
            {
                if (obj is CommitKey other)
                {
                    return Session == other.Session && Repository == other.Repository && Sha == other.Sha;
                }

                return false;
            }

            public override int GetHashCode()
            {
                return Session.GetHashCode() ^ Repository.GetHashCode() ^ Sha.GetHashCode();
            }
        }
    }
}
