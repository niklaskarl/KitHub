// <copyright file="IModelPropertyInitializer.cs" company="Niklas Karl">
// Copyright (c) Niklas Karl. All rights reserved.
// </copyright>

using System;
using Newtonsoft.Json.Linq;

namespace KitHub
{
    internal interface IModelPropertyInitializer<T>
    {
        T InitializeProperty(ModelBase self, JToken data);
    }
}
