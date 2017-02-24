// -----------------------------------------------------------------------
// <copyright file="KitHubAuthorizationException.cs" company="Niklas Karl">
// Copyright (c) Niklas Karl. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Net;

namespace KitHub
{
    /// <summary>
    /// The exception that is thrown if an unauthorized request to an GitHub Api endpoint that needs authorization.
    /// </summary>
    public class KitHubAuthorizationException : KitHubRequestException
    {
        internal KitHubAuthorizationException(string message, HttpStatusCode statusCode, string content)
            : base(message, HttpStatusCode.Unauthorized, content)
        {
        }
    }
}
