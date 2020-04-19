using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Relay;
using ITSS.Repository.Relay.Interfaces;

namespace ITSS.Repository.Relay
{
    public class RelaySender : IRelaySender
    {
        private readonly HttpClient _httpClient;
        private readonly RelayParam _relayParam;
        private readonly Uri _requestUri;
        public RelaySender(RelayParam relayParam)
        {
            _httpClient = new HttpClient();
            _relayParam = relayParam;
            _requestUri = GetUri();
            PrepareWebParameters();
        }

        public async Task<string> SendRequest(string query, CancellationToken ct)
        {
            var content = CreateHttpContent(query);
            var response = await _httpClient.PostAsync(_requestUri.AbsoluteUri, content, ct);

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Request failed - {response.StatusCode} {response.ReasonPhrase}");

            return await response.Content.ReadAsStringAsync();
        }

        private Uri GetUri()
        {
            var requestUri = new Uri(string.Format("https://{0}/{1}", _relayParam.AzureRelayNamespace, _relayParam.AzureRelayConnectionName));
            return requestUri;
        }

        private async void PrepareWebParameters()
        {         
            var token = await GetToken(_requestUri.AbsoluteUri);
            _httpClient.DefaultRequestHeaders.Add("ServiceBusAuthorization", token);
        }

        private async Task<string> GetToken(string uri)
        {
            var tokenProvider = TokenProvider.CreateSharedAccessSignatureTokenProvider(_relayParam.AzureRelayKeyName, _relayParam.AzureRelayKey);
            var token = (await tokenProvider.GetTokenAsync(uri, TimeSpan.FromHours(1))).TokenString;
            return token;
        }

        private static StringContent CreateHttpContent(string content)
        {
            StringContent httpContent = null;

            if (!string.IsNullOrWhiteSpace(content))
            {
                //var json = JsonConvert.SerializeObject(content);
                // We don't use serialization here since we don't send a json file. We do send just the query text.
                // In the future we need to send SearchParam object as json.
                httpContent = new StringContent(content);
                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            }
            return httpContent;
        }
    }
}
