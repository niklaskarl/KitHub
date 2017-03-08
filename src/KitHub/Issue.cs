// -----------------------------------------------------------------------
// <copyright file="Issue.cs" company="Niklas Karl">
// Copyright (c) Niklas Karl. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using KitHub.Core;

namespace KitHub
{
    /// <summary>
    /// A issue on GitHub.
    /// </summary>
    public abstract class Issue : RefreshableModelBase
    {
        internal Issue(KitHubSession session)
            : base(session)
        {
        }

        /// <summary>
        /// The repository the issue belongs to.
        /// </summary>
        public abstract Repository Repository { get; }

        /// <summary>
        /// The number of the issue in the repository.
        /// </summary>
        public abstract int Number { get; }

        /// <summary>
        /// The id of the issue.
        /// </summary>
        public abstract long? Id { get; internal set; }

        /// <summary>
        /// The state of the issue.
        /// Either <c>open</c> or <c>closed</c>.
        /// </summary>
        public abstract string State { get; internal set; }

        /// <summary>
        /// Gets the title of the issue.
        /// </summary>
        public abstract string Title { get; internal set; }

        /// <summary>
        /// Gets the body of the issue.
        /// </summary>
        public abstract string Body { get; internal set; }

        /// <summary>
        /// Gets the user who created the issue.
        /// </summary>
        public abstract User User { get; internal set; }

        /// <summary>
        /// Gets the labels assiciated with the issue.
        /// </summary>
        public abstract IReadOnlyList<Label> Labels { get; }

        /// <summary>
        /// Gets the user assigned to the issue.
        /// </summary>
        public abstract User Assignee { get; internal set; }

        /// <summary>
        /// Gets the milestone the issue is associated with.
        /// </summary>
        public abstract Milestone Milestone { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether the issue is locked or not.
        /// </summary>
        public abstract bool? IsLocked { get; internal set; }

        /// <summary>
        /// Gets the number of comments on the issue.
        /// </summary>
        public abstract int? NumberOfComments { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether the issue is a pull request or not.
        /// </summary>
        public abstract bool IsPullRequest { get; internal set; }

        /// <summary>
        /// Gets the timestamp the issue was closed at.
        /// </summary>
        public abstract DateTimeOffset? ClosedAt { get; internal set; }

        /// <summary>
        /// Gets the timestamp the issue was created at.
        /// </summary>
        public abstract DateTimeOffset? CreatedAt { get; internal set; }

        /// <summary>
        /// Gets the timestamp the issue was last updated at.
        /// </summary>
        public abstract DateTimeOffset? UpdatedAt { get; internal set; }

        /// <summary>
        /// Gets the user who closed to the issue.
        /// </summary>
        public abstract User ClosedBy { get; internal set; }

        /// <summary>
        /// Casts the issue to the corresponding pull request.
        /// </summary>
        /// <returns>The pull request for the issue.</returns>
        public abstract PullRequest AsPullRequest();
    }
}
