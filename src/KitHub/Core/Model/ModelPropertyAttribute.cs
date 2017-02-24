// <copyright file="ModelPropertyAttribute.cs" company="Niklas Karl">
// Copyright (c) Niklas Karl. All rights reserved.
// </copyright>

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
