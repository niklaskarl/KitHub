// -----------------------------------------------------------------------
// <copyright file="IssueEvent.cs" company="Niklas Karl">
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
    /// A issue event sent from GitHub.
    /// </summary>
    public class IssueEvent : Event
    {
        internal IssueEvent(KitHubSession session)
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
        [ModelProperty("payload.issue", Initializer = typeof(IssueInitializer))]
        public Issue Issue { get; private set; }

        private sealed class IssueInitializer : IModelInitializer
        {
            public object InitializeModel(BindableBase self, JToken data)
            {
                return ConcreteIssue.Create(self.Session, (self as IssueEvent).Repository, data);
            }
        }
    }
}
