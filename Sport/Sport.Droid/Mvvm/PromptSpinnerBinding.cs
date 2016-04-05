using System;
using MvvmCross.Binding;
using MvvmCross.Binding.Droid.Target;
using MvvmCross.Binding.Droid.Views;

namespace Sport.Droid.Mvvm
{
	public class PromptSpinnerBinding : MvxAndroidTargetBinding
	{
		protected MvxSpinner EditText
		{
			get { return (MvxSpinner)Target; }
		}

		public PromptSpinnerBinding(MvxSpinner target) : base(target)
		{
		}
		protected override void SetValueImpl (object target, object value)
		{
			var editext = (MvxSpinner)target;
			if (value != null)
				editext.Prompt = value.ToString ();
		}

		public override Type TargetType
		{
			get { return typeof(string); }
		}

		public override MvxBindingMode DefaultMode
		{
			get { return MvxBindingMode.OneWay; }
		}


	}
}