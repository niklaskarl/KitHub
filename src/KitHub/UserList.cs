// -----------------------------------------------------------------------
// <copyright file="UserList.cs" company="Niklas Karl">
// Copyright (c) Niklas Karl. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace KitHub
{
    /// <summary>
    /// A list of <see cref="User"/> objects.
    /// </summary>
    [ListModel(Initializer = typeof(User.DefaultInitializer))]
    public sealed class UserList : ListModelBase<User>
    {
        internal UserList(KitHubSession session, Uri refreshUri)
            : base(session)
        {
            RefreshUri = refreshUri;
            RefreshAsync();
        }

        /// <inheritdoc/>
        protected override Uri RefreshUri { get; }
    }
}
