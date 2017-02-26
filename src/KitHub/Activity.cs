using System;
using System.IO;
using KitHub.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace KitHub
{
    /// <summary>
    /// A activity issues by GitHub.
    /// </summary>
    public class Activity : SerializableModelBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Activity"/> class.
        /// </summary>
        /// <param name="session"></param>
        protected Activity(KitHubSession session)
            : base(session)
        {
        }

        /// <summary>
        /// Gets the id of the activity.
        /// </summary>
        [ModelProperty("id")]
        public long? Id { get; private set; }

        /// <summary>
        /// Gets the type of the activity.
        /// </summary>
        [ModelProperty("type")]
        public string Type { get; private set; }

        /// <summary>
        /// Gets the actor of the activity.
        /// </summary>
        [ModelProperty("actor", Initializer = typeof(ActorInitializer))]
        public User Actor { get; private set; }

        /// <summary>
        /// Gets the repository the activity was issued for.
        /// </summary>
        [ModelProperty("repo", Initializer = typeof(RepositoryInitializer))]
        public Repository Repository { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the activity is public or not.
        /// </summary>
        [ModelProperty("public")]
        public bool? IsPublic { get; private set; }

        /// <summary>
        /// Gets the timestamp when the activity was created.
        /// </summary>
        [ModelProperty("created_at")]
        public DateTimeOffset? CreatedAt { get; private set; }

        internal class DefaultInitializer : IModelInitializer
        {
            public object InitializeModel(BindableBase self, JToken data)
            {
                Activity activity = new Activity(self.Session);
                activity.SetFromData(data);
                return activity;
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
                    throw new InvalidDataException();
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
                    throw new InvalidDataException();
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
