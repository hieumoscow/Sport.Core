using System;
using Android.Support.Design.Widget;
using MvvmCross.Binding;
using MvvmCross.Binding.Droid.Target;
using Sport.Core.Mvvm.Validation;

namespace Sport.Droid.Mvvm
{
    public class TextLayoutTargetBinding : MvxAndroidTargetBinding
    {
        protected TextInputLayout EditText
        {
            get { return (TextInputLayout)Target; }
        }
        public TextLayoutTargetBinding(TextInputLayout target) : base(target)
        {
        }
        public override MvxBindingMode DefaultMode => MvxBindingMode.OneWay;





        public override Type TargetType { get { return typeof(TextValue); } }

        protected override void SetValueImpl(object target, object value)
        {
            var layout = (TextInputLayout)target;
            var textValue = value as TextValue;
            if (textValue != null)
            {
                textValue.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == nameof(textValue.ErrorMessage))
                    {
                        if (layout != null)
                        {
                            Update(layout, textValue);
                        }
                    }
                };
            }
            Update(layout, textValue);
        }

        private static void Update(TextInputLayout layout, TextValue textValue)
        {
            layout.ErrorEnabled = !string.IsNullOrEmpty(textValue.ErrorMessage);
            layout.Error = textValue.ErrorMessage;
        }

        protected override void Dispose(bool isDisposing)
        {

            base.Dispose(isDisposing);
        }
    }
}