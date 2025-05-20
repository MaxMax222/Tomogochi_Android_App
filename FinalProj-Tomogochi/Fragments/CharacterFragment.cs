
using System;
using System.Collections.Generic;
using System.Timers;
using Android.Content;
using Android.OS;
using Android.Views;
using Microcharts;
using Microcharts.Droid;
using SkiaSharp;
using AndroidX.Core.Content;
using Android.App;
using Android.Widget;
using Bumptech.Glide;
using FinalProj_Tomogochi.Classes;
using System.Linq;
using Firebase.Firestore;
using Android.Gms.Extensions;
using Java.Util;
using Java.Security;
using Java.Lang;
using Android.Icu.Lang;

namespace FinalProj_Tomogochi.Fragments
{
    [Obsolete]
    public class CharacterFragment : AndroidX.Fragment.App.Fragment
    {
        private BGupdateFBlistener listener = new BGupdateFBlistener();
        private List<ChartEntry> entries;
        private ChartView chartView;
        private TextView balance;
        private System.Timers.Timer timer;
        Classes.Character character;
        CollectionReference BGcollectionRef;
        DocumentReference characterRef;
        [Obsolete]
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);

            character = User.GetUserInstance().ActiveCharacter;

            View view = inflater.Inflate(Resource.Layout.character_view_screen, container, false);
            chartView = view.FindViewById<ChartView>(Resource.Id.chartView);
            var avatar = view.FindViewById<ImageView>(Resource.Id.avatar_img);
            Glide.With(this)
                    .Load(User.GetUserInstance().ActiveCharacter.avatar_path)
                    .Error(Resource.Drawable.anonymus)
                    .Into(avatar);
            var char_name_view = view.FindViewById<TextView>(Resource.Id.char_name_txt);
            char_name_view.Text = User.GetUserInstance().ActiveCharacter.Name;
            balance = view.FindViewById<TextView>(Resource.Id.balance_txt);
            balance.Text = $"Current Balance: {character.Balance}$";

            character.UpdateChart(chartView);

            characterRef = FirebaseHelper.GetFirestore()
                    .Collection("users").Document(FirebaseHelper.GetFirebaseAuthentication().CurrentUser.Uid)
                    .Collection("characters").Document(User.GetUserInstance().ActiveCharacter.Name);

            BGcollectionRef = characterRef.Collection("lastBGs");
            timer = new System.Timers.Timer(1000 * 20);
            timer.Elapsed += Timer_Elapsed;
            timer.AutoReset = true;  // true = repeat
            timer.Enabled = true;

            listener.OnBGEntryRetrieved += Listener_OnBGEntryRetrieved;
            return view;
        }



        private async void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                var time = DateTime.Now;

                character.UpdateBG();

                var updates = new Dictionary<string, Java.Lang.Object> {
                { "balance", new Java.Lang.String(character.Balance.ToString()) },
                { "bgChange", new Java.Lang.String(character.BG_Change.ToString()) }
                };
                await characterRef.Update(updates);
                // Only add the *latest* entry, don't delete and re-add all
                var bgMap = new HashMap();
                bgMap.Put("label", time.ToString("HH:mm"));
                bgMap.Put("value", character.CurrentBG);
                await BGcollectionRef.Add(bgMap);

                // Optional: trim older entries if count exceeds 10
                QuerySnapshot snapshot = (QuerySnapshot)await BGcollectionRef.OrderBy("label").Get();
                if (snapshot.Documents.Count > 10)
                {
                    int excess = snapshot.Documents.Count - 10;
                    for (int i = 0; i < excess; i++)
                    {
                        await snapshot.Documents[i].Reference.Delete();
                    }
                }
            }
            catch (System.Exception ex)
            {
                Toast.MakeText(Application.Context, $"Timer update failed: {ex.Message}", ToastLength.Long).Show();
            }
          
        }


        private void Listener_OnBGEntryRetrieved(object sender, BGupdateFBlistener.BGEntriesEventArgs e)
        {
            entries = e.BGEntries.OrderBy(p => p.Label).ToList();

            // Update the character's BG list with the new entries
            var character = User.GetUserInstance().ActiveCharacter;
            character.LastBGs.Clear(); // Clear existing entries
            foreach (var entry in entries)
            {
                character.UpdateBG_List(entry); // Add entries with existing logic
            }
            character.UpdateChart(chartView);
            balance.Text = $"Current Balance: {character.Balance}$";

        }
    }
}

