using System.Threading.Tasks;
using Sport.Shared;

namespace Sport.Core.Services.Interfaces
{
    public interface IAuthService
    {
        LoginProvider LoginProvider { get; set; }
        Task<LoginSession> LoginAsync(LoginProvider provider);
    }
}
