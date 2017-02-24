// -----------------------------------------------------------------------
// <copyright file="KitHubRequestException.cs" company="Niklas Karl">
// Copyright (c) Niklas Karl. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

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
