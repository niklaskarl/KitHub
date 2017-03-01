// -----------------------------------------------------------------------
// <copyright file="PushEvent.cs" company="Niklas Karl">
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
    /// A push event sent from GitHub.
    /// </summary>
    public class PushEvent : Event
    {
        internal PushEvent(KitHubSession session)
            : base(session)
        {
            InternalCommits = new ModelListProperty<CommitEntity>(session);
        }

        /// <summary>
        /// Gets the id of the push.
        /// </summary>
        [ModelProperty("payload.push_id")]
        public long PushId { get; private set; }

        /// <summary>
        /// Gets the number of commits in the push.
        /// </summary>
        [ModelProperty("payload.size")]
        public int Size { get; private set; }

        /// <summary>
        /// Gets the number of distinct commits in the push.
        /// </summary>
        [ModelProperty("payload.distinct_size")]
        public int DistinctSize { get; private set; }

        /// <summary>
        /// Gets the reference the commits where pushed to.
        /// </summary>
        [ModelProperty("payload.ref")]
        public string Ref { get; private set; }

        /// <summary>
        /// Gets the sha of the head of the push.
        /// </summary>
        [ModelProperty("payload.head")]
        public string Head { get; private set; }

        /// <summary>
        /// Gets the sha of the head before the push.
        /// </summary>
        [ModelProperty("payload.before")]
        public string Before { get; private set; }

        /// <summary>
        /// Gets the commits in the push.
        /// </summary>
        public IEnumerable<Commit> Commits => new MappingList<CommitEntity, Commit>(InternalCommits, c =>
        {
            Commit commit = Commit.GetOrCreate(Session, Repository, c.Sha);
            commit.Message = c.Message;

            return commit;
        });

        [ModelListProperty("payload.commits")]
        private ListBase<CommitEntity> InternalCommits { get; }

        private class CommitEntity
        {
            [JsonProperty(PropertyName = "sha", Required = Required.Always)]
            public string Sha { get; set; }

            [JsonProperty(PropertyName = "message")]
            public string Message { get; set; }
        }
    }
}
