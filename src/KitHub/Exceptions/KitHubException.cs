// -----------------------------------------------------------------------
// <copyright file="KitHubException.cs" company="Niklas Karl">
// Copyright (c) Niklas Karl. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace KitHub
{
    /// <summary>
    /// The base class of all exceptions that are thrown if the GitHub Api reports errors.
    /// </summary>
    public class KitHubException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="KitHubException"/> class.
        /// </summary>
        /// <param name="message">The message describing the exception.</param>
        public KitHubException(string message)
            : base(message)
        {
        }
    }
}
