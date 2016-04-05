using MvvmCross.Core.ViewModels;
using Sport.Core.Mvvm;
using Sport.Core.ViewModels.Base;

namespace Sport.Core.ViewModels.Home
{
	public class InfoViewModel : BaseViewModel
    {
        public InfoViewModel()
        {
            Info = "This is info for you...";
        }

	    public string Info { get; private set; }

        private MvxCommand showThirdViewModelCommand;

        public MvxCommand ShowThirdViewModelCommand
        {
            get
            {
                showThirdViewModelCommand = showThirdViewModelCommand ?? new MvxCommand(DoShowThirdViewModel);
                return showThirdViewModelCommand;
            }
        }

        private void DoShowThirdViewModel()
        {
            ShowViewModel<ThirdViewModel>();
        }


    }
}