using Android.App;
using Android.Content.Res;
using Android.Widget;
using Firebase;
using Firebase.Auth;
using Firebase.Firestore;
using Firebase.Storage;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using Xamarin.Essentials;

namespace FinalProj_Tomogochi.Classes
{
    public static class FirebaseHelper
    {
        static string projectId;
        static string apiKey;
        static FirebaseApp app;

        public static FirebaseFirestore GetFirestore() =>  FirebaseFirestore.GetInstance(RetrieveApp());
        public static FirebaseAuth GetFirebaseAuthentication() => FirebaseAuth.Instance;
        public static FirebaseStorage GetFirebaseStorage() => FirebaseStorage.GetInstance("gs://tomogochi-finalproj.firebasestorage.app/");

        private static FirebaseApp RetrieveApp()
        {
            if (app != null)
                return app;

            LoadConfigFromJson();

            app = FirebaseApp.InitializeApp(Application.Context);
            if (app == null)
            {
                var options = new FirebaseOptions.Builder()
                    .SetProjectId(projectId)
                    .SetApplicationId(projectId)
                    .SetApiKey(apiKey)
                    .SetDatabaseUrl($"https://{projectId}.firebaseio.com")
                    .SetStorageBucket($"{projectId}.appspot.com")
                    .Build();

                app = FirebaseApp.InitializeApp(Application.Context, options);
            }

            return app;
        }

        private static void LoadConfigFromJson()
        {
            if (!string.IsNullOrEmpty(projectId)) // Already loaded
                return;

            try
            {
                AssetManager assets = Application.Context.Assets;
                using (var stream = assets.Open("google-services.json"))
                using (var reader = new StreamReader(stream))
                {
                    var json = reader.ReadToEnd();
                    var jObject = JObject.Parse(json);

                    // Correctly extract values from the JSON
                    projectId = jObject["project_info"]?["project_id"]?.ToString();
                    apiKey = jObject["client"]?[0]?["api_key"]?[0]?["current_key"]?.ToString();

                    // Ensure all necessary fields are populated
                    if (string.IsNullOrEmpty(projectId) || string.IsNullOrEmpty(apiKey))
                    {
                        throw new InvalidOperationException("Missing necessary Firebase configuration values.");
                    }
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, $"[FirebaseHelper] Failed to load firebase_config.json: {ex.Message}", ToastLength.Long).Show();
            }
        }

    }
}
