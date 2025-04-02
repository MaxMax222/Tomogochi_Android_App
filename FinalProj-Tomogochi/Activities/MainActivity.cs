using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using FinalProj_Tomogochi.Fragments;
using Google.Android.Material.BottomNavigation;

namespace FinalProj_Tomogochi.Activities
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme")]
    public class MainActivity : AppCompatActivity, BottomNavigationView.IOnNavigationItemSelectedListener
    {
        [System.Obsolete]
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            BottomNavigationView navigation = FindViewById<BottomNavigationView>(Resource.Id.navigation);
            navigation.SetOnNavigationItemSelectedListener(this);

            navigation.SelectedItemId = Resource.Id.navigation_character;

            LoadFragment(new CharacterFragment());
        }

        [Obsolete]
        private void LoadFragment(AndroidX.Fragment.App.Fragment fragment)
        {
            SupportFragmentManager.BeginTransaction()
               .Replace(Resource.Id.fragment_container, fragment)
               .Commit();
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        [Obsolete]
        public bool OnNavigationItemSelected(IMenuItem item)
        {

            AndroidX.Fragment.App.Fragment selectedFragment = null;

            switch (item.ItemId)
            {
                case Resource.Id.navigation_character:
                    selectedFragment = new CharacterFragment();
                    break;
                case Resource.Id.navigation_dashboard:
                    Toast.MakeText(Application.Context, Resource.String.title_dashboard, ToastLength.Long).Show();
                    return true;
                case Resource.Id.navigation_notifications:
                    Toast.MakeText(Application.Context, Resource.String.title_notifications, ToastLength.Long).Show();
                    return true;
            }

            if (selectedFragment != null)
            {
                LoadFragment(selectedFragment);
                return true;
            }
            return false;
        }
    }
}


