// -----------------------------------------------------------------------
// <copyright file="ModelBase{T}.Serialize.cs" company="Niklas Karl">
// Copyright (c) Niklas Karl. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace KitHub
{
    public abstract partial class ModelBase : INotifyPropertyChanged
    {
        private static readonly JsonSerializer Serializer = new JsonSerializer();

        protected void SetFromData(JToken data)
        {
            if (data is JObject obj)
            {
                TypeInfo type = GetType().GetTypeInfo();
                foreach (PropertyInfo property in type.DeclaredProperties)
                {
                    ModelPropertyAttribute attribute = property.GetCustomAttribute<ModelPropertyAttribute>(true);
                    if (attribute != null)
                    {
                        SetPropertyFromData(obj, property, attribute);
                        continue;
                    }

                    // TODO check for list attribute
                }
            }
        }

        private void SetPropertyFromData(JObject data, PropertyInfo property, ModelPropertyAttribute attribute)
        {
            string name = attribute.PropertyName ?? property.Name;
            if (data.TryGetValue(name, out JToken value))
            {
                TypeInfo type = property.PropertyType.GetTypeInfo();
                if (type.IsSubclassOf(typeof(ModelBase)))
                {
                    // TODO
                }
                else
                {
                    SetNonModelPropertyFromData(value, property);
                }
            }
        }

        private void SetModelPropertyFromData(JToken data, PropertyInfo property)
        {
            // TODO
        }

        private void SetNonModelPropertyFromData(JToken data, PropertyInfo property)
        {
            if (property.CanWrite)
            {
                property.SetValue(this, data.ToObject(property.PropertyType, Serializer));
            }
            else if (property.CanRead)
            {
                // TODO check for lists
                // TODO check for dictionaries
            }
        }
    }
}
