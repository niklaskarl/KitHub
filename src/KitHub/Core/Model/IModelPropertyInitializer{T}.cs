// -----------------------------------------------------------------------
// <copyright file="IModelPropertyInitializer{T}.cs" company="Niklas Karl">
// Copyright (c) Niklas Karl. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using System;
using Newtonsoft.Json.Linq;

namespace KitHub
{
    internal interface IModelPropertyInitializer<T>
    {
        T InitializeProperty(ModelBase self, JToken data);
    }
}
