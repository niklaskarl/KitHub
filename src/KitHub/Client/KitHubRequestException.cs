// <copyright file="KitHubRequestException.cs" company="Niklas Karl">
// Copyright (c) Niklas Karl. All rights reserved.
// </copyright>

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
