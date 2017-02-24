// <copyright file="KitHubResponse.cs" company="Niklas Karl">
// Copyright (c) Niklas Karl. All rights reserved.
// </copyright>

using System;
using Newtonsoft.Json.Linq;

namespace KitHub
{
    internal class KitHubResponse
    {
        internal bool HasChanged { get; set; }

        internal JToken Content { get; set; }

        internal string EntityTag { get; set; }

        internal DateTime? LastModified { get; set; }
    }
}
