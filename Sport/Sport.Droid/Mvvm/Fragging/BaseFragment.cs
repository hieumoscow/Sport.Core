using Android.OS;
using Android.Views;
using MvvmCross.Binding.Droid.BindingContext;
using MvvmCross.Core.Views;

namespace Sport.Droid.Mvvm.Fragging
{
    public abstract class BaseFragment : MvvmCross.Droid.Support.V7.Fragging.Fragments.MvxFragment
    {
        protected Android.Support.V7.Widget.Toolbar Toolbar { get; private set; }
        private MvvmCross.Droid.Support.V7.AppCompat.MvxActionBarDrawerToggle _drawerToggle;

        protected BaseFragment()
        {
            RetainInstance = true;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var ignore = base.OnCreateView(inflater, container, savedInstanceState);

            var view = this.BindingInflate(FragmentId, null);

            Toolbar = view.FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);

            //if (Toolbar != null && Activity is MainActivity)
            //{
            //    ((MainActivity)Activity).SetSupportActionBar(Toolbar);
            //    ((MainActivity)Activity).SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            //    ((MainActivity)Activity).SupportActionBar.SetDisplayShowCustomEnabled(true);
            //    _drawerToggle = new MvvmCross.Droid.Support.V7.AppCompat.MvxActionBarDrawerToggle(
            //        Activity,                               // host Activity
            //        ((MainActivity)Activity).DrawerLayout,  // DrawerLayout object
            //        Toolbar,                               // nav drawer icon to replace 'Up' caret
            //        Resource.String.drawer_open,            // "open drawer" description
            //        Resource.String.drawer_close            // "close drawer" description
            //    );

            //    ((MainActivity)Activity).DrawerLayout.SetDrawerListener(_drawerToggle);

            //    var myMvxViewModel = ViewModel as MyMvxViewModel;

            //    if (myMvxViewModel != null)
            //    {
            //        myMvxViewModel.PropertyChanged += (sender, args) =>
            //        {
            //            if (args.PropertyName.Equals(nameof(myMvxViewModel.Title)))
            //            {
            //                Toolbar.Title = myMvxViewModel.Title;
            //            }
            //        };
            //        Toolbar.Title = myMvxViewModel.Title;
            //    }

            //}
            //else if (Toolbar != null)
            //{
            //    ((RegisterActivity)Activity).SetSupportActionBar(Toolbar);
            //    ((RegisterActivity)Activity).SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            //    if (Toolbar != null) Toolbar.NavigationClick += (s, e) =>
            //    {
            //        Activity.Finish();
            //    };
            //}

            
            //var mvxViewModel = ViewModel as MyMvxViewModel;
            //if (mvxViewModel != null)
            //    mvxViewModel.Parent = ((IMvxView)Activity).ViewModel;
            return view;
        }

        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            
            base.OnCreateOptionsMenu(menu, inflater);
        }

        protected abstract int FragmentId { get; }

        public override void OnConfigurationChanged(Android.Content.Res.Configuration newConfig)
        {
            base.OnConfigurationChanged(newConfig);
            if (Toolbar != null)
            {
                _drawerToggle.OnConfigurationChanged(newConfig);
            }
        }

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnActivityCreated(savedInstanceState);
            //if (Toolbar != null && Activity is MainActivity)
            //{
            //    _drawerToggle.SyncState();
            //}
        }
    }

    public abstract class BaseFragment<TViewModel> : BaseFragment where TViewModel : class, MvvmCross.Core.ViewModels.IMvxViewModel
    {
        public new TViewModel ViewModel
        {
            get { return (TViewModel)base.ViewModel; }
            set { base.ViewModel = value; }
        }
    }

    public abstract class BaseDialogFragment : MvvmCross.Droid.Support.V7.Fragging.Fragments.MvxDialogFragment
    {
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return base.OnCreateView(inflater, container, savedInstanceState);
        }
    }

}