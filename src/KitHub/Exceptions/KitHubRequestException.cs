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
    /// <summary>
    /// The exception that is thrown if a request to the GitHub Api fails.
    /// </summary>
    public class KitHubRequestException : KitHubException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="KitHubRequestException"/> class.
        /// </summary>
        /// <param name="message">The message describing the exception.</param>
        /// <param name="statusCode">The status code returned by the request.</param>
        /// <param name="content">The content returned by the request.</param>
        public KitHubRequestException(string message, HttpStatusCode statusCode, string content)
            : base(message)
        {
            StatusCode = statusCode;
        }
        
        /// <summary>
        /// Gets the status code of the response from GitHub.
        /// </summary>
        public HttpStatusCode StatusCode { get; }

        /// <summary>
        /// Gets the content of the response from GitHub.
        /// </summary>
        public string Content { get; }
    }
}
