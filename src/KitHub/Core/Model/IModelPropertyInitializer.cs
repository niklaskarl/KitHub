using System;
using Newtonsoft.Json.Linq;

namespace KitHub
{
    internal interface IModelPropertyInitializer<T>
    {
        T InitializeProperty(ModelBase self, JToken data);
    }
}
