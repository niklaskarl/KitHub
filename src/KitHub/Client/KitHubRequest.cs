using System;

namespace KitHub
{
    internal class KitHubRequest
    {
        public Uri Uri { get; set; }

        internal string EntityTag { get; set; }

        internal DateTime? LastModified { get; set; }
    }
}
