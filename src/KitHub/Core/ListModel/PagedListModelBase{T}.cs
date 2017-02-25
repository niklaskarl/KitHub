// -----------------------------------------------------------------------
// <copyright file="PagedListModelBase{T}.cs" company="Niklas Karl">
// Copyright (c) Niklas Karl. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace KitHub
{
    public abstract class PagedListModelBase<T> : BindableBase
    {
        private Page<T>[] _pages;

        private string _pagedUri;

        private SemaphoreSlim _sync;

        internal PagedListModelBase(KitHubSession session)
            : base(session)
        {
            _sync = new SemaphoreSlim(1);
        }

        public int PageCount => _pages.Length;

        protected abstract Uri Uri { get; }

        public async Task<Page<T>> GetPageAsync(int number, bool refresh = false, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (number >= PageCount)
            {
                throw new IndexOutOfRangeException();
            }

            Page<T> result = null;
            if (number < _pages.Length)
            {
                Page<T> page = _pages[number];
                if (page != null)
                {
                    result = page.Clone();
                    if (refresh)
                    {
                        await RefreshPageInternalAsync(result, cancellationToken);
                    }
                }
            }

            if (result == null)
            {
                result = new Page<T>(this, number);
                await RefreshPageInternalAsync(result, cancellationToken);
            }

            _pages[number] = result;
            return result;
        }

        internal async Task RefreshPageAsync(Page<T> page, CancellationToken cancellationToken)
        {
            await Task.Run(() => RefreshPageInternalAsync(page, cancellationToken), cancellationToken);
        }

        protected Task InitializeAsync(CancellationToken cancellationToken)
        {
            Uri uri = Uri;
            if (!uri.IsAbsoluteUri)
            {
                uri = new Uri(Session.Client.BaseApiUri, uri);
            }

            _pagedUri = uri.AbsoluteUri;
            return RefreshPageInternalAsync(new Page<T>(this, 1), cancellationToken);
        }

        private static IModelInitializer<T> CreateInitializer(ListModelAttribute attribute)
        {
            ConstructorInfo constructor = attribute.Initializer?.GetTypeInfo()?.GetConstructor(new Type[0]);
            return constructor?.Invoke(null) as IModelInitializer<T>;
        }

        private async Task RefreshPageInternalAsync(Page<T> page, CancellationToken cancellationToken)
        {
            await _sync.WaitAsync(cancellationToken);
            try
            {
                KitHubRequest request = new KitHubRequest()
                {
                    Uri = new Uri(string.Format(_pagedUri, page.Number + 1)),
                    EntityTag = page.EntityTag,
                    LastModified = page.LastModified
                };

                KitHubResponse response = await Session.Client.GetAsync(request, cancellationToken);
                if (response.HasChanged)
                {
                    await Session.DispatchAsync(() =>
                    {
                        int pageCount = ParseLinkHeader(page.Number, response);

                        page.EntityTag = response.EntityTag;
                        page.LastModified = response.LastModified;
                        SetItemsFromData(page, response.Content);

                    _pages = new Page<T>[pageCount];
                        _pages[page.Number] = page;
                    });
                }
            }
            finally
            {
                _sync.Release();
            }
        }

        private int ParseLinkHeader(int page, KitHubResponse response)
        {
            if (response.Links == null)
            {
                throw new InvalidDataException("The paged response did not contain a link header.");
            }

            int result;
            string pattern = @"\A(?<pre>[^\?]*\?([^&]*&)*page=)(?<page>[^&]*)(?<suf>.*)\z";
            if (response.Links.TryGetValue("last", out Uri last))
            {
                Match match = Regex.Match(last.AbsoluteUri, pattern);
                if (match.Success && int.TryParse(match.Groups["page"].Value, out int pageCount))
                {
                    result = pageCount;
                    _pagedUri = match.Groups["pre"].Value + "{0}" + match.Groups["suf"].Value;
                }
                else
                {
                    result = page + 1;
                    _pagedUri = last.AbsoluteUri;
                }
            }
            else if (response.Links.TryGetValue("prev", out Uri prev))
            {
                result = page + 1;
                Match match = Regex.Match(prev.AbsoluteUri, pattern);
                if (match.Success && int.TryParse(match.Groups["page"].Value, out int pageCount))
                {
                    _pagedUri = match.Groups["pre"].Value + "{0}" + match.Groups["suf"].Value;
                }
                else
                {
                    _pagedUri = prev.AbsoluteUri;
                }
            }
            else
            {
                throw new InvalidDataException("The paged response did contain neither a 'last' nor a 'prev' link header entry.");
            }

            return result;
        }

        private void SetItemsFromData(Page<T> page, JToken data)
        {
            if (data is JArray array)
            {
                IReadOnlyList<T> items = null;

                TypeInfo type = GetType().GetTypeInfo();
                ListModelAttribute attribute = type.GetCustomAttribute<ListModelAttribute>(true);
                if (attribute != null)
                {
                    IModelInitializer<T> initializer = CreateInitializer(attribute);
                    if (initializer != null)
                    {
                        items = InitializeItemsFromData(array, initializer);
                    }
                }

                if (items == null)
                {
                    items = InitializeItemsFromData(array);
                }

                page.UpdateList(items);
            }
            else
            {
                throw new InvalidDataException();
            }
        }

        private IReadOnlyList<T> InitializeItemsFromData(JArray data)
        {
            List<T> items = new List<T>(data.Count);
            foreach (JToken item in data)
            {
                items.Add(item.ToObject<T>(KitHubSession.Serializer));
            }

            return items;
        }

        private IReadOnlyList<T> InitializeItemsFromData(JArray data, IModelInitializer<T> initializer)
        {
            List<T> items = new List<T>(data.Count);
            foreach (JToken item in data)
            {
                items.Add(initializer.InitializeModel(this, item));
            }

            return items;
        }
    }
}
