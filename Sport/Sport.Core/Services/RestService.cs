using System.Diagnostics;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Sport.Core.Exceptions;
using Sport.Core.Models.EndPoints;

namespace Sport.Core.Services
{
    public class RestService
    {
        protected readonly HttpService HttpService;
        public const string LOCAL_HOST = "http://hoconha.com/";
        public const string PRO_HOST = "http://hoconha.com/";
        public const string HOST = LOCAL_HOST;
        private readonly JsonSerializerSettings _settings;

        public RestService()
        {
            HttpService = new HttpService("");
            _settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };
        }

        #region BASE
        protected async Task<T> PostAsync<T>(string api, object obj)
        {
            var data = JsonConvert.SerializeObject(obj, _settings);
            Debug.WriteLine($"POST({api}): {data}");
            var response =
                await HttpService.PostAsync(HOST + api, data, "application/json");
            Debug.WriteLine($"RESPONSE({api}): {response}");
            var temp = JsonConvert.DeserializeObject<ApiResponse<T>>(response);
            if (temp.Successful)
            {
                if (temp.Auth != null)
                {
                    App.Data.Session = temp.Auth;
                    HttpService.AccessToken = App.Data.Session.AccessToken;
                }
                return temp.Data;
            }
            throw new ApiException<T>(temp);
        }
        protected async Task<T> GetAsync<T>(string api)
        {
            Debug.WriteLine($"GET({api})");
            var response = await HttpService.GetAsync(HOST + api);
            Debug.WriteLine($"RESPONSE({api}): {response}");
            var temp = JsonConvert.DeserializeObject<ApiResponse<T>>(response);
            if (temp.Successful)
            {
                if (temp.Auth != null)
                {
                    App.Data.Session = temp.Auth;
                    HttpService.AccessToken = App.Data.Session.AccessToken;
                }
                return temp.Data;
            }
            throw new ApiException<T>(temp);
        }
        #endregion
    }
}