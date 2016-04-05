using Foundation;
using MvvmCross.iOS.Support.SidePanels;
using Sport.Core.ViewModels.Base;

namespace Sport.Touch.Views.Base
{
    [Register("MainView")]
	[MvxPanelPresentation(MvxPanelEnum.Center, MvxPanelHintType.ResetRoot, true)]
    public class MainView : BaseViewController<MainViewModel>
    {
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            ViewModel.ShowMenu();
        }
    }   
}