﻿
using System;
using System.Collections.Generic;

using Android.Content;
using Microcharts;
using Microcharts.Droid;
using SkiaSharp;
using AndroidX.Core.Content;
using Android.App;
using Firebase.Firestore;
using System.Threading.Tasks;
using Android.Gms.Extensions;

namespace FinalProj_Tomogochi.Classes
{
    public class Character
	{
		public string Name { get; }
		public string avatar_path{ get; }
		public int CurrentBG { get; private set; }
		public List<ChartEntry> LastBGs; 
        public int BG_Change { get; private set; }
        private Random rnd = new Random();
        public DateTime LastActive { get; set; }
        public double Balance { get; set; }
        public Dictionary<Food, int> Inventoiry;

		public Character(string name, string path)
		{
            Name = name;
            avatar_path = path;
            CurrentBG = 120;
            LastBGs = new List<ChartEntry> { new ChartEntry(CurrentBG) {
                Label = DateTime.Now.ToString("HH:mm"),
                ValueLabel = CurrentBG.ToString(),
                Color = SKColor.Parse(User.GetColorString(CurrentBG, Application.Context))
            } };
            BG_Change = 0;
            Inventoiry = new Dictionary<Food, int>();
		}

        public Character(string name, string path, double balance, int bgChange,int currentBg, List<ChartEntry> BGs, Dictionary<Food,int> inventory)
        {
            Name = name;
            avatar_path = path;
            Balance = balance;
            LastBGs = BGs;
            BG_Change = bgChange;
            CurrentBG = currentBg;
            Inventoiry = inventory;
        }

		public void UpdateChart(ChartView chartView)
		{
            var chart = new LineChart
            {
                Entries = LastBGs,
                BackgroundColor = SKColor.Parse("#f7ecdc"),
                LineMode = LineMode.Spline,
                LineSize = 5,
                LabelTextSize = 30,
                PointMode = PointMode.Circle,
                PointSize = 10,
                ValueLabelOrientation = Orientation.Horizontal,
                LabelOrientation = Orientation.Horizontal
            };

            chartView.Chart = chart;
        }

        public void UpdateBG_List(ChartEntry BG)
        {
            if (LastBGs.Count >= 10)
            {
                LastBGs.RemoveAt(0);
            }
            LastBGs.Add(BG);
        }

        [Obsolete]
        public async Task EatFoodAsync(Food food)
        { 
            Inventoiry[food]--;
            await UpdateFBinventoryAsync();
            bool raises = rnd.NextDouble() < food.BG_IncreaseChance;
            bool lowers = rnd.NextDouble() < food.BG_DecreaseChance;

            if (raises) {
                BG_Change += food.IncreaseImpact;
                return; }
            if (lowers) {
                BG_Change -= food.DecreaseImpact;
            }
        }

        [Obsolete]
        private async Task UpdateFBinventoryAsync()
        {
            var userReference = FirebaseHelper.GetFirestore().Collection("users").Document(FirebaseHelper.GetFirebaseAuthentication().CurrentUser.Uid);
            var inventoryRef = userReference.Collection("characters").Document(Name).Collection("inventory");

            foreach (var entry in Inventoiry)
            {
                var food = entry.Key;
                var quantity = entry.Value;

                var foodDocRef = inventoryRef.Document(food.Name);

                if (quantity <= 0)
                {
                    // Delete the food from Firestore if it's no longer in the inventory
                    await foodDocRef.Delete();
                }
                else
                {
                    // Update the quantity
                    var update = new Dictionary<string, Java.Lang.Object>
                    {
                        { "quantity", new Java.Lang.Integer(quantity) }
                    };
                    await foodDocRef.Update(update);
                }
            }
        }

        public void UpdateBG()
        {
            CurrentBG += BG_Change;

            if(CurrentBG < 20 )
            {
                Balance += 1;
            }
            else if(CurrentBG >= 20 && CurrentBG <= 60)
            {
                Balance += 5;
            }
            else if(CurrentBG > 60 && CurrentBG <= 80)
            {
                Balance += 7;
            }
            else if(CurrentBG > 80 && CurrentBG <= 100)
            {
                Balance += 20;
            }
            else if (CurrentBG > 100 && CurrentBG <= 140)
            {
                Balance += 10;
            }
            else if (CurrentBG > 140 && CurrentBG <= 180)
            {
                Balance += 7;
            }
            else if (CurrentBG > 180 && CurrentBG <= 250)
            {
                Balance += 5;
            }
            else if (CurrentBG > 250 && CurrentBG <= 350)
            {
                Balance += 2;
            }
            else
            {
                Balance += 1;
            }
            BG_Change = 0;
        }

        
    }
}

