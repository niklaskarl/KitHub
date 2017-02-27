// -----------------------------------------------------------------------
// <copyright file="LAbel.cs" company="Niklas Karl">
// Copyright (c) Niklas Karl. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using KitHub.Core;
using Newtonsoft.Json.Linq;

namespace KitHub
{
    /// <summary>
    /// A label of a <see cref="Repository"/> on GitHub.
    /// </summary>
    public class Label : RefreshableModelBase
    {
        private static readonly IDictionary<LabelKey, Label> Cache = new Dictionary<LabelKey, Label>();

        private LabelKey _key;

        private Label(LabelKey key)
            : base(key.Session)
        {
            _key = key;
        }

        /// <summary>
        /// Gets the repository the label belongs to.
        /// </summary>
        public Repository Repository => _key.Repository;

        /// <summary>
        /// Gets the name of the label.
        /// </summary>
        [ModelProperty("name")]
        public string Name => _key.Name;

        /// <summary>
        /// Gets the id of the label.
        /// </summary>
        [ModelProperty("id")]
        public long? Id
        {
            get => GetProperty() as long?;
            set => SetProperty(value);
        }

        /// <summary>
        /// gets the color of the label.
        /// </summary>
        [ModelProperty("color")]
        public string Color
        {
            get => GetProperty() as string;
            set => SetProperty(value);
        }

        /// <summary>
        /// Gets a value indicating whether the label is a default label.
        /// </summary>
        [ModelProperty("default")]
        public bool? IsDefault
        {
            get => GetProperty() as bool?;
            set => SetProperty(value);
        }

        /// <inheritdoc />
        protected override Uri Uri => new Uri($"/repos/{Repository.Owner.Login}/{Repository.Name}/labels/{Name}", UriKind.Relative);
        
        internal static Label Create(KitHubSession session, Repository repository, JToken data)
        {
            if (data == null || data.Type == JTokenType.Null)
            {
                return null;
            }

            if (data is JObject obj && obj.TryGetValue("name", out JToken nameToken) && nameToken is JValue nameValue && nameValue.Value is string name)
            {
                Label label = GetOrCreate(session, repository, name);
                label.SetFromData(data);

                return label;
            }

            throw new KitHubDataException("The label object is invalid.", data);
        }

        internal static Label GetOrCreate(KitHubSession session, Repository repository, string name)
        {
            if (repository == null)
            {
                throw new ArgumentException("The repository of a label must not be null.");
            }

            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("The name of a label must not be null or empty.");
            }

            LabelKey key = new LabelKey(session, repository, name);
            lock (Cache)
            {
                if (Cache.TryGetValue(key, out Label existing))
                {
                    return existing;
                }
                else
                {
                    Label result = new Label(key);
                    Cache[key] = result;

                    return result;
                }
            }
        }

        private sealed class LabelKey
        {
            public LabelKey(KitHubSession session, Repository repository, string name)
            {
                Session = session;
                Repository = repository;
                Name = name;
            }

            public KitHubSession Session { get; }

            public Repository Repository { get; }

            public string Name { get; }

            public override bool Equals(object obj)
            {
                if (obj is LabelKey other)
                {
                    return Session == other.Session && Repository == other.Repository && Name == other.Name;
                }

                return false;
            }

            public override int GetHashCode()
            {
                return Session.GetHashCode() ^ Repository.GetHashCode() ^ Name.GetHashCode();
            }
        }
    }
}
