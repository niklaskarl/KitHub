// -----------------------------------------------------------------------
// <copyright file="UserList.cs" company="Niklas Karl">
// Copyright (c) Niklas Karl. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;

namespace KitHub
{
    /// <summary>
    /// A list of <see cref="User"/> objects.
    /// </summary>
    [ListModel(Initializer = typeof(User.DefaultInitializer))]
    public sealed class UserList : ListModelBase<User>
    {
        private UserList(KitHubSession session, Uri refreshUri)
            : base(session)
        {
            RefreshUri = refreshUri;
        }

        /// <inheritdoc/>
        protected override Uri RefreshUri { get; }

        internal static async Task<UserList> CreateAsync(KitHubSession session, Uri refreshUri, CancellationToken cancellationToken)
        {
            UserList list = new UserList(session, refreshUri);
            await list.RefreshAsync(cancellationToken);

            return list;
        }
    }
}
