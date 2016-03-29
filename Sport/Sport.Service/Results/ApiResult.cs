using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using WebGrease.Css.Extensions;

namespace Sport.Service.Results
{
    public class ApiResult : IHttpActionResult
    {
        private readonly IHttpActionResult _actionResult;

        public ApiResult(Exception ex, bool fatal = false)
        {
            Successful = false;
            while (ex.InnerException != null)
            {
                ex = ex.InnerException;
            }
            ErrorDesc = ex.Message;
            if (fatal)
            {
                Log = ex.StackTrace;
                //TODO: log               
            }
        }

        public ApiResult(IHttpActionResult action)
        {
            _actionResult = action;
            Successful = true;
        }

        public ApiResult(object data)
        {
            Data = data;
            Successful = true;
        }

        public ApiResult(IdentityResult result, ModelStateDictionary modelState, object data)
        {
            if (result == null)
            {
                result = new IdentityResult();
            }
            else
            {
                Successful = result.Succeeded;
            }

            if (!Successful)
            {
                if (result.Errors != null && result.Errors.Any())
                {
                    if (result.Errors.Count() == 1)
                        ErrorDesc = result.Errors.First();
                    else
                    {
                        ErrorDesc = result.Errors;
                    }
                }
                else
                {
                    var errors = new List<KeyValuePair<string, string>>();
                    modelState.ForEach(
                        x =>
                            x.Value.Errors.ForEach(
                                y =>
                                    errors.Add(new KeyValuePair<string, string>(GetProperyName(x.Key),
                                        y.ErrorMessage ?? y.Exception?.Message))));

                    if (errors.Count() == 1)
                        ErrorDesc = errors.First();
                    else
                    {
                        ErrorDesc = errors;
                    }
                }
            }
            else
            {
                Data = data;
            }
        }

        public ApiResult()
        {

        }

        [JsonProperty("successful")]
        public bool Successful { get; set; }

        [JsonProperty("errorDesc")]
        public object ErrorDesc { get; set; }

        [JsonProperty("data")]
        public object Data { get; set; }

        [JsonProperty("auth")]
        public object Auth { get; set; }

        [JsonProperty("log")]
        public object Log { get; set; }

        public async Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            if (_actionResult != null)
            {
                var response = await _actionResult.ExecuteAsync(cancellationToken);
                Data = response.Content;
                Successful = response.IsSuccessStatusCode;
            }

            return
                new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content =
                        new StringContent(
                            JsonConvert.SerializeObject(this,
                                GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings),
                            Encoding.UTF8, "application/json")
                };
        }

        private static string GetProperyName(string key)
        {
            if (key.Contains("."))
            {
                return Path.GetExtension(key).Trim('.');
            }
            return key;
        }

        public static ApiResult Exception(Exception ex, bool fatal = false)
        {
            return new ApiResult(ex, fatal);
        }

        public static ApiResult Result(IdentityResult result, ModelStateDictionary modelState, object data)
        {
            return new ApiResult(result, modelState, data);
        }

        public static ApiResult Ok()
        {
            return new ApiResult {Successful = true};
        }

        public static ApiResult Ok(object data)
        {
            return new ApiResult(data);
        }
    }
}