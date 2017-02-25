// -----------------------------------------------------------------------
// <copyright file="RepositoryList.cs" company="Niklas Karl">
// Copyright (c) Niklas Karl. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using KitHub.Core;

namespace KitHub
{
    /// <summary>
    /// A list of <see cref="Repository"/> objects.
    /// </summary>
    [ListModel(Initializer = typeof(Repository.DefaultInitializer))]
    public sealed class RepositoryList : ListModelBase<Repository>
    {
        private RepositoryList(KitHubSession session, Uri refreshUri)
            : base(session)
        {
            Uri = refreshUri;
        }

        /// <inheritdoc/>
        protected override Uri Uri { get; }

        internal static async Task<RepositoryList> CreateAsync(KitHubSession session, Uri refreshUri, CancellationToken cancellationToken)
        {
            RepositoryList list = new RepositoryList(session, refreshUri);
            await list.RefreshAsync(cancellationToken);

            return list;
        }
    }
}
