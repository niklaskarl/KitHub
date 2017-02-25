// -----------------------------------------------------------------------
// <copyright file="KitHubRequest.cs" company="Niklas Karl">
// Copyright (c) Niklas Karl. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace KitHub.Client
{
    internal class KitHubRequest
    {
        public Uri Uri { get; set; }

        internal string EntityTag { get; set; }

        internal DateTime? LastModified { get; set; }
    }
}
