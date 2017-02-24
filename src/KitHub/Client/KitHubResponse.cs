﻿// -----------------------------------------------------------------------
// <copyright file="KitHubResponse.cs" company="Niklas Karl">
// Copyright (c) Niklas Karl. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

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
