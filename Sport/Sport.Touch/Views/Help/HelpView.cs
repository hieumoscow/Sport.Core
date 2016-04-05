using Foundation;
using MvvmCross.iOS.Support.SidePanels;
using Sport.Core.ViewModels.Help;
using Sport.Touch.Views.Base;

namespace Sport.Touch.Views.Help
{
    [Register("HelpView")]
    [MvxPanelPresentation(MvxPanelEnum.Center, MvxPanelHintType.ResetRoot, true)]
    public class HelpView : BaseViewController<HelpAndFeedbackViewModel>
    {
        public override void ViewWillAppear(bool animated)
        {
            Title = "Help View";
            base.ViewWillAppear(animated);
        }
    }
}
