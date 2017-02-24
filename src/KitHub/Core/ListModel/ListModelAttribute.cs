// -----------------------------------------------------------------------
// <copyright file="ListModelAttribute.cs" company="Niklas Karl">
// Copyright (c) Niklas Karl. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace KitHub
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    internal sealed class ListModelAttribute : Attribute
    {
        public ListModelAttribute()
        {
        }

        public Type Initializer { get; set; }
    }
}
