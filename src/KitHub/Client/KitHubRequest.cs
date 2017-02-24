// <copyright file="KitHubRequest.cs" company="Niklas Karl">
// Copyright (c) Niklas Karl. All rights reserved.
// </copyright>

using System;

namespace KitHub
{
    internal class KitHubRequest
    {
        public Uri Uri { get; set; }

        internal string EntityTag { get; set; }

        internal DateTime? LastModified { get; set; }
    }
}
