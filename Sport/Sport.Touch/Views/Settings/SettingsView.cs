using Foundation;
using MvvmCross.iOS.Support.SidePanels;
using Sport.Core.ViewModels.Settings;
using Sport.Touch.Views.Base;

namespace Sport.Touch.Views.Settings
{
    [Register("SettingsView")]
	[MvxPanelPresentation(MvxPanelEnum.Center, MvxPanelHintType.ResetRoot, true)]
    public class SettingsView : BaseViewController<SettingsViewModel>
    {
        public override void ViewWillAppear(bool animated)
        {
            Title = "Settings View";
            base.ViewWillAppear(animated);
        }
    }
}
