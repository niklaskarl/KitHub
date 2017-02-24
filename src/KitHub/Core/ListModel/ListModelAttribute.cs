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
