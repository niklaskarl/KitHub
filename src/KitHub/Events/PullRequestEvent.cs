// -----------------------------------------------------------------------
// <copyright file="PullRequestEvent.cs" company="Niklas Karl">
// Copyright (c) Niklas Karl. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using System;
using KitHub.Core;
using Newtonsoft.Json.Linq;

namespace KitHub
{
    /// <summary>
    /// A pull request event sent from GitHub.
    /// </summary>
    public class PullRequestEvent : Event
    {
        internal PullRequestEvent(KitHubSession session)
            : base(session)
        {
        }

        /// <summary>
        /// Gets the action that was performed.
        /// </summary>
        [ModelProperty("payload.action")]
        public string Action { get; private set; }

        /// <summary>
        /// Gets the issue the event refers to.
        /// </summary>
        [ModelProperty("payload.pull_request", Initializer = typeof(PullRequestInitializer))]
        public PullRequest PullRequest { get; private set; }

        private sealed class PullRequestInitializer : IModelInitializer
        {
            public object InitializeModel(BindableBase self, JToken data)
            {
                return PullRequest.Create(self.Session, (self as PullRequestEvent).Repository, data);
            }
        }
    }
}
