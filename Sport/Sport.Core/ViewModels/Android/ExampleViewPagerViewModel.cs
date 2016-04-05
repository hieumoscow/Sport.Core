using Sport.Core.Mvvm;

namespace Sport.Core.ViewModels.Android
{
	public class ExampleViewPagerViewModel : BaseViewModel
    {
        public RecyclerViewModel Recycler { get; private set; }

        public ExampleViewPagerViewModel()
        {
            Recycler = new RecyclerViewModel();
        }
    }
}