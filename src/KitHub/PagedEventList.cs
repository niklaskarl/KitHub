using System;
using System.Threading;
using System.Threading.Tasks;
using KitHub.Core;

namespace KitHub
{
    /// <summary>
    /// A paged list of <see cref="Event"/> objects.
    /// </summary>
    [ListModel(Initializer = typeof(Event.DefaultInitializer))]
    public class PagedEventList : PagedListModelBase<Event>
    {
        private PagedEventList(KitHubSession session, Uri uri)
            : base(session)
        {
            Uri = uri;
        }

        /// <inheritdoc/>
        protected override Uri Uri { get; }

        internal static async Task<PagedEventList> CreateAsync(KitHubSession session, Uri uri, CancellationToken cancellationToken)
        {
            PagedEventList list = new PagedEventList(session, uri);
            await list.InitializeAsync(cancellationToken);

            return list;
        }
    }
}
