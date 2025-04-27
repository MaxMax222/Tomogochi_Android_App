
using System;
using System.Threading.Tasks;
using Android.App;
using Android.OS;
using Android.Widget;
using FinalProj_Tomogochi.Classes;

namespace FinalProj_Tomogochi.Activities
{
    [Activity (Label = "RegisterActivity", Theme = "@style/AppTheme")]			
	public class RegisterActivity : Activity
	{
        private EditText _fullNameEditText, _lastNameEditText, _usernameEditText, _emailEditText, _passwordEditText, _confirmPasswordEditText;
        private Button _registerButton, _cancelButton;
        protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
            SetContentView(Resource.Layout.register_screen);
			Init();
		}

        private void Init()
        {
            _fullNameEditText = FindViewById<EditText>(Resource.Id.fullName_edittxt);
            _usernameEditText = FindViewById<EditText>(Resource.Id.username_edittxt);
            _emailEditText = FindViewById<EditText>(Resource.Id.email_edittxt);
            _passwordEditText = FindViewById<EditText>(Resource.Id.password_edittxt);
            _confirmPasswordEditText = FindViewById<EditText>(Resource.Id.confirm_password_edittxt);

            _registerButton = FindViewById<Button>(Resource.Id.register_btn);
            _registerButton.Click += async (sender, e) => await RegisterUser();

            _cancelButton = FindViewById<Button>(Resource.Id.cencel_btn);
            _cancelButton.Click += (sender, e) =>
            {
                Finish();
            };
        }

        private async Task RegisterUser()
        {
            string fullName = _fullNameEditText.Text.Trim();
            string username = _usernameEditText.Text.Trim();
            string email = _emailEditText.Text.Trim();
            string password = _passwordEditText.Text.Trim();
            string confirmPassword = _confirmPasswordEditText.Text.Trim();

            if (string.IsNullOrEmpty(fullName) ||
                string.IsNullOrEmpty(username) || string.IsNullOrEmpty(email) ||
                string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword))
            {
                Toast.MakeText(this, "Please fill in all fields.", ToastLength.Short).Show();
                return;
            }

            if (!Android.Util.Patterns.EmailAddress.Matcher(email).Matches())
            {
                Toast.MakeText(this, "Please enter a valid email address.", ToastLength.Short).Show();
                return;
            }

            if (password != confirmPassword)
            {
                Toast.MakeText(this, "Passwords do not match.", ToastLength.Short).Show();
                return;
            }

            if (password.Length < 6)
            {
                Toast.MakeText(this, "Password must be at least 6 characters long.", ToastLength.Short).Show();
                return;
            }

            try
            {
                bool success = await User.Register(fullName, username, email, password);
                if (success)
                {
                    Toast.MakeText(this, "Registration successful!", ToastLength.Short).Show();
                    Finish();
                }
                else
                {
                    Toast.MakeText(this, "Registration failed. Try again.", ToastLength.Short).Show();
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
            }
        }
    }
}

