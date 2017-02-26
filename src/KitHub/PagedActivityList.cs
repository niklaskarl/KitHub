using System;
using System.Threading;
using System.Threading.Tasks;
using KitHub.Core;

namespace KitHub
{
    /// <summary>
    /// A paged list of <see cref="Activity"/> objects.
    /// </summary>
    [ListModel(Initializer = typeof(Activity.DefaultInitializer))]
    public class PagedActivityList : PagedListModelBase<Activity>
    {
        private PagedActivityList(KitHubSession session, Uri uri)
            : base(session)
        {
            Uri = uri;
        }

        /// <inheritdoc/>
        protected override Uri Uri { get; }

        internal static async Task<PagedActivityList> CreateAsync(KitHubSession session, Uri uri, CancellationToken cancellationToken)
        {
            PagedActivityList list = new PagedActivityList(session, uri);
            await list.InitializeAsync(cancellationToken);

            return list;
        }
    }
}
