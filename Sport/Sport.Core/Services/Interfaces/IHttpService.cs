using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sport.Core.Services.Interfaces
{
    public interface IHttpService
    {
        string AccessToken { get; set; }

        Task<string> GetAsync(string url);
        Task<string> GetAsync(string url, Dictionary<string, string> data);
        Task<string> GetAsync(string url, CancellationToken token);
        Task<string> PostAsync(string url, string data);
        Task<string> PostAsync(string url, Dictionary<string, string> data);
        Task<string> PostAsync(string url, string data, string dataType);
    }
}