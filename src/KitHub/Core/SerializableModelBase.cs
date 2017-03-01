// -----------------------------------------------------------------------
// <copyright file="SerializableModelBase.cs" company="Niklas Karl">
// Copyright (c) Niklas Karl. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Reflection;
using Newtonsoft.Json.Linq;

namespace KitHub.Core
{
    /// <summary>
    /// The base class of all entities that are serializable.
    /// </summary>
    public abstract class SerializableModelBase : BindableBase
    {
        internal SerializableModelBase(KitHubSession session)
            : base(session)
        {
        }

        /// <summary>
        /// Updates the properties of the entity from a serialized representation of the entity.
        /// </summary>
        /// <param name="data">The serialized representation of the entity.</param>
        protected void SetFromData(JToken data)
        {
            if (data is JObject obj)
            {
                SetFromDataForType(GetType().GetTypeInfo(), obj);
            }
        }

        private static IModelInitializer CreateInitializer(ModelPropertyAttribute attribute)
        {
            if (attribute.Initializer != null)
            {
                return Activator.CreateInstance(attribute.Initializer) as IModelInitializer;
            }

            return null;
        }

        private static IModelInitializer CreateInitializer(ModelListPropertyAttribute attribute)
        {
            if (attribute.Initializer != null)
            {
                return Activator.CreateInstance(attribute.Initializer) as IModelInitializer;
            }

            return null;
        }

        private void SetFromDataForType(TypeInfo type, JObject data)
        {
            TypeInfo baseType = type.BaseType?.GetTypeInfo();
            if (baseType != null)
            {
                SetFromDataForType(baseType, data);
            }

            foreach (PropertyInfo property in type.DeclaredProperties)
            {
                ModelPropertyAttribute attribute = property.GetCustomAttribute<ModelPropertyAttribute>(true);
                if (attribute != null)
                {
                    SetPropertyFromData(data, property, attribute);
                    continue;
                }

                ModelListPropertyAttribute listAttribute = property.GetCustomAttribute<ModelListPropertyAttribute>(true);
                if (listAttribute != null)
                {
                    SetListFromData(data, property, listAttribute);
                    continue;
                }
            }
        }

        private void SetPropertyFromData(JObject data, PropertyInfo property, ModelPropertyAttribute attribute)
        {
            string name = attribute.PropertyName ?? property.Name;
            JToken value = data.SelectToken(name);
            if (value != null)
            {
                TypeInfo type = property.PropertyType.GetTypeInfo();
                IModelInitializer initializer = CreateInitializer(attribute);
                if (type.IsSubclassOf(typeof(SerializableModelBase)))
                {
                    SetModelPropertyFromData(value, property, initializer);
                }
                else
                {
                    SetNonModelPropertyFromData(value, property, initializer);
                }
            }
        }

        private void SetModelPropertyFromData(JToken data, PropertyInfo property, IModelInitializer initializer)
        {
            if (initializer != null)
            {
                SerializableModelBase value = initializer.InitializeModel(this, data) as SerializableModelBase;
                if (property.CanWrite)
                {
                    property.SetValue(this, value);
                }
                else if (property.CanWrite)
                {
                    if (property.GetValue(this) is SerializableModelBase existing && Equals(value, existing) && !ReferenceEquals(value, existing))
                    {
                        existing.SetFromData(data);
                    }
                }
            }
        }

        private void SetNonModelPropertyFromData(JToken data, PropertyInfo property, IModelInitializer initializer)
        {
            // TODO check for initializer

            if (property.CanWrite)
            {
                property.SetValue(this, data.ToObject(property.PropertyType, KitHubSession.Serializer));
            }
            else if (property.CanRead)
            {
                // TODO check for lists
                // TODO check for dictionaries
            }
        }

        private void SetListFromData(JObject data, PropertyInfo property, ModelListPropertyAttribute attribute)
        {
            string name = attribute.PropertyName ?? property.Name;
            JToken value = data.SelectToken(name);
            if (value is JArray array)
            {
                IModelInitializer initializer = CreateInitializer(attribute);
                if (property.CanRead)
                {
                    object current = property.GetValue(this);
                    if (current != null)
                    {
                        TypeInfo currentType = current.GetType().GetTypeInfo();
                        Type modelListType = typeof(ModelListProperty<>).MakeGenericType(currentType.GenericTypeArguments[0]);
                        if (currentType.AsType() == modelListType || currentType.IsSubclassOf(modelListType))
                        {
                            MethodInfo method = modelListType.GetTypeInfo().GetDeclaredMethod(nameof(ModelListProperty<object>.UpdateList));
                            method.Invoke(current, new object[] { this, array, initializer });
                            return;
                        }
                    }
                }
            }
        }
    }
}
