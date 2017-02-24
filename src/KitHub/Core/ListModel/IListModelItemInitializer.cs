using System;
using Newtonsoft.Json.Linq;

namespace KitHub
{
    internal interface IListModelItemInitializer<T>
    {
        T InitializeItem(ListModelBase<T> self, JToken data);
    }
}
