// -----------------------------------------------------------------------
// <copyright file="PullRequest.cs" company="Niklas Karl">
// Copyright (c) Niklas Karl. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using KitHub.Core;
using Newtonsoft.Json.Linq;

namespace KitHub
{
    /// <summary>
    /// A pull request on GitHub.
    /// </summary>
    public sealed class PullRequest : Issue
    {
        private static readonly IDictionary<ConcreteIssue, PullRequest> Cache = new Dictionary<ConcreteIssue, PullRequest>();

        private ConcreteIssue _issue;

        private PullRequest(ConcreteIssue issue)
            : base(issue.Session)
        {
            _issue = issue;
        }

        /// <summary>
        /// The repository the pull request belongs to.
        /// </summary>
        public override Repository Repository => _issue.Repository;

        /// <summary>
        /// The number of the pull request in the repository.
        /// </summary>
        public override int Number => _issue.Number;

        /// <summary>
        /// The id of the pull request.
        /// </summary>
        [ModelProperty("id")]
        public override long? Id
        {
            get => _issue.Id;
            internal set => _issue.Id = value;
        }

        /// <summary>
        /// The state of the pull request.
        /// Either <c>open</c> or <c>closed</c>.
        /// </summary>
        [ModelProperty("state")]
        public override string State
        {
            get => _issue.State;
            internal set => _issue.State = value;
        }

        /// <summary>
        /// Gets the title of the pull request.
        /// </summary>
        [ModelProperty("title")]
        public override string Title
        {
            get => _issue.Title;
            internal set => _issue.Title = value;
        }

        /// <summary>
        /// Gets the body of the pull request.
        /// </summary>
        [ModelProperty("body")]
        public override string Body
        {
            get => _issue.Body;
            internal set => _issue.Body = value;
        }

        /// <summary>
        /// Gets the user who created the pull request.
        /// </summary>
        [ModelProperty("user", Initializer = typeof(User.DefaultInitializer))]
        public override User User
        {
            get => _issue.User;
            internal set => _issue.User = value;
        }

        /// <summary>
        /// Gets the labels assiciated with the pull request.
        /// </summary>
        [ModelListProperty("labels", Initializer = typeof(LabelInitializer))]
        public override IReadOnlyList<Label> Labels => _issue.Labels;

        /// <summary>
        /// Gets the user assigned to the pull request.
        /// </summary>
        [ModelProperty("assignee", Initializer = typeof(User.DefaultInitializer))]
        public override User Assignee
        {
            get => _issue.Assignee;
            internal set => _issue.Assignee = value;
        }

        /// <summary>
        /// Gets the milestone the pull request is associated with.
        /// </summary>
        [ModelProperty("milestone", Initializer = typeof(MilestoneInitializer))]
        public override Milestone Milestone
        {
            get => _issue.Milestone;
            internal set => _issue.Milestone = value;
        }

        /// <summary>
        /// Gets a value indicating whether the pull request is locked or not.
        /// </summary>
        [ModelProperty("locked")]
        public override bool? IsLocked
        {
            get => _issue.IsLocked;
            internal set => _issue.IsLocked = value;
        }

        /// <summary>
        /// Gets the number of comments on the pull request.
        /// </summary>
        [ModelProperty("comments")]
        public override int? NumberOfComments
        {
            get => _issue.NumberOfComments;
            internal set => _issue.NumberOfComments = value;
        }

        /// <summary>
        /// Gets a value indicating whether the issue is a pull request or not.
        /// This is always true.
        /// </summary>
        public override bool IsPullRequest
        {
            get => true;
            internal set => throw new InvalidOperationException();
        }

        /// <summary>
        /// Gets the timestamp the pull request was closed at.
        /// </summary>
        [ModelProperty("closed_at")]
        public override DateTimeOffset? ClosedAt
        {
            get => _issue.ClosedAt;
            internal set => _issue.ClosedAt = value;
        }

        /// <summary>
        /// Gets the timestamp the pull request was created at.
        /// </summary>
        [ModelProperty("created_at")]
        public override DateTimeOffset? CreatedAt
        {
            get => _issue.CreatedAt;
            internal set => _issue.CreatedAt = value;
        }

        /// <summary>
        /// Gets the timestamp the pull request was last updated at.
        /// </summary>
        [ModelProperty("updated_at")]
        public override DateTimeOffset? UpdatedAt
        {
            get => _issue.UpdatedAt;
            internal set => _issue.UpdatedAt = value;
        }

        /// <summary>
        /// Gets the user who closed to the pull request.
        /// </summary>
        [ModelProperty("closed_by", Initializer = typeof(User.DefaultInitializer))]
        public override User ClosedBy
        {
            get => _issue.ClosedBy;
            internal set => _issue.ClosedBy = value;
        }

        /// <summary>
        /// Gets the timestamp the pull request was merged at.
        /// </summary>
        [ModelProperty("merged_at")]
        public DateTimeOffset? MergedAt
        {
            get => GetProperty() as DateTimeOffset?;
            internal set => SetProperty(value);
        }

        // TODO add head

        // TODO add base

        /// <inheritdoc/>
        protected override Uri Uri => new Uri($"/repos/{Repository.Owner.Login}/{Repository.Name}/pulls/{Number}", UriKind.Relative);

        /// <inheritdoc/>
        public override PullRequest AsPullRequest()
        {
            return this;
        }

        internal static PullRequest Create(KitHubSession session, Repository repository, JToken data)
        {
            if (data == null || data.Type == JTokenType.Null)
            {
                return null;
            }

            if (data is JObject obj && obj.TryGetValue("number", out JToken numberToken) && numberToken.Type == JTokenType.Integer)
            {
                ConcreteIssue issue = ConcreteIssue.GetOrCreate(session, repository, numberToken.Value<int>());
                issue.IsPullRequest = true;

                PullRequest pullRequest = GetOrCreate(issue);
                pullRequest.SetFromData(data);

                return pullRequest;
            }

            throw new KitHubDataException("The issue object is invalid.", data);
        }

        internal static PullRequest GetOrCreate(ConcreteIssue issue)
        {
            if (issue == null)
            {
                throw new ArgumentException("The issue of an pull request must not be null.");
            }
            
            lock (Cache)
            {
                if (Cache.TryGetValue(issue, out PullRequest existing))
                {
                    return existing;
                }
                else
                {
                    PullRequest result = new PullRequest(issue);
                    Cache[issue] = result;

                    return result;
                }
            }
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (obj is PullRequest other)
            {
                return Equals(_issue, other._issue);
            }
            else if (obj is ConcreteIssue otherIssue)
            {
                return Equals(_issue, otherIssue);
            }

            return false;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return _issue.GetHashCode();
        }

        private sealed class LabelInitializer : IModelInitializer
        {
            public object InitializeModel(BindableBase self, JToken data)
            {
                return Label.Create(self.Session, (self as PullRequest).Repository, data);
            }
        }

        private sealed class MilestoneInitializer : IModelInitializer
        {
            public object InitializeModel(BindableBase self, JToken data)
            {
                return Milestone.Create(self.Session, (self as PullRequest).Repository, data);
            }
        }
    }
}
