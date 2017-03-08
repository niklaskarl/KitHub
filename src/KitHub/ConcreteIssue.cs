// -----------------------------------------------------------------------
// <copyright file="Issue.cs" company="Niklas Karl">
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
    internal class ConcreteIssue : Issue
    {
        private static readonly IDictionary<IssueKey, ConcreteIssue> Cache = new Dictionary<IssueKey, ConcreteIssue>();

        private IssueKey _key;

        internal ConcreteIssue(ConcreteIssue issue)
            : this(issue._key)
        {
        }

        private ConcreteIssue(IssueKey key)
            : base(key.Session)
        {
            _key = key;
            Labels = new ModelListProperty<Label>(key.Session);
        }
        
        public override Repository Repository => _key.Repository;
        
        [ModelProperty("number")]
        public override int Number => _key.Number;
        
        [ModelProperty("id")]
        public override long? Id
        {
            get => GetProperty() as long?;
            internal set => SetProperty(value);
        }
        
        [ModelProperty("state")]
        public override string State
        {
            get => GetProperty() as string;
            internal set => SetProperty(value);
        }
        
        [ModelProperty("title")]
        public override string Title
        {
            get => GetProperty() as string;
            internal set => SetProperty(value);
        }
        
        [ModelProperty("body")]
        public override string Body
        {
            get => GetProperty() as string;
            internal set => SetProperty(value);
        }
        
        [ModelProperty("user", Initializer = typeof(User.DefaultInitializer))]
        public override User User
        {
            get => GetProperty() as User;
            internal set => SetProperty(value);
        }
        
        [ModelListProperty("labels", Initializer = typeof(LabelInitializer))]
        public override IReadOnlyList<Label> Labels { get; }
        
        [ModelProperty("assignee", Initializer = typeof(User.DefaultInitializer))]
        public override User Assignee
        {
            get => GetProperty() as User;
            internal set => SetProperty(value);
        }
        
        [ModelProperty("milestone", Initializer = typeof(MilestoneInitializer))]
        public override Milestone Milestone
        {
            get => GetProperty() as Milestone;
            internal set => SetProperty(value);
        }
        
        [ModelProperty("locked")]
        public override bool? IsLocked
        {
            get => GetProperty() as bool?;
            internal set => SetProperty(value);
        }
        
        [ModelProperty("comments")]
        public override int? NumberOfComments
        {
            get => GetProperty() as int?;
            internal set => SetProperty(value);
        }
        
        [ModelProperty("pull_request", Initializer = typeof(PullRequestInitializer))]
        public override bool IsPullRequest { get; internal set; }
        
        [ModelProperty("closed_at")]
        public override DateTimeOffset? ClosedAt
        {
            get => GetProperty() as DateTimeOffset?;
            internal set => SetProperty(value);
        }
        
        [ModelProperty("created_at")]
        public override DateTimeOffset? CreatedAt
        {
            get => GetProperty() as DateTimeOffset?;
            internal set => SetProperty(value);
        }
        
        [ModelProperty("updated_at")]
        public override DateTimeOffset? UpdatedAt
        {
            get => GetProperty() as DateTimeOffset?;
            internal set => SetProperty(value);
        }
        
        [ModelProperty("closed_by", Initializer = typeof(User.DefaultInitializer))]
        public override User ClosedBy
        {
            get => GetProperty() as User;
            internal set => SetProperty(value);
        }
        
        protected override Uri Uri => new Uri($"/repos/{Repository.Owner.Login}/{Repository.Name}/issues/{Number}", UriKind.Relative);
        
        public override PullRequest AsPullRequest()
        {
            if (!IsPullRequest)
            {
                throw new InvalidOperationException();
            }

            return PullRequest.GetOrCreate(this);
        }
        
        public override bool Equals(object obj)
        {
            if (obj is PullRequest otherPullRequest)
            {
                return otherPullRequest.Equals(this);
            }
            else if (obj is ConcreteIssue other)
            {
                return ReferenceEquals(this, other);
            }

            return false;
        }
        
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        internal static ConcreteIssue Create(KitHubSession session, Repository repository, JToken data)
        {
            if (data == null || data.Type == JTokenType.Null)
            {
                return null;
            }
            
            if (data is JObject obj && obj.TryGetValue("number", out JToken numberToken) && numberToken.Type == JTokenType.Integer)
            {
                ConcreteIssue issue = GetOrCreate(session, repository, numberToken.Value<int>());
                issue.SetFromData(data);

                return issue;
            }

            throw new KitHubDataException("The issue object is invalid.", data);
        }

        internal static ConcreteIssue GetOrCreate(KitHubSession session, Repository repository, int number)
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
                if (Cache.TryGetValue(key, out ConcreteIssue existing))
                {
                    return existing;
                }
                else
                {
                    ConcreteIssue result = new ConcreteIssue(key);
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
                return Label.Create(self.Session, (self as ConcreteIssue).Repository, data);
            }
        }

        private sealed class MilestoneInitializer : IModelInitializer
        {
            public object InitializeModel(BindableBase self, JToken data)
            {
                return Milestone.Create(self.Session, (self as ConcreteIssue).Repository, data);
            }
        }

        private sealed class PullRequestInitializer
        {
            public object InitializeModel(BindableBase self, JToken data)
            {
                return true;
            }
        }
    }
}
