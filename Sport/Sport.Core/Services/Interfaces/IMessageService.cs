using System.Threading.Tasks;
using MvvmCross.Core.ViewModels;

namespace Sport.Core.Services.Interfaces
{
    public interface IMessageService
    {
        void StartProgress(string message);
        void StopProgress();
        Task ShowMessageAsync(string content, string title);
        Task<bool> ShowConfirmMessageAsync(string content, string title, string yes = "yes", string no = "no");
        Task ShowNoInternetAsync();
        Task<object> ShowCustomDialog<T>() where T : IMvxViewModel;
    }
}
