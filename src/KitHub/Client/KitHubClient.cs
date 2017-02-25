// -----------------------------------------------------------------------
// <copyright file="KitHubClient.cs" company="Niklas Karl">
// Copyright (c) Niklas Karl. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace KitHub.Client
{
    internal class KitHubClient
    {
        private const string BaseAddress = "https://github.com/";

        private const string BaseApiAddress = "https://api.github.com/";

        private HttpClient _client;

        internal KitHubClient(string token)
        {
            _client = new HttpClient()
            {
                BaseAddress = new Uri(BaseApiAddress, UriKind.Absolute),
                DefaultRequestHeaders =
                {
                    Accept =
                    {
                        new MediaTypeWithQualityHeaderValue("application/vnd.github.v3.raw+json")
                    },
                    UserAgent =
                    {
                        new ProductInfoHeaderValue(new ProductHeaderValue("KitHub", "1.0.0.0"))
                    }
                }
            };

            if (!string.IsNullOrEmpty(token))
            {
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("token", token);
            }
        }

        internal Uri BaseUri { get => new Uri(BaseAddress, UriKind.Absolute); }

        internal Uri BaseApiUri { get => new Uri(BaseApiAddress, UriKind.Absolute); }

        internal Task<KitHubResponse> GetAsync(KitHubRequest arg, CancellationToken cancellationToken)
        {
            return SendRequestAsync(arg, HttpMethod.Get, null, cancellationToken);
        }

        private static async Task<Exception> ConstructExceptionAsync(HttpResponseMessage response)
        {
            JToken content;
            using (Stream stream = await response.Content.ReadAsStreamAsync())
            using (JsonReader reader = new JsonTextReader(new StreamReader(stream)))
            {
                content = JToken.ReadFrom(reader);
            }

            string message = content.Value<string>("message");

            switch (response.StatusCode)
            {
                case HttpStatusCode.Unauthorized:
                    return new KitHubAuthorizationException(message, response.StatusCode, content.ToString(Formatting.Indented));
                default:
                    return new KitHubRequestException(message, response.StatusCode, content.ToString(Formatting.Indented));
            }
        }

        private async Task<KitHubResponse> SendRequestAsync(KitHubRequest arg, HttpMethod method, JToken data, CancellationToken cancellationToken)
        {
            HttpResponseMessage response = null;
            while (response == null)
            {
                if (NetworkInterface.GetIsNetworkAvailable())
                {
                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, arg.Uri);
                    request.Headers.IfModifiedSince = arg.LastModified;
                    if (arg.EntityTag != null)
                    {
                        request.Headers.IfNoneMatch.Add(new EntityTagHeaderValue(arg.EntityTag));
                    }

                    try
                    {
                        response = await _client.SendAsync(request, HttpCompletionOption.ResponseContentRead, cancellationToken);
                    }
                    catch (HttpRequestException)
                    {
                        response = null;
                    }

                    if (response != null && response.StatusCode >= HttpStatusCode.InternalServerError)
                    {
                        response = null;
                    }
                }

                if (response == null)
                {
                    await Task.Delay(100);
                    cancellationToken.ThrowIfCancellationRequested();
                }
            }

            switch (response.StatusCode)
            {
                case HttpStatusCode.Redirect:
                    method = HttpMethod.Get;
                    data = null;
                    goto case HttpStatusCode.RedirectKeepVerb;
                case HttpStatusCode.MovedPermanently:
                case HttpStatusCode.RedirectKeepVerb:
                    arg.Uri = response.Headers.Location;
                    return await SendRequestAsync(arg, method, data, cancellationToken);
            }

            if (response.StatusCode == HttpStatusCode.NotModified)
            {
                return new KitHubResponse()
                {
                    HasChanged = false,
                    Content = null,
                    EntityTag = arg.EntityTag,
                    LastModified = arg.LastModified,
                    Links = GetLinks(response.Headers)
                };
            }

            if ((int)response.StatusCode >= 200 && (int)response.StatusCode < 300)
            {
                JToken content = null;
                if ((response.Content.Headers.ContentLength ?? 0) > 0)
                {
                    if (response.Content.Headers.ContentType?.MediaType == "application/json")
                    {
                        string charset = response.Content.Headers.ContentType?.CharSet;
                        Encoding encoding = string.IsNullOrEmpty(charset) ? Encoding.UTF8 : Encoding.GetEncoding(charset);

                        using (Stream stream = await response.Content.ReadAsStreamAsync())
                        using (JsonReader reader = new JsonTextReader(new StreamReader(stream, encoding)))
                        {
                            content = JToken.ReadFrom(reader);
                        }
                    }
                }

                return new KitHubResponse()
                {
                    HasChanged = true,
                    Content = content,
                    EntityTag = response.Headers.ETag.Tag,
                    LastModified = response.Content.Headers.LastModified?.UtcDateTime,
                    Links = GetLinks(response.Headers)
                };
            }

            throw await ConstructExceptionAsync(response);
        }

        private IReadOnlyDictionary<string, Uri> GetLinks(HttpResponseHeaders headers)
        {
            if (headers.TryGetValues("Link", out IEnumerable<string> values))
            {
                SortedDictionary<string, Uri> links = new SortedDictionary<string, Uri>();
                foreach (string value in values)
                {
                    string pattern = @"\G\s*<([^>]*)>\s*;\s*rel\s*=\s*""([^""]*)""\s*,?";
                    MatchCollection matches = Regex.Matches(value, pattern);
                    foreach (Match match in matches)
                    {
                        if (Uri.TryCreate(match.Groups[1].Value, UriKind.RelativeOrAbsolute, out Uri uri))
                        {
                            links[match.Groups[2].Value] = uri;
                        }
                    }
                }

                return links;
            }

            return null;
        }
    }
}
