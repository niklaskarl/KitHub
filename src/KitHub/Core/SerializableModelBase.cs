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
                if (type.IsSubclassOf(typeof(RefreshableModelBase)))
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
                property.SetValue(this, data.ToObject(property.PropertyType, KitHubSession.Serializer));
            }
            else if (property.CanRead)
            {
                // TODO check for lists
                // TODO check for dictionaries
            }
        }
    }
}
