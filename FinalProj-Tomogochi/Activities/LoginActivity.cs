
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
using Firebase.Auth;
using FinalProj_Tomogochi.Classes;

namespace FinalProj_Tomogochi.Activities
{
	[Activity (Label = "LoginActivity", Theme = "@style/AppTheme", MainLauncher = true)]			
	public class LoginActivity : Activity
	{
		Button log_btn, reg_btn;
        EditText email_edtxt, password_edtxt;
        CheckBox remember;
        private User user;

        private ISharedPreferences _preferences;
        private ISharedPreferencesEditor _editor;
        protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			SetContentView(Resource.Layout.login_screen);
			Init();

		}

        private void Init()
        {
            email_edtxt = FindViewById<EditText>(Resource.Id.email_edittxt);
            password_edtxt = FindViewById<EditText>(Resource.Id.password_edittxt);
            remember = FindViewById<CheckBox>(Resource.Id.remember_checkbox);
            user = User.GetUserInstance();
            _preferences = GetSharedPreferences("UserPreferences", FileCreationMode.Private);
            _editor = _preferences.Edit();
            LoadUserPreferences();

            

            log_btn = FindViewById<Button>(Resource.Id.login_btn);
            log_btn.Click += LoginButton_Click;

            reg_btn = FindViewById<Button>(Resource.Id.register_btn);
            reg_btn.Click += (sender, e) =>
            {
                var intent = new Intent(this, typeof(RegisterActivity));
                StartActivity(intent);
            };

        }
        private async void LoginButton_Click(object sender, EventArgs e)
        {
            string enteredEmail = email_edtxt.Text.Trim();
            string enteredPassword = password_edtxt.Text.Trim();    

            if (string.IsNullOrEmpty(enteredEmail) || string.IsNullOrEmpty(enteredPassword))
            {
                Toast.MakeText(this, "Please fill in both fields.", ToastLength.Long).Show();
                return;
            }
            if (enteredPassword.Length < 6)
            {
                Toast.MakeText(this, "Password must contain at least 6 characters", ToastLength.Long).Show();
                return;
            }

            try
            {
                if (await User.Login(enteredEmail, enteredPassword))
                {
                    //_ = user.FetchUserData();
                    SaveUserPreferences(enteredEmail, enteredPassword);
                    NavigateToMain();
                }
                else
                {
                    Toast.MakeText(this, "Invalid username or password. Please try again.", ToastLength.Long).Show();
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
            }
        }

        private void SaveUserPreferences(string email, string password)
        {
            if (remember.Checked)
            {
                _editor.PutString("Email", email);
                _editor.PutString("Password", password);
                _editor.PutBoolean("Remember", true);
            }
            else
            {
                _editor.Remove("Email");
                _editor.Remove("Password");
                _editor.PutBoolean("Remember", false);
            }
            _editor.Apply();
        }

        private void LoadUserPreferences()
        {
            if (_preferences.GetBoolean("Remember", false))
            {
                email_edtxt.Text = _preferences.GetString("Email", "");
                password_edtxt.Text = _preferences.GetString("Password", "");
                remember.Checked = true;
            }
        }

        private void NavigateToMain()
        {
            var intent = new Intent(this, typeof(MainActivity));
            intent.SetFlags(ActivityFlags.ClearTask | ActivityFlags.NewTask);
            StartActivity(intent);
        }
    }
}

