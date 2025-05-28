
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
using Android.Widget;
using Orientation = Microcharts.Orientation;

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
        public double Balance { get; set; }
        public Dictionary<Food, int> Inventoiry;

		public Character(string name, string path)
		{
            Name = name;
            avatar_path = path;
            CurrentBG = rnd.Next(60,251);
            LastBGs = new List<ChartEntry> { new ChartEntry(CurrentBG) {
                Label = DateTime.Now.ToString("HH:mm"),
                ValueLabel = CurrentBG.ToString(),
                Color = SKColor.Parse(User.GetColorString(CurrentBG, Application.Context))
            } };
            BG_Change = rnd.Next(-15,16);
            Balance = GetLastBalanceGain();
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
                BackgroundColor = SKColor.Parse("#C6DDF0"),
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
            double chance = rnd.NextDouble();
            bool raises = chance < food.BG_IncreaseChance;
            bool lowers = chance < food.BG_DecreaseChance;

            if (raises) {
                BG_Change += food.IncreaseImpact;
                Toast.MakeText(Application.Context, food.Name + " will encrease BG",ToastLength.Short).Show();
                }
            else if (lowers) {
                BG_Change -= food.DecreaseImpact;
                Toast.MakeText(Application.Context, food.Name + " will lower BG", ToastLength.Short).Show();
            }
            else
            {
                Toast.MakeText(Application.Context, food.Name + " will not affect BG", ToastLength.Short).Show();
            }
            await FirebaseHelper.GetFirestore().Collection("characters").Document(Name)
                .Update("bgChange", BG_Change.ToString());
        }

        [Obsolete]
        private async Task UpdateFBinventoryAsync()
        {
            var inventoryRef = FirebaseHelper.GetFirestore().Collection("characters").Document(Name).Collection("inventory");

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
            Balance += GetLastBalanceGain();

            // Predict the impact of next BG_Change on Balance
            int potentialChange = PredictBGChange();
            BG_Change = potentialChange;
        }

        private int PredictBGChange()
        {
            int min = -15;
            int max = 15;

            // Predict impact: the higher the last Balance gain, the bigger the next possible swing
            int lastBalanceGain = GetLastBalanceGain();

            // Scale the BG change range based on previous balance gain
            // This will stretch the possible impact range
            if (lastBalanceGain >= 20)
            {
                min = -40;
                max = 40;
            }
            else if (lastBalanceGain >= 10)
            {
                min = -20;
                max = 20;
            }
            else if (lastBalanceGain >= 5)
            {
                min = -15;
                max = 15;
            }

            return rnd.Next(min, max + 1);
        }

        private int GetLastBalanceGain()
        {
            if (CurrentBG < 20)
                return 1;
            else if (CurrentBG <= 60)
                return 5;
            else if (CurrentBG <= 80)
                return 7;
            else if (CurrentBG <= 100)
                return 20;
            else if (CurrentBG <= 140)
                return 10;
            else if (CurrentBG <= 180)
                return 7;
            else if (CurrentBG <= 250)
                return 5;
            else if (CurrentBG <= 350)
                return 2;
            else
                return 1;
        }


    }
}

