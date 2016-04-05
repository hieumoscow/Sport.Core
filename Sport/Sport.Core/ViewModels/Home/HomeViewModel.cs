using MvvmCross.Core.ViewModels;
using Sport.Core.Mvvm;
using Sport.Core.ViewModels.Android;
using Sport.Core.ViewModels.Base;

namespace Sport.Core.ViewModels.Home
{
	public class HomeViewModel : BaseViewModel
    {
        public HomeViewModel() 
        {
            Recycler = new RecyclerViewModel();
        }

        /// <summary>Gets the recycler.</summary>
        /// <value>The recycler.</value>
        public RecyclerViewModel Recycler { get; private set; }

	    public MvxCommand GoToInfoCommand
	    {
	        get { return new MvxCommand(() => ShowViewModel<InfoViewModel>());}
	    }
    }
}