using MvvmCross.Core.ViewModels;
using MvvmCross.Platform.Platform;
using MvvmCross.WindowsPhone.Platform;
using Microsoft.Phone.Controls;
using Sport.Core;

namespace Sport.Phone
{
    public class Setup : MvxPhoneSetup
    {
        public Setup(PhoneApplicationFrame rootFrame) : base(rootFrame)
        {
        }

        protected override IMvxApplication CreateApp()
        {
            return new Core.App(Platform.WindowsPhone);
        }

        protected override IMvxTrace CreateDebugTrace()
        {
            return new DebugTrace();
        }
    }
}
