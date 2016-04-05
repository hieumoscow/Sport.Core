using MvvmCross.Core.ViewModels;
using Sport.Core.Mvvm;
using Sport.Core.ViewModels.Base;

namespace Sport.Core.ViewModels.Home
{
	public class ThirdViewModel : BaseViewModel
    {
     
        private MvxCommand saveAndCloseCommand;

        public MvxCommand SaveAndCloseCommand
        {
            get
            {
                saveAndCloseCommand = saveAndCloseCommand ?? new MvxCommand(DoSaveAndClose);
                return saveAndCloseCommand;
            }
        }

        private void DoSaveAndClose()
        {
            Close(this);
        }


    }
}