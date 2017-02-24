using System;
using Newtonsoft.Json.Linq;

namespace KitHub
{
    [ListModel(Initializer = typeof(User.DefaultInitializer))]
    public sealed class UserList : ListModelBase<User>
    {
        internal UserList(KitHubSession session, Uri refreshUri) :
            base(session)
        {
            RefreshUri = refreshUri;
            RefreshAsync();
        }

        protected override Uri RefreshUri { get; }
    }
}
