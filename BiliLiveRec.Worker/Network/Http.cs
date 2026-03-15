using System;
using System.Collections.Generic;
using System.Text;

namespace BiliLiveRec.Worker.Network
{
    internal class Http
    {
        public static HttpClientHandler httpClientHandler = new HttpClientHandler()
        {
            Proxy = null,
            UseProxy = false
        };
        public static HttpClient _httpClient = new(httpClientHandler);
        public static void Init()
        {
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:148.0) Gecko/20100101 Firefox/148.0");
        }
        public static async Task<Stream> GetStream(string url)
        {
            HttpClient _httpClient = new();
            HttpResponseMessage responseMessage = await _httpClient.SendAsync(new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(url)
            });
            responseMessage.EnsureSuccessStatusCode();
            return await responseMessage.Content.ReadAsStreamAsync();

        }
        public static async Task<string> GetString(string url)
        {
            HttpResponseMessage responseMessage = await _httpClient.SendAsync(new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(url)
            });
            responseMessage.EnsureSuccessStatusCode();
            return await responseMessage.Content.ReadAsStringAsync();

        }
        public static async Task<HttpResponseMessage> TryGetResp(string url)
        {
            HttpResponseMessage responseMessage = await _httpClient.SendAsync(new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(url)
            });
            return responseMessage;
        }
    }
}
