
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace FinalProj_Tomogochi.Activities
{
	[Activity (Label = "RegisterActivity", Theme = "@style/AppTheme")]			
	public class RegisterActivity : Activity
	{
		Button reg_btn, cencel_btn;
		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
            SetContentView(Resource.Layout.register_screen);
			Init();
		}

        private void Init()
        {
            cencel_btn = FindViewById<Button>(Resource.Id.cencel_btn);
            cencel_btn.Click += (sender, e) =>
            {
                var intent = new Intent(this, typeof(MainActivity));
                intent.SetFlags(ActivityFlags.ClearTask | ActivityFlags.NewTask);
                StartActivity(intent);
            };

            reg_btn = FindViewById<Button>(Resource.Id.register_btn);
            reg_btn.Click += (sender, e) =>
            {
                Finish();
            };
        }
    }
}

