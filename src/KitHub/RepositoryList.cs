using System;
using System.Threading;
using System.Threading.Tasks;

namespace KitHub
{
    [ListModel(Initializer = typeof(Repository.DefaultInitializer))]
    public sealed class RepositoryList : ListModelBase<Repository>
    {
        private RepositoryList(KitHubSession session, Uri refreshUri) :
            base(session)
        {
            RefreshUri = refreshUri;
        }

        internal static async Task<RepositoryList> CreateAsync(KitHubSession session, Uri refreshUri, CancellationToken cancellationToken)
        {
            RepositoryList list = new RepositoryList(session, refreshUri);
            await list.RefreshAsync(cancellationToken);
            
            return list;
        }

        protected override Uri RefreshUri { get; }
    }
}
