
using System;
using System.Collections.Generic;

using Android.Content;
using Microcharts;
using Microcharts.Droid;
using SkiaSharp;
using AndroidX.Core.Content;
using Android.App;

namespace FinalProj_Tomogochi.Classes
{
    public class Character
	{
		public string Name { get; }
		public string avatar_path{ get; }
		public int CurrentBG { get; private set; }
		public List<ChartEntry> LastBGs; 
        public int BG_Change { get; private set; }
        private Random rnd;
        public DateTime LastActive { get; set; }
		public Character(string name, string path)
		{
            Name = name;
            avatar_path = path;
            CurrentBG = 120;
            LastBGs = new List<ChartEntry> { new ChartEntry(CurrentBG) {
                Label = DateTime.Now.ToString("HH:mm"),
                ValueLabel = CurrentBG.ToString(),
                Color = SKColor.Parse(GetColorString(CurrentBG, Application.Context))
            } };
            BG_Change = 0;
            rnd = new Random();
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

        public void CalculateAffectBG(Food food)
        {
            bool raises = rnd.NextDouble() < food.BG_IncreaseChance;
            bool lowers = rnd.NextDouble() < food.BG_DecreaseChance;

            if (raises) {
                BG_Change += food.IncreaseImpact;
                return; }
            if (lowers) {
                BG_Change -= food.DecreaseImpact;
            }
        }

        public void UpdateBG()
        {
            CurrentBG += BG_Change;
            BG_Change = 0;
        }

        public string GetColorString(int sugar, Context context)
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

