// -----------------------------------------------------------------------
// <copyright file="ModelPropertyAttribute.cs" company="Niklas Karl">
// Copyright (c) Niklas Karl. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace KitHub
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    internal sealed class ModelPropertyAttribute : Attribute
    {
        public ModelPropertyAttribute(string propertyName)
        {
            PropertyName = propertyName;
        }

        public string PropertyName { get; set; }

        public Type Initializer { get; set; }
    }
}
