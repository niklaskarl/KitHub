using System;
using System.Net;

namespace KitHub
{
    public class KitHubRequestException : Exception
    {
        internal KitHubRequestException(string message, HttpStatusCode statusCode)
            : base(message)
        {
            StatusCode = statusCode;
        }

        public HttpStatusCode StatusCode { get; }
    }
}
