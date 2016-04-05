using System;
using Android.Support.Design.Widget;
using Android.Text;
using Android.Widget;
using MvvmCross.Binding;
using MvvmCross.Binding.Droid.Target;
using Sport.Core.Mvvm.Validation;

namespace Sport.Droid.Mvvm
{
    public class EditTextTargetBinding : MvxAndroidTargetBinding
    {
        protected EditText EditText
        {
            get { return (EditText)Target; }
        }
        public EditTextTargetBinding(EditText target) : base(target)
        {
        }
        public override MvxBindingMode DefaultMode => MvxBindingMode.TwoWay;

        public override void SubscribeToEvents()
        {
            EditText.TextChanged += TargetOnTextChanged;
        }

        private void TargetOnTextChanged(object sender, TextChangedEventArgs e)
        {
            var target = Target as EditText;

            if (target == null)
                return;

            var value = target.Text;
            TextValue.PropertyChanged -= OnPropertyChanged;
            if (TextValue != null && !value.Equals(TextValue.Value) &&
                (!string.IsNullOrEmpty(TextValue?.Value) || !string.IsNullOrEmpty(value)))
                TextValue.Value = (value);
            TextValue.PropertyChanged += OnPropertyChanged;
        }

        public override Type TargetType { get { return typeof(TextValue); } }

        private TextValue TextValue { get; set; }
        protected override void SetValueImpl(object target, object value)
        {
            var editText
                = (EditText)target;
            TextValue = value as TextValue;
            if (TextValue != null)
            {
                TextValue.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == nameof(TextValue.ErrorMessage))
                    {
                        var layout = editText.Parent as TextInputLayout;
                        if (layout != null)
                        {
                            layout.ErrorEnabled = !string.IsNullOrEmpty(TextValue.ErrorMessage);
                            layout.Error = TextValue.ErrorMessage;
                        }
                    }
                    
                };
                TextValue.PropertyChanged += OnPropertyChanged;
                editText.Text = (TextValue.Value);
                editText.Enabled = TextValue.CanChange;
            }
        }

        private void OnPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(TextValue.Value))
            {
               
                    EditText.Text = TextValue.Value;
            }
        }

        protected override void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                EditText.TextChanged -= TargetOnTextChanged;
            }
            base.Dispose(isDisposing);
        }
    }
}