using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace KitHub
{
    public abstract partial class ModelBase : BindableBase
    {
        private IDictionary<string, object> _properties;
        
        private SemaphoreSlim _sync;

        private bool _refreshing;

        private string _entityTag;

        private DateTime? _lastModified;

        internal ModelBase(KitHubSession session)
        {
            _properties = new Dictionary<string, object>();
            Session = session;
            
            _sync = new SemaphoreSlim(1);
        }

        internal KitHubSession Session { get; }

        protected abstract Uri RefreshUri { get; }

        protected abstract object Key { get; }

        public Task RefreshAsync(CancellationToken cancellationToken)
        {
            return Task.Run(() => RefreshInternalAsync(cancellationToken), cancellationToken);
        }
        
        internal void SetFromResponse(KitHubResponse response)
        {
            _entityTag = response.EntityTag;
            _lastModified = response.LastModified;

            if (response.HasChanged)
            {
                SetFromData(response.Content);
            }
        }

        protected object GetProperty([CallerMemberName]string propertyName = null)
        {
            if (_properties.TryGetValue(propertyName, out object value))
            {
                return value;
            }
            else
            {
                RefreshAsync(CancellationToken.None);
                return null;
            }
        }

        protected void SetProperty(object value, [CallerMemberName]string propertyName = null)
        {
            if (_properties.TryGetValue(propertyName, out object previous))
            {
                if (!Equals(previous, value))
                {
                    _properties[propertyName] = value;
                    OnPropertyChanged(propertyName);
                }
            }
            else
            {
                _properties[propertyName] = value;
                OnPropertyChanged(propertyName);
            }
        }

        private async Task RefreshInternalAsync(CancellationToken cancellationToken)
        {
            _refreshing = true;
            await _sync.WaitAsync(cancellationToken);

            try
            {
                if (_refreshing)
                {
                    KitHubRequest request = new KitHubRequest()
                    {
                        Uri = RefreshUri,
                        EntityTag = _entityTag,
                        LastModified = _lastModified
                    };

                    KitHubResponse response = await Session.Client.GetAsync(request, cancellationToken);
                    if (response.HasChanged)
                    {
                        await Session.DispatchAsync(() => SetFromResponse(response));
                    }

                    _refreshing = false;
                }
            }
            finally
            {
                _sync.Release();
            }
        }
    }
}
