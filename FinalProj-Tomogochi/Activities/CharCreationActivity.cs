
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using Bumptech.Glide;
using System.IO;
using System.Net.Http;
using FinalProj_Tomogochi.Classes;

namespace FinalProj_Tomogochi.Activities
{
    [Activity (Label = "CharCreationActivity", Theme = "@style/AppTheme")]			
	public class CharCreationActivity : Activity
	{
		private Spinner hair_color_spinner, body_color_spinner, characters_spinner;
		private ImageView avatar_imageView;
        private Button create_btn;
        private EditText character_name_edttxt;
        private string fullUrl;

        protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			// Create your application here
			SetContentView(Resource.Layout.character_creation_screen);

			Init();
		}

        private void Init()
        {
            
            avatar_imageView = FindViewById<ImageView>(Resource.Id.avatar_imgView);
            character_name_edttxt = FindViewById<EditText>(Resource.Id.character_name_edttxt);
            // Hair colors
            hair_color_spinner = FindViewById<Spinner>(Resource.Id.spinner_hairColor);
            List<string> hair_colors = new List<string> { "Russion Violet", "Cafe Noir", "Orange" };
            hair_color_spinner.Adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, hair_colors);

            // Body colors
            body_color_spinner = FindViewById<Spinner>(Resource.Id.spinner_skinColor);
            List<string> body_colors = new List<string> { "Rose Taupe", "Pale Dogwood", "Sunset" };
            body_color_spinner.Adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, body_colors);

            // Characters
            characters_spinner = FindViewById<Spinner>(Resource.Id.spinner_Characters);
            List<string> characters = new List<string> {
        "Leah", "Robert", "Christopher", "George", "Jack", "Kimberly",
        "Riley", "Caleb", "Adrian", "Liam", "Aidan", "Avery", "Vivian", "Brian",
        "Christian", "Maria", "Luis", "Jessica", "Leo", "Emery"
            };
            characters_spinner.Adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, characters);

            // Handle selection changes
            hair_color_spinner.ItemSelected += OnSpinnerChanged;
           
            body_color_spinner.ItemSelected += OnSpinnerChanged;
            
            characters_spinner.ItemSelected += OnSpinnerChanged;

            create_btn = FindViewById<Button>(Resource.Id.create_btn);
            create_btn.Click += Create_btn_Click;

            // Load initial avatar
            UpdateAvatar();
        }

        private async void Create_btn_Click(object sender, EventArgs e)
        {
            if (character_name_edttxt.Text.Trim() != "")
            {
                try
                {
                    using (var httpClient = new HttpClient())
                    {
                        // Download the image bytes from DiceBear URL
                        var imageBytes = await httpClient.GetByteArrayAsync(fullUrl);

                        // Set the storage path in Firebase (example path)
                        string in_storagePath = $"avatars/{Guid.NewGuid()}.png";

                        // Upload the avatar to Firebase Storage
                        string downloadUrl = await User.GetUserInstance().SaveAvatarToFirebaseStorageAsync(imageBytes, in_storagePath);

                        if (downloadUrl != null)
                        {
                            // Create the Character with the URL from Firebase
                            Character character = new Character(character_name_edttxt.Text, downloadUrl);
                            User.GetUserInstance().Character = character;
                            await User.GetUserInstance().SaveCharacterToFirestoreAsync(character);
                            
                            Toast.MakeText(Application.Context, "Character created successfully!", ToastLength.Short).Show();

                            var intent = new Intent(this, typeof(MainActivity));
                            intent.SetFlags(ActivityFlags.ClearTask | ActivityFlags.NewTask);
                            StartActivity(intent);
                        }
                        else
                        {
                            Toast.MakeText(Application.Context, "Failed to upload avatar.", ToastLength.Short).Show();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Toast.MakeText(Application.Context, $"Error: {ex.Message}", ToastLength.Long).Show();
                }
            }
            else
            {
                Toast.MakeText(Application.Context, "Please enter a name.", ToastLength.Short).Show();
            }
        }


        private void OnSpinnerChanged(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            UpdateAvatar();
        }

        private void UpdateAvatar()
        {
            string seed = characters_spinner.SelectedItem?.ToString();
            string hairColor = ParseHairColor();
            string skinColor = ParseBodyColor();

            // Optional: glassesProbability set to 0 to hide them
            fullUrl = $"https://api.dicebear.com/9.x/miniavs/png?seed={seed}&hairColor={hairColor}&skinColor={skinColor}&glassesProbability=0";

            Glide.With(this)
                 .Load(fullUrl)
                 .Error(Resource.Drawable.anonymus) // fallback image
                 .Into(avatar_imageView);
        }

        private string ParseBodyColor()
        {
            switch (body_color_spinner.SelectedItem?.ToString())
            {
                case "Rose Taupe":
                    return "836055";

                case "Pale Dogwood":
                    return "f2d0c5";

                case "Sunset":
                    return "ffcb7e";
                default:
                    return "transparent";
            }
        }

        private string ParseHairColor()
        {
            switch (hair_color_spinner.SelectedItem?.ToString())
            {
                case "Russion Violet":
                    return "1b0b47";

                case "Cafe Noir":
                    return "47280b";

                case "Orange":
                    return "ad3a20";
                default:
                    return "transparent";
            }
        }


        
    }
}

