// -----------------------------------------------------------------------
// <copyright file="Milestone.cs" company="Niklas Karl">
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
    /// A milestone of a repository on GitHub.
    /// </summary>
    public class Milestone : RefreshableModelBase
    {
        private static readonly IDictionary<MilestoneKey, Milestone> Cache = new Dictionary<MilestoneKey, Milestone>();

        private MilestoneKey _key;

        private Milestone(MilestoneKey key)
            : base(key.Session)
        {
            _key = key;
        }

        /// <summary>
        /// Gets the repository the milestone belongs to.
        /// </summary>
        public Repository Repository => _key.Repository;

        /// <summary>
        /// Gets the number of the milestone.
        /// </summary>
        [ModelProperty("number")]
        public int Number => _key.Number;

        /// <summary>
        /// Gets the id of the milestone.
        /// </summary>
        [ModelProperty("id")]
        public long? Id
        {
            get => GetProperty() as long?;
            set => SetProperty(value);
        }

        /// <summary>
        /// Gets the state of the milestone.
        /// </summary>
        [ModelProperty("state")]
        public string State
        {
            get => GetProperty() as string;
            set => SetProperty(value);
        }

        /// <summary>
        /// Gets the title of the milestone.
        /// </summary>
        [ModelProperty("title")]
        public string Title
        {
            get => GetProperty() as string;
            set => SetProperty(value);
        }

        /// <summary>
        /// Gets the description of the milestone.
        /// </summary>
        [ModelProperty("description")]
        public string Description
        {
            get => GetProperty() as string;
            set => SetProperty(value);
        }

        /// <summary>
        /// Gets the creator of the milestone.
        /// </summary>
        [ModelProperty("creator", Initializer = typeof(User.DefaultInitializer))]
        public User Creator
        {
            get => GetProperty() as User;
            set => SetProperty(value);
        }

        /// <summary>
        /// Gets the number of still open issues assigned to the milestone.
        /// </summary>
        [ModelProperty("open_issues")]
        public int? NumberOfOpenIssues
        {
            get => GetProperty() as int?;
            set => SetProperty(value);
        }

        /// <summary>
        /// Gets the number of already closed issues assigned to the milestone.
        /// </summary>
        [ModelProperty("closed_issues")]
        public int? NumberOfClosedIssues
        {
            get => GetProperty() as int?;
            set => SetProperty(value);
        }

        /// <summary>
        /// Gets the timestamp the milestone was created at.
        /// </summary>
        [ModelProperty("created_at")]
        public DateTimeOffset? CreatedAt
        {
            get => GetProperty() as DateTimeOffset?;
            set => SetProperty(value);
        }

        /// <summary>
        /// Gets the timestamp the milestone was last updated at.
        /// </summary>
        [ModelProperty("updated_at")]
        public DateTimeOffset? UpdatedAt
        {
            get => GetProperty() as DateTimeOffset?;
            set => SetProperty(value);
        }

        /// <summary>
        /// Gets the timestamp the milestone was closed at.
        /// </summary>
        [ModelProperty("closed_at")]
        public DateTimeOffset? ClosedAt
        {
            get => GetProperty() as DateTimeOffset?;
            set => SetProperty(value);
        }

        /// <summary>
        /// Gets the timestamp the milestone is due on.
        /// </summary>
        [ModelProperty("due_on")]
        public DateTimeOffset? DueOn
        {
            get => GetProperty() as DateTimeOffset?;
            set => SetProperty(value);
        }

        internal static Milestone Create(KitHubSession session, Repository repository, JToken data)
        {
            if (data == null || data.Type == JTokenType.Null)
            {
                return null;
            }
            
            if (data is JObject obj && obj.TryGetValue("number", out JToken numberToken) && numberToken.Type == JTokenType.Integer)
            {
                Milestone milestone = GetOrCreate(session, repository, numberToken.Value<int>());
                milestone.SetFromData(data);

                return milestone;
            }

            throw new KitHubDataException("The milestone object is invalid.", data);
        }

        internal static Milestone GetOrCreate(KitHubSession session, Repository repository, int number)
        {
            if (repository == null)
            {
                throw new ArgumentException("The repository of a milestone must not be null.");
            }

            if (number < 1)
            {
                throw new ArgumentException("The number of a milestone must greater than or equal to 1.");
            }

            MilestoneKey key = new MilestoneKey(session, repository, number);
            lock (Cache)
            {
                if (Cache.TryGetValue(key, out Milestone existing))
                {
                    return existing;
                }
                else
                {
                    Milestone result = new Milestone(key);
                    Cache[key] = result;

                    return result;
                }
            }
        }

        /// <inheritdoc />
        protected override Uri Uri => new Uri($"/repos/{Repository.Owner.Login}/{Repository.Name}/milestones/{Number}");

        private sealed class MilestoneKey
        {
            public MilestoneKey(KitHubSession session, Repository repository, int number)
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
                if (obj is MilestoneKey other)
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
    }
}
