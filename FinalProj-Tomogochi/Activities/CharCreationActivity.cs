
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
using Bumptech.Glide;
using static Android.App.DownloadManager;

namespace FinalProj_Tomogochi.Activities
{
	[Activity (Label = "CharCreationActivity", Theme = "@style/AppTheme")]			
	public class CharCreationActivity : Activity
	{
		private Spinner hair_color_spinner, body_color_spinner, characters_spinner;
		private ImageView avatar_imageView;
		string url;
		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			// Create your application here
			SetContentView(Resource.Layout.character_creation_screen);

			Init();
		}

        private void Init()
        {
            url = "https://api.dicebear.com/9.x/Miniavs/png?seed=";

            avatar_imageView = FindViewById<ImageView>(Resource.Id.avatar_imgView);

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

            // Load initial avatar
            UpdateAvatar();
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
            string fullUrl = $"https://api.dicebear.com/9.x/miniavs/png?seed={seed}&hairColor={hairColor}&skinColor={skinColor}&glassesProbability=0";

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

