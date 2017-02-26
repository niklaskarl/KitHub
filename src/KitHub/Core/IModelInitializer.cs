// -----------------------------------------------------------------------
// <copyright file="IModelInitializer.cs" company="Niklas Karl">
// Copyright (c) Niklas Karl. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using System;
using Newtonsoft.Json.Linq;

namespace KitHub.Core
{
    internal interface IModelInitializer
    {
        object InitializeModel(BindableBase self, JToken data);
    }
}
