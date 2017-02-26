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
        /// <summary>
        /// Initializes a new instance of the <see cref="KitHubAuthorizationException"/> class.
        /// </summary>
        /// <param name="message">The message describing the exception.</param>
        /// <param name="statusCode">The status code returned by the request.</param>
        /// <param name="content">The content returned by the request.</param>
        public KitHubAuthorizationException(string message, HttpStatusCode statusCode, string content)
            : base(message, HttpStatusCode.Unauthorized, content)
        {
        }
    }
}
