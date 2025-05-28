
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
using FinalProj_Tomogochi.Services;

namespace FinalProj_Tomogochi.Fragments
{
    [Obsolete]
    public class CharacterFragment : AndroidX.Fragment.App.Fragment
    {
        private BGupdateFBlistener BGlistener = User.GetUserInstance().BGlistener;
        private BalanceUpdateFBlistener Blistener = User.GetUserInstance().BalanceListener;
        private List<ChartEntry> entries;
        private ChartView chartView;
        private TextView balance;
        Classes.Character character;
        [Obsolete]
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);

            character = User.GetUserInstance().Character;

            View view = inflater.Inflate(Resource.Layout.character_view_screen, container, false);
            chartView = view.FindViewById<ChartView>(Resource.Id.chartView);
            var avatar = view.FindViewById<ImageView>(Resource.Id.avatar_img);
            Glide.With(this)
                    .Load(User.GetUserInstance().Character.avatar_path)
                    .Error(Resource.Drawable.anonymus)
                    .Into(avatar);
            var char_name_view = view.FindViewById<TextView>(Resource.Id.char_name_txt);
            char_name_view.Text = User.GetUserInstance().Character.Name;
            balance = view.FindViewById<TextView>(Resource.Id.balance_txt);
            balance.Text = $"Current Balance: {character.Balance}$";

            character.UpdateChart(chartView);

            if (!BGupdate.IsRunning)
            {
                var intent = new Intent(Context, typeof(BGupdate));
                if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
                    Context.StartForegroundService(intent);
                else
                    Context.StartService(intent);
            }

            BGlistener.OnBGEntryRetrieved += Listener_OnBGEntryRetrieved;
            Blistener.onBalanceRetrieved += Blistener_onBalanceRetrieved;
            return view;
        }

        private void Blistener_onBalanceRetrieved(object sender, BalanceUpdateFBlistener.BalanceArgs e)
        {
            balance.Text = $"Current Balance: {character.Balance}$";
        }

        private void Listener_OnBGEntryRetrieved(object sender, BGupdateFBlistener.BGEntriesEventArgs e)
        {
            entries = e.BGEntries.OrderBy(p => p.Label).ToList();

            // Update the character's BG list with the new entries
            var character = User.GetUserInstance().Character;
            character.LastBGs.Clear(); // Clear existing entries
            foreach (var entry in entries)
            {
                character.UpdateBG_List(entry); // Add entries with existing logic
            }
            character.UpdateChart(chartView);
        }
    }
}

