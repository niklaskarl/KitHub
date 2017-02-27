// -----------------------------------------------------------------------
// <copyright file="KitHubDataException.cs" company="Niklas Karl">
// Copyright (c) Niklas Karl. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace KitHub
{
    /// <summary>
    /// The exception that is thrown when serialized data is corrupt.
    /// </summary>
    public class KitHubDataException : KitHubException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="KitHubDataException"/> class.
        /// </summary>
        /// <param name="message">The message describing the exception.</param>
        /// <param name="coruptData">The data that is considered corrupt.</param>
        public KitHubDataException(string message, object coruptData)
            : base(message)
        {
            CorruptData = coruptData;
        }

        /// <summary>
        /// Gets the data that is considered corrupt.
        /// </summary>
        public object CorruptData { get; }
    }
}
