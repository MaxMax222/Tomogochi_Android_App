
using System;
using System.Collections.Generic;

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

namespace FinalProj_Tomogochi.Fragments
{
    [Obsolete]
    public class CharacterFragment : AndroidX.Fragment.App.Fragment
    {
        private BGupdateFBlistener listener = new BGupdateFBlistener();
        private List<ChartEntry> entries;
        private ChartView chartView;
        [Obsolete]
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);

            View view = inflater.Inflate(Resource.Layout.character_view_screen, container, false);
            chartView = view.FindViewById<ChartView>(Resource.Id.chartView);
            var avatar = view.FindViewById<ImageView>(Resource.Id.avatar_img);
            Glide.With(this)
                    .Load(User.GetUserInstance().ActiveCharacter.avatar_path)
                    .Error(Resource.Drawable.anonymus)
                    .Into(avatar);
            var char_name_view = view.FindViewById<TextView>(Resource.Id.char_name_txt);
            char_name_view.Text = User.GetUserInstance().ActiveCharacter.Name;

            listener.OnBGEntryRetrieved += Listener_OnBGEntryRetrieved;

            User.GetUserInstance().ActiveCharacter.UpdateChart(chartView);

            return view;
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
            character.UpdateChart(chartView); // Re-draw the chart

        }
    }
}

