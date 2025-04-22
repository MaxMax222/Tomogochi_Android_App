using System;
using System.Collections.Generic;
using Android.Gms.Extensions;
using Android.Widget;
using System.Threading.Tasks;
using Firebase.Auth;
using Firebase.Firestore;
using Android.App;
using Java.Util;

namespace FinalProj_Tomogochi.Classes
{
	public class User
	{
		private static User _instance;
		private List<Character> characters;
        public string FullName { get; private set; }
        public string Email { get; private set; }
        public string Username { get; private set; }
        public Character ActiveCharacter { get; private set; }


        public static FirebaseAuth FirebaseAuth { get; private set; }
        public static FirebaseFirestore database { get; private set; }
        public const string COLLECTION_NAME = "users";

        private User()
		{
            database = FireBaseHelper.GetFirestore();
            FirebaseAuth = FireBaseHelper.GetFirebaseAuthentication();
        }

        public static User GetUserInstance()
        {
            _instance ??= new User();
            return _instance;
        }

        public static async Task<bool> Login(string Email, string Password)
        {
            try
            {
                await FirebaseAuth.SignInWithEmailAndPassword(Email, Password);

            }
            catch (Exception Ex)
            {
                Toast.MakeText(Application.Context, Ex.Message, ToastLength.Long).Show();
                return false;
            }
            return true;
        }

        public bool SignOut()
        {
            try
            {
                FirebaseAuth.SignOut();
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Long).Show();
                return false;
            }
            return true;
        }

        public static async Task<bool> Register(string fullName, string username, string email, string password)
        {
            try
            {
                await FirebaseAuth.CreateUserWithEmailAndPassword(email, password);
            }
            catch (Exception Ex)
            {
                Toast.MakeText(Application.Context, Ex.Message, ToastLength.Long);
                return false;
            }
            try
            {
                HashMap newUser = new HashMap();
                newUser.Put("firstName", fullName);
                newUser.Put("username", username);
                newUser.Put("email", email);

                var userReference = database.Collection(COLLECTION_NAME).Document(FirebaseAuth.CurrentUser.Uid);
                await userReference.Set(newUser);
            }
            catch (Exception Ex)
            {
                Toast.MakeText(Application.Context, Ex.Message, ToastLength.Long).Show();
                return false;
            }
            return true;
        }
    }
}

