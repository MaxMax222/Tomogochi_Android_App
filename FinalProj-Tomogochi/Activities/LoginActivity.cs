﻿
using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Gms.Extensions;
using Android.OS;
using Android.Widget;
using FinalProj_Tomogochi.Classes;
using Firebase.Auth;
using Firebase.Firestore;

namespace FinalProj_Tomogochi.Activities
{
    [Activity (Label = "LoginActivity", Theme = "@style/AppTheme", MainLauncher = true)]			
	public class LoginActivity : Activity
	{
		Button log_btn, reg_btn;
        EditText email_edtxt, password_edtxt;
        CheckBox remember;

        private ISharedPreferences _preferences;
        private ISharedPreferencesEditor _editor;
        protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			SetContentView(Resource.Layout.login_screen);
            User.GetUserInstance();
			Init();

		}

        private void Init()
        {
            email_edtxt = FindViewById<EditText>(Resource.Id.email_edittxt);
            password_edtxt = FindViewById<EditText>(Resource.Id.password_edittxt);
            remember = FindViewById<CheckBox>(Resource.Id.remember_checkbox);
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
                    SaveUserPreferences(enteredEmail, enteredPassword);
                    await NavigateToMainAsync();
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

        private async Task NavigateToMainAsync()
        {
            Intent intent;
            var userRef = User.database.Collection("users").Document(User.FirebaseAuth.CurrentUser.Uid);
            var userDoc = (DocumentSnapshot)await userRef.Get();

            if (userDoc.Contains("characterRef"))
            {
                await User.GetUserInstance().LoadCharacterFromFirestoreAsync();
                intent = new Intent(this, typeof(MainActivity));
                
            }
            else
            {
                intent = new Intent(this, typeof(CharCreationActivity));
            }
            intent.SetFlags(ActivityFlags.ClearTask | ActivityFlags.NewTask);
            StartActivity(intent);

        }
    }
}

