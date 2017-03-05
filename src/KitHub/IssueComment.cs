// -----------------------------------------------------------------------
// <copyright file="IssueComment.cs" company="Niklas Karl">
// Copyright (c) Niklas Karl. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using KitHub.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace KitHub
{
    /// <summary>
    /// A comment on a issue on GitHub.
    /// </summary>
    public class IssueComment : RefreshableModelBase
    {
        private static readonly IDictionary<IssueCommentKey, IssueComment> Cache = new Dictionary<IssueCommentKey, IssueComment>();

        private IssueCommentKey _key;

        internal IssueComment(IssueCommentKey key)
            : base(key.Session)
        {
            _key = key;
        }

        /// <summary>
        /// The issue the comment belongs to.
        /// </summary>
        public Issue Issue => _key.Issue;

        /// <summary>
        /// The id of the comment in the repository.
        /// </summary>
        [ModelProperty("id")]
        public long Id => _key.Id;

        /// <summary>
        /// Gets the body of the comment.
        /// </summary>
        [ModelProperty("body")]
        public string Body
        {
            get => GetProperty() as string;
            internal set => SetProperty(value);
        }

        /// <summary>
        /// Gets the user who created the comment.
        /// </summary>
        [ModelProperty("user", Initializer = typeof(User.DefaultInitializer))]
        public User User
        {
            get => GetProperty() as User;
            internal set => SetProperty(value);
        }

        /// <summary>
        /// Gets the timestamp the comment was created at.
        /// </summary>
        [ModelProperty("created_at")]
        public DateTimeOffset? CreatedAt
        {
            get => GetProperty() as DateTimeOffset?;
            internal set => SetProperty(value);
        }

        /// <summary>
        /// Gets the timestamp the comment was last updated at.
        /// </summary>
        [ModelProperty("updated_at")]
        public DateTimeOffset? UpdatedAt
        {
            get => GetProperty() as DateTimeOffset?;
            internal set => SetProperty(value);
        }

        /// <inheritdoc />
        protected override Uri Uri => new Uri($"/repos/{Issue.Repository.Owner.Login}/{Issue.Repository.Name}/issues/comments/{Id}", UriKind.Relative);

        internal static IssueComment Create(KitHubSession session, Issue issue, JToken data)
        {
            if (data == null || data.Type == JTokenType.Null)
            {
                return null;
            }

            if (data is JObject obj && obj.TryGetValue("id", out JToken idToken) && idToken.Type == JTokenType.Integer)
            {
                IssueComment comment = GetOrCreate(session, issue, idToken.Value<long>());
                comment.SetFromData(data);

                return comment;
            }

            throw new KitHubDataException("The issue object is invalid.", data);
        }

        internal static IssueComment GetOrCreate(KitHubSession session, Issue issue, long id)
        {
            if (issue == null)
            {
                throw new ArgumentException("The issue of a comment must not be null.");
            }

            if (id < 0)
            {
                throw new ArgumentException("The id of a comment must not be negative.");
            }

            IssueCommentKey key = new IssueCommentKey(session, issue, id);
            lock (Cache)
            {
                if (Cache.TryGetValue(key, out IssueComment existing))
                {
                    return existing;
                }
                else
                {
                    IssueComment result = new IssueComment(key);
                    Cache[key] = result;

                    return result;
                }
            }
        }

        internal sealed class IssueCommentKey
        {
            public IssueCommentKey(KitHubSession session, Issue issue, long id)
            {
                Session = session;
                Issue = issue;
                Id = id;
            }

            public KitHubSession Session { get; }

            public Issue Issue { get; }

            public long Id { get; }

            public override bool Equals(object obj)
            {
                if (obj is IssueCommentKey other)
                {
                    return Session == other.Session && Issue == other.Issue && Id == other.Id;
                }

                return false;
            }

            public override int GetHashCode()
            {
                return Session.GetHashCode() ^ Issue.GetHashCode() ^ Id.GetHashCode();
            }
        }
    }
}
