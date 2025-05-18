using System;
using System.Collections.Generic;
using Android.Gms.Extensions;
using Android.Widget;
using System.Threading.Tasks;
using Firebase.Auth;
using Firebase.Firestore;
using Android.App;
using Java.Util;
using Firebase.Storage;
using static Xamarin.Essentials.Permissions;
using ShopMiniProj.Classes;
using Microcharts;
using SkiaSharp;
using Android.Content;
using AndroidX.Core.Content;

namespace FinalProj_Tomogochi.Classes
{
	public class User
	{
		private static User _instance;
		public List<Character> characters;
        public string FullName { get; private set; }
        public string Email { get; private set; }
        public string Username { get; private set; }
        public Character ActiveCharacter { get; set; }


        public static FirebaseAuth FirebaseAuth { get; private set; }
        public static FirebaseFirestore database { get; private set; }
        public const string COLLECTION_NAME = "users";

        private User()
		{
            database = FirebaseHelper.GetFirestore();
            FirebaseAuth = FirebaseHelper.GetFirebaseAuthentication();
            characters = new List<Character>();
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

        public async Task<string> SaveAvatarToFirebaseStorageAsync(byte[] file_bytes, string in_storage_path)
        {
            try
            {
                var storage = FirebaseStorage.GetInstance("gs://tomogochi-finalproj.firebasestorage.app/"); //FirebaseStorage storage = FirebaseStorage.getInstance("gs://my-custom-bucket");
                var storageRef = storage.GetReference(in_storage_path);

                await storageRef.PutBytes(file_bytes);
                // Once upload finishes, get the download URL
                var downloadUrl = await storageRef.DownloadUrl;
                return downloadUrl.ToString();
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context,$"Upload failed: {ex}",ToastLength.Long).Show();
                return null;
            }
        }

        public async Task SaveCharacterToFirestoreAsync(Character character)
        {
            try
            {
                var userReference = database.Collection(COLLECTION_NAME).Document(FirebaseAuth.CurrentUser.Uid);
                var characterDocRef = userReference.Collection("characters").Document(character.Name);

                HashMap characterData = new HashMap();
                characterData.Put("name", character.Name);
                characterData.Put("avatar_path", character.avatar_path);
                characterData.Put("balance",0.ToString());
                characterData.Put("bgChange",0.ToString());
                await characterDocRef.Set(characterData);

                Toast.MakeText(Application.Context, "Character saved to Firestore!", ToastLength.Short).Show();
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, $"Failed to save character: {ex.Message}", ToastLength.Long).Show();
            }
        }

        public async Task LoadCharactersFromFirestoreAsync()
        {
            try
            {
                characters.Clear(); // clear existing list to avoid duplicates
                var userReference = database.Collection(COLLECTION_NAME).Document(FirebaseAuth.CurrentUser.Uid);
                var charactersCollectionSnapshot = (QuerySnapshot)await userReference.Collection("characters").Get();
                foreach (var document in charactersCollectionSnapshot.Documents)    
                {
                    string name = document.GetString("name");

                    var bgsRef = userReference.Collection("characters").Document(name).Collection("lastBGs");
                    var bgSnapshot = (QuerySnapshot)await bgsRef.Get();
                    var BGs = new List<ChartEntry>();
                    foreach (DocumentSnapshot BG in bgSnapshot.Documents)
                    {
                        string label = BG.Get("label").ToString();
                        string BGvalue = BG.Get("value").ToString();
                        BGs.Add(new ChartEntry(int.Parse(BGvalue))
                        {
                            Label = label,
                            ValueLabel = BGvalue,
                            Color = SKColor.Parse(GetColorString(int.Parse(BGvalue), Application.Context))
                        });
                    }

                    string avatarPath = document.GetString("avatar_path");
                    double balance = double.Parse(document.GetString("balance"));
                    int bgChange = int.Parse(document.GetString("bgChange"));

                    Character character = new Character(name, avatarPath,balance,bgChange,BGs);
                    characters.Add(character);
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, $"Failed to load characters: {ex.Message}", ToastLength.Long).Show();
            }
        }

        public static string GetColorString(int sugar, Context context)
        {
            int colorResId;

            if (sugar > 70 && sugar < 180)
                colorResId = Resource.Color.hunter_green;
            else if (sugar >= 180)
                colorResId = Resource.Color.tea_green;
            else
                colorResId = Resource.Color.bright_pink_crayola;

            int colorInt = ContextCompat.GetColor(context, colorResId);
            string hex = $"#{colorInt & 0xFFFFFF:X6}"; // Format as hex string like "#3CB371"
            return hex;
        }

    }
}

