// <copyright file="UserList.cs" company="Niklas Karl">
// Copyright (c) Niklas Karl. All rights reserved.
// </copyright>

using System;

namespace KitHub
{
    [ListModel(Initializer = typeof(User.DefaultInitializer))]
    public sealed class UserList : ListModelBase<User>
    {
        internal UserList(KitHubSession session, Uri refreshUri)
            : base(session)
        {
            RefreshUri = refreshUri;
            RefreshAsync();
        }

        protected override Uri RefreshUri { get; }
    }
}
