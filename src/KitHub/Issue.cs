// -----------------------------------------------------------------------
// <copyright file="Issue.cs" company="Niklas Karl">
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
    /// A issue on GitHub.
    /// </summary>
    public class Issue : RefreshableModelBase
    {
        private static readonly IDictionary<IssueKey, Issue> Cache = new Dictionary<IssueKey, Issue>();

        private IssueKey _key;

        internal Issue(IssueKey key)
            : base(key.Session)
        {
            _key = key;
            Labels = new ModelListProperty<Label>(key.Session);
        }

        /// <summary>
        /// The repository the issue belongs to.
        /// </summary>
        public Repository Repository => _key.Repository;

        /// <summary>
        /// The number of the issue in the repository.
        /// </summary>
        [ModelProperty("number")]
        public int Number => _key.Number;

        /// <summary>
        /// The id of the issue.
        /// </summary>
        [ModelProperty("id")]
        public long? Id
        {
            get => GetProperty() as long?;
            internal set => SetProperty(value);
        }

        /// <summary>
        /// The state of the issue.
        /// Either <c>open</c> or <c>closed</c>.
        /// </summary>
        [ModelProperty("state")]
        public string State
        {
            get => GetProperty() as string;
            internal set => SetProperty(value);
        }

        /// <summary>
        /// Gets the title of the issue.
        /// </summary>
        [ModelProperty("title")]
        public string Title
        {
            get => GetProperty() as string;
            internal set => SetProperty(value);
        }

        /// <summary>
        /// Gets the body of the issue.
        /// </summary>
        [ModelProperty("body")]
        public string Body
        {
            get => GetProperty() as string;
            internal set => SetProperty(value);
        }

        /// <summary>
        /// Gets the user who created the issue.
        /// </summary>
        [ModelProperty("user", Initializer = typeof(User.DefaultInitializer))]
        public User User
        {
            get => GetProperty() as User;
            internal set => SetProperty(value);
        }

        /// <summary>
        /// Gets the labels assiciated with the issue.
        /// </summary>
        [ModelListProperty("labels", Initializer = typeof(LabelInitializer))]
        public IReadOnlyList<Label> Labels { get; }

        /// <summary>
        /// Gets the user assigned to the issue.
        /// </summary>
        [ModelProperty("assignee", Initializer = typeof(User.DefaultInitializer))]
        public User Assignee
        {
            get => GetProperty() as User;
            internal set => SetProperty(value);
        }

        /// <summary>
        /// Gets the milestone the issue is associated with.
        /// </summary>
        [ModelProperty("milestone", Initializer = typeof(MilestoneInitializer))]
        public Milestone Milestone
        {
            get => GetProperty() as Milestone;
            internal set => SetProperty(value);
        }

        /// <summary>
        /// Gets a value indicating whether the issue is locked or not.
        /// </summary>
        [ModelProperty("locked")]
        public bool? IsLocked
        {
            get => GetProperty() as bool?;
            internal set => SetProperty(value);
        }

        /// <summary>
        /// Gets the number of comments on the issue.
        /// </summary>
        [ModelProperty("comments")]
        public int? NumberOfComments
        {
            get => GetProperty() as int?;
            internal set => SetProperty(value);
        }

        /// <summary>
        /// Gets a value indicating whether the issue is a pull request or not.
        /// </summary>
        public bool IsPullRequest => PullRequest != null;

        /// <summary>
        /// Gets the timestamp the issue was closed at.
        /// </summary>
        [ModelProperty("closed_at")]
        public DateTimeOffset? ClosedAt
        {
            get => GetProperty() as DateTimeOffset?;
            internal set => SetProperty(value);
        }

        /// <summary>
        /// Gets the timestamp the issue was created at.
        /// </summary>
        [ModelProperty("created_at")]
        public DateTimeOffset? CreatedAt
        {
            get => GetProperty() as DateTimeOffset?;
            internal set => SetProperty(value);
        }

        /// <summary>
        /// Gets the timestamp the issue was last updated at.
        /// </summary>
        [ModelProperty("updated_at")]
        public DateTimeOffset? UpdatedAt
        {
            get => GetProperty() as DateTimeOffset?;
            internal set => SetProperty(value);
        }

        /// <summary>
        /// Gets the user who closed to the issue.
        /// </summary>
        [ModelProperty("closed_by", Initializer = typeof(User.DefaultInitializer))]
        public User ClosedBy
        {
            get => GetProperty() as User;
            internal set => SetProperty(value);
        }

        /// <inheritdoc />
        protected override Uri Uri => new Uri($"/repos/{Repository.Owner.Login}/{Repository.Name}/issues/{Number}", UriKind.Relative);

        [ModelProperty("pull_request")]
        private PullRequestEntity PullRequest { get; set; }

        internal static Issue Create(KitHubSession session, Repository repository, JToken data)
        {
            if (data == null || data.Type == JTokenType.Null)
            {
                return null;
            }
            
            if (data is JObject obj && obj.TryGetValue("number", out JToken numberToken) && numberToken.Type == JTokenType.Integer)
            {
                Issue issue = GetOrCreate(session, repository, numberToken.Value<int>());
                issue.SetFromData(data);

                return issue;
            }

            throw new KitHubDataException("The issue object is invalid.", data);
        }

        internal static Issue GetOrCreate(KitHubSession session, Repository repository, int number)
        {
            if (repository == null)
            {
                throw new ArgumentException("The repository of an issue must not be null.");
            }

            if (number <= 0)
            {
                throw new ArgumentException("The number of an issue must be greater than or equal to 1.");
            }

            IssueKey key = new IssueKey(session, repository, number);
            lock (Cache)
            {
                if (Cache.TryGetValue(key, out Issue existing))
                {
                    return existing;
                }
                else
                {
                    Issue result = new Issue(key);
                    Cache[key] = result;

                    return result;
                }
            }
        }

        internal sealed class IssueKey
        {
            public IssueKey(KitHubSession session, Repository repository, int number)
            {
                Session = session;
                Repository = repository;
                Number = number;
            }

            public KitHubSession Session { get; }

            public Repository Repository { get; }

            public int Number { get; }

            public override bool Equals(object obj)
            {
                if (obj is IssueKey other)
                {
                    return Session == other.Session && Repository == other.Repository && Number == other.Number;
                }

                return false;
            }

            public override int GetHashCode()
            {
                return Session.GetHashCode() ^ Repository.GetHashCode() ^ Number.GetHashCode();
            }
        }

        private sealed class LabelInitializer : IModelInitializer
        {
            public object InitializeModel(BindableBase self, JToken data)
            {
                return Label.Create(self.Session, (self as Issue).Repository, data);
            }
        }

        private sealed class MilestoneInitializer : IModelInitializer
        {
            public object InitializeModel(BindableBase self, JToken data)
            {
                return Milestone.Create(self.Session, (self as Issue).Repository, data);
            }
        }

        private sealed class PullRequestEntity
        {
            [JsonProperty(PropertyName = "url")]
            public string Url { get; set; }

            [JsonProperty(PropertyName = "html_url")]
            public string HtmlUrl { get; set; }

            [JsonProperty(PropertyName = "diff_url")]
            public string DiffUrl { get; set; }

            [JsonProperty(PropertyName = "html_url")]
            public string PatchUrl { get; set; }
        }
    }
}
