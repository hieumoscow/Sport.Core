using MvvmCross.Core.ViewModels;

namespace Sport.Core.Mvvm.Validation
{
    public class TextValue : MvxNotifyPropertyChanged
    {

        private string _value;
        public string Value
        {
            get { return _value; }
            set
            {
                SetProperty(ref _value, value);
                if (!string.IsNullOrEmpty(ErrorMessage))
                    Validate();
            }
        }

        public void SetValue(string str)
        {
            _value = str;
        }

        private bool _canChange;
        public bool CanChange
        {
            get { return _canChange; }
            set { SetProperty(ref _canChange, value); }
        }


        private string _errorMessage;
        public string ErrorMessage
        {
            get { return _errorMessage; }
            set { SetProperty(ref _errorMessage, value); }
        }

        public TextValue(string value, bool canChange = true, params Validation[] funcs)
        {
            _funcs = funcs;
            _value = value;
            _canChange = canChange;
        }

        private Validation[] _funcs;
        public TextValue(params Validation[] funcs)
        {
            _funcs = funcs;
            _canChange = true;
        }

        public override string ToString()
        {
            return Value;
        }

        public bool Validate()
        {
            if (_funcs != null)
            {
                foreach (var func in _funcs)
                {
                    if (!func.Validate(Value))
                    {
                        ErrorMessage = func.Message;
                        return false;
                    }
                }
                ErrorMessage = null;
            }
            return true;
        }

    }
}
