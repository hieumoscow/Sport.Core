using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using MvvmCross.Binding.Droid.BindingContext;
using MvvmCross.Droid.Support.V7.Fragging.Fragments;
using Tutor.Core.ViewModels.Dialog;

namespace Tutor.Droid.Fragments.Dialogs
{
    [Register("tutor.droid.fragments.PhoneDialog")]
    public class PhoneDialog : MvxDialogFragment<PhoneDialogViewModel>
    {
        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            EnsureBindingContextSet(savedInstanceState);
            var view = this.BindingInflate(Resource.Layout.dialog_input_phone, null);
            var dialog = new AlertDialog.Builder(Activity);
            dialog.SetTitle("Gửi lại mã xác nhận");
            dialog.SetView(view);
            dialog.SetCancelable(false);
            dialog.SetPositiveButton("GỬI", (IDialogInterfaceOnClickListener)null);
            dialog.SetNegativeButton("HỦY", (sender, args) =>
            {
                ViewModel.SetResult(null);
            });

            var result = dialog.Create();
            result.ShowEvent += Result_ShowEvent;
            return result;
        }

        private void Result_ShowEvent(object sender, EventArgs e)
        {
            var result = this.Dialog as AlertDialog;
            if (result != null)
                result.GetButton((int)DialogButtonType.Positive).Click += (s, a) =>
                {
                    if (ViewModel.PhoneNumber.Validate())
                    {
                        ViewModel.SetResult(ViewModel.PhoneNumber.Value);
                        Dismiss();
                    }
                };
        }
    }

    [Register("tutor.droid.fragments.RoleSelectionDialog")]
    public class RoleSelectionDialog : MvxDialogFragment<RoleSelectionDialogViewModel>
    {

        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            EnsureBindingContextSet(savedInstanceState);
            var view = this.BindingInflate(Resource.Layout.dialog_roleselection, null);
            var dialog = new AlertDialog.Builder(Activity);
            dialog.SetView(view);
            dialog.SetCancelable(false);
            var result = dialog.Create();

            return result;
        }
    }
}