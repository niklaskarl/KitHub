// <copyright file="IListModelItemInitializer.cs" company="Niklas Karl">
// Copyright (c) Niklas Karl. All rights reserved.
// </copyright>

using System;
using Newtonsoft.Json.Linq;

namespace KitHub
{
    internal interface IListModelItemInitializer<T>
    {
        T InitializeItem(ListModelBase<T> self, JToken data);
    }
}
