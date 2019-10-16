using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using XenaExchange.Client.Rest.Exceptions;
using XenaExchange.Client.Serialization;

namespace XenaExchange.Client.Rest
{
    public abstract class RestClientBase
    {
        public const string HttpClientName = "xena.exchange";

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly HttpClient _httpClient;

        protected HttpClient HttpClient => _httpClientFactory?.CreateClient(HttpClientName) ?? _httpClient;

        protected RestClientBase(IHttpClientFactory httpClientFactory = null, HttpClient httpClient = null)
        {
            if (httpClientFactory == null && httpClient == null)
            {
                var msg = $"Either {nameof(httpClientFactory)} or {nameof(httpClient)} should be specified.";
                throw new ArgumentNullException(msg);
            }

            _httpClientFactory = httpClientFactory;
            _httpClient = httpClient;
        }

        protected HttpRequestMessage BuildRequestBase(string path, HttpMethod method, string query = null)
        {
            var httpClient = HttpClient;
            var uriBuilder = new UriBuilder(httpClient.BaseAddress) { Path = path };
            if (!string.IsNullOrWhiteSpace(query))
                uriBuilder.Query = query;

            return new HttpRequestMessage(method, uriBuilder.Uri);
        }

        protected async Task<TResult> SendAsyncBase<TResult>(
            HttpRequestMessage request,
            ISerializer serializer,
            CancellationToken cancellationToken = default)
        {
            var response = await HttpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                    var responseStr = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    return serializer.Deserialize<TResult>(responseStr);
                case var _ when response.StatusCode < HttpStatusCode.BadRequest:
                    throw new RestClientException(
                        $"Only {HttpStatusCode.OK} successful code is supported",
                        response.StatusCode);

                default:
                    var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    var error = content;

                    if (!string.IsNullOrWhiteSpace(content))
                    {
                        try
                        {
                            error = JsonConvert.DeserializeObject<ErrorResponse>(content)?.Error;
                        }
                        catch (Exception)
                        {
                        } // If it's not a json, ok, let's return content as is
                    }

                    throw new RestClientException(error, response.StatusCode, request.RequestUri.AbsoluteUri);
            }
        }
    }
}