// -----------------------------------------------------------------------
// <copyright file="Event.cs" company="Niklas Karl">
// Copyright (c) Niklas Karl. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using System;
using KitHub.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace KitHub
{
    /// <summary>
    /// A event issues by GitHub.
    /// </summary>
    public class Event : SerializableModelBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Event"/> class.
        /// </summary>
        /// <param name="session"></param>
        protected Event(KitHubSession session)
            : base(session)
        {
        }

        /// <summary>
        /// Gets the id of the event.
        /// </summary>
        [ModelProperty("id")]
        public long? Id { get; protected set; }

        /// <summary>
        /// Gets the type of the event.
        /// </summary>
        [ModelProperty("type")]
        public string Type { get; protected set; }

        /// <summary>
        /// Gets the actor of the event.
        /// </summary>
        [ModelProperty("actor", Initializer = typeof(ActorInitializer))]
        public User Actor { get; protected set; }

        /// <summary>
        /// Gets the repository the event was issued for.
        /// </summary>
        [ModelProperty("repo", Initializer = typeof(RepositoryInitializer))]
        public Repository Repository { get; protected set; }

        /// <summary>
        /// Gets a value indicating whether the event is public or not.
        /// </summary>
        [ModelProperty("public")]
        public bool? IsPublic { get; protected set; }

        /// <summary>
        /// Gets the timestamp when the event was created.
        /// </summary>
        [ModelProperty("created_at")]
        public DateTimeOffset? CreatedAt { get; protected set; }

        internal class DefaultInitializer : IModelInitializer
        {
            public object InitializeModel(BindableBase self, JToken data)
            {
                Event result = null;
                if (data is JObject obj && obj.TryGetValue("type", out JToken typeToken))
                {
                    if (typeToken is JValue typeValue && typeValue.Value is string type)
                    {
                        switch (type)
                        {
                            case "PushEvent":
                                result = new PushEvent(self.Session);
                                break;
                            default:
                                result = new Event(self.Session);
                                break;
                        }

                        result.SetFromData(data);

                        return result;
                    }
                }

                throw new KitHubDataException("The event object is invalid.", data);
            }
        }

        private class ActorInitializer : IModelInitializer
        {
            public object InitializeModel(BindableBase self, JToken data)
            {
                UserEntity entity;
                try
                {
                    entity = data.ToObject<UserEntity>();
                }
                catch (JsonException)
                {
                    throw new KitHubDataException("The user object is invalid.", data);
                }

                User user = User.GetOrCreate(self.Session, entity.Login);
                user.Id = entity.Id;
                if (Uri.TryCreate(entity.AvatarUrl, UriKind.Absolute, out Uri avatarUrl))
                {
                    user.AvatarUrl = avatarUrl;
                }

                return user;
            }

            private class UserEntity
            {
                [JsonProperty(PropertyName = "id", Required = Required.Always)]
                public long Id { get; set; }

                [JsonProperty(PropertyName = "login", Required = Required.Always)]
                public string Login { get; set; }

                [JsonProperty(PropertyName = "display_login")]
                public string DisplayLogin { get; set; }

                [JsonProperty(PropertyName = "gravatar_id")]
                public string GravatarId { get; set; }

                [JsonProperty(PropertyName = "url")]
                public string Url { get; set; }

                [JsonProperty(PropertyName = "avatar_url")]
                public string AvatarUrl { get; set; }
            }
        }

        private class RepositoryInitializer : IModelInitializer
        {
            public object InitializeModel(BindableBase self, JToken data)
            {
                RepositoryEntity entity;
                try
                {
                    entity = data.ToObject<RepositoryEntity>();
                }
                catch (JsonException)
                {
                    throw new KitHubDataException("The repository object is invalid.", data);
                }

                int index = entity.FullName.IndexOf('/');
                string login = entity.FullName.Remove(index);
                string name = entity.FullName.Substring(index + 1);

                User owner = User.GetOrCreate(self.Session, login);
                Repository repository = Repository.GetOrCreate(self.Session, owner, name);
                repository.Id = entity.Id;

                return repository;
            }

            private class RepositoryEntity
            {
                [JsonProperty(PropertyName = "id", Required = Required.Always)]
                public long Id { get; set; }

                [JsonProperty(PropertyName = "name", Required = Required.Always)]
                public string FullName { get; set; }

                [JsonProperty(PropertyName = "url")]
                public string Url { get; set; }
            }
        }
    }
}
