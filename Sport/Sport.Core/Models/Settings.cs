using MvvmCross.Core.ViewModels;

namespace Sport.Core.Models
{
    public class Settings : MvxNotifyPropertyChanged
    {
        private bool _locationConsent;
        private bool _pushConsent;
        private string _pushRegistrationId;

        public bool LocationConsent
        {
            get { return _locationConsent; }
            set { _locationConsent = value; RaisePropertyChanged(() => LocationConsent); }
        }

        public bool PushConsent
        {
            get { return _pushConsent; }
            set
            {
                _pushConsent = value;
                RaisePropertyChanged(() => PushConsent);

            }
        }

        public string PushRegistrationId
        {
            get { return _pushRegistrationId; }
            set { _pushRegistrationId = value; RaisePropertyChanged(() => PushRegistrationId); }
        }
    }
}