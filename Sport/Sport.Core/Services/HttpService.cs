using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Sport.Core.Exceptions;
using Sport.Core.Models.EndPoints;
using Sport.Core.Services.Interfaces;
using NetworkInterface = System.Net.NetworkInformation.NetworkInterface;

namespace Sport.Core.Services
{
    public class HttpService :  IHttpService
    {
        private readonly HttpClient _httpClient;
        private CancellationTokenSource _cts = new CancellationTokenSource();
        private readonly CookieContainer _cookieContainer;
        private readonly string _baseAddress;

        public HttpService(string baseAddress)
        {
            _baseAddress = baseAddress;
            _cookieContainer = new CookieContainer();

            _handler = new HttpClientHandler
            {
                AllowAutoRedirect = true,
                UseCookies = true,
                CookieContainer = _cookieContainer
            };

            //if (_handler.SupportsAutomaticDecompression)
            //    _handler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;


            _httpClient = new HttpClient(_handler);
            //_httpClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (compatible; MSIE 10.0; Windows Phone 8.0; Trident/6.0; IEMobile/10.0; ARM; Touch; NOKIA; Lumia 920)");
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "application/json, application/xhtml+xml, */*");
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Language", "en-US");
        }
        public string AccessToken { get; set; }
        public void CancelCurrentRequest()
        {
            if (_cts != null)
                _cts.Cancel();
        }

        public event EventHandler ConnectionFailed;
        private readonly HttpClientHandler _handler;

        public void AddCookie(string key, string value)
        {
            _cookieContainer.SetCookies(new Uri(_baseAddress), key + "=" + value);
        }

        public async Task<string> GetNoRedirectAsync(string url)
        {
            _handler.AllowAutoRedirect = false;
            var response = await _httpClient.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();
            return content;
        }
        public async Task<string> GetAsync(string url)
        {
            try
            {
                if (!NetworkInterface.GetIsNetworkAvailable())
                    throw new NoInternetConnection();
                TryAddAuth();
                var response = await _httpClient.GetAsync(url, _cts.Token);
                var content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return JsonConvert.SerializeObject(new Response<object>()
                    {
                        Log = content,
                        Successful = false,
                        ErrorDesc = response.StatusCode.ToString()
                    });
                }
                return content;
            }
            catch (Exception e)
            {
                ConnectionFailed?.Invoke(e, EventArgs.Empty);
                throw;
            }
        }

        public async Task<string> PutAsync(string url, HttpContent content)
        {
            try
            {
                TryAddAuth();
                var response = await _httpClient.PutAsync(url, content);
               
                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception e)
            {
                if (ConnectionFailed != null)
                    ConnectionFailed(e, EventArgs.Empty);
                throw;
            }
        }

        public async Task<string> GetAsync(string url, CancellationToken token)
        {
            try
            {
                if (!NetworkInterface.GetIsNetworkAvailable())
                    throw new NoInternetConnection();
                TryAddAuth();

                var response = await _httpClient.GetAsync(url, token);

               

                var content = await response.Content.ReadAsStringAsync();
                return content;
            }
            catch (Exception e)
            {
                if (ConnectionFailed != null)
                    ConnectionFailed(e, EventArgs.Empty);
                throw;
            }
        }

        public async Task<string> PostAsync(string url, string data)
        {
            try
            {
                if (!NetworkInterface.GetIsNetworkAvailable())
                    throw new NoInternetConnection();

                TryAddAuth();
                _cts = new CancellationTokenSource();
                var response = await _httpClient.PostAsync(url, new StringContent(data), _cts.Token);

               

                var content = await response.Content.ReadAsStringAsync();
                return content;
            }
            catch (Exception e)
            {
                if (ConnectionFailed != null)
                    ConnectionFailed(e, EventArgs.Empty);
                throw;
            }
        }

        private void TryAddAuth()
        {
            if (string.IsNullOrEmpty(AccessToken))
            {
                _httpClient.DefaultRequestHeaders.Authorization = null;
            }
            else
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", AccessToken);

        }

        // ReSharper disable once MethodOverloadWithOptionalParameter
        public async Task<string> PostAsync(string url, string data, string dataType)
        {
            try
            {
                if (!NetworkInterface.GetIsNetworkAvailable())
                    throw new NoInternetConnection();
                TryAddAuth();
                _cts = new CancellationTokenSource();
                var postData = new StringContent(data);
                postData.Headers.ContentType = new MediaTypeHeaderValue(dataType);
                var response = await _httpClient.PostAsync(url, postData, _cts.Token);
                var content = await response.Content.ReadAsStringAsync();
                // response.EnsureSuccessStatusCode();
                if (!response.IsSuccessStatusCode)
                {
                    return JsonConvert.SerializeObject(new Response<object>()
                    {
                        Log = content,
                        Successful = false,
                        ErrorDesc = response.StatusCode.ToString()
                    });
                }
                
                return content;
            }
            catch (Exception e)
            {
                ConnectionFailed?.Invoke(e, EventArgs.Empty);
                throw;
            }
        }


        public async Task<string> PostAsync(string url, Dictionary<string, string> data)
        {
            try
            {
                if (!NetworkInterface.GetIsNetworkAvailable())
                    throw new NoInternetConnection();
                TryAddAuth();
                _cts = new CancellationTokenSource();
                var response = await _httpClient.PostAsync(url, new CustomFormUrlEncodedContent(data), _cts.Token);

               

                var content = await response.Content.ReadAsStringAsync();
                return content;
            }
            catch (Exception e)
            {
                if (ConnectionFailed != null)
                    ConnectionFailed(e, EventArgs.Empty);
                throw;
            }
        }

        public async Task<string> GetAsync(string url, Dictionary<string, string> data)
        {
            var dataContent = new FormUrlEncodedContent(data);
            var param = await dataContent.ReadAsStringAsync();
            return await GetAsync(url + "?" + param);
        }
    }

    public class CustomFormUrlEncodedContent : ByteArrayContent
    {
        public CustomFormUrlEncodedContent(IEnumerable<KeyValuePair<string, string>> nameValueCollection)
            : base(GetContentByteArray(nameValueCollection))
        {
            Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
        }

        private static byte[] GetContentByteArray(IEnumerable<KeyValuePair<string, string>> nameValueCollection)
        {
            if (nameValueCollection == null)
            {
                throw new ArgumentNullException("nameValueCollection");
            }

            var stringBuilder = new StringBuilder();
            foreach (var current in nameValueCollection)
            {
                if (stringBuilder.Length > 0)
                {
                    stringBuilder.Append('&');
                }

                stringBuilder.Append(Encode(current.Key));
                stringBuilder.Append('=');
                stringBuilder.Append(Encode(current.Value));
            }
            return Encoding.UTF8.GetBytes(stringBuilder.ToString());
        }

        private static string Encode(string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                return string.Empty;
            }
            return WebUtility.UrlEncode(data).Replace("%20", "+");
        }
    }



}
