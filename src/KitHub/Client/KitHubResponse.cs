using System;
using System.Net;
using Newtonsoft.Json.Linq;

namespace KitHub
{
    internal class KitHubResponse
    {
        internal bool HasChanged { get; set; }

        internal JToken Content { get; set; }

        internal string EntityTag { get; set; }

        internal DateTime? LastModified { get; set; }
    }
}
