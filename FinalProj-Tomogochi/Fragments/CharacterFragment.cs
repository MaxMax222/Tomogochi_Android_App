
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Microcharts;
using Microcharts.Droid;
using SkiaSharp;
using AndroidX.Fragment.App;
using AndroidX.Core.Content;
using Android.App;

namespace FinalProj_Tomogochi.Fragments
{
    [Obsolete]
    public class CharacterFragment : AndroidX.Fragment.App.Fragment
    {
        
        [Obsolete]
        public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);

            View view = inflater.Inflate(Resource.Layout.character_view_screen, container, false);
            var chartView = view.FindViewById<ChartView>(Resource.Id.chartView);

            // Create sample data
            var entries = new List<ChartEntry>();
            var rnd = new Random();
            int sugar = 120;
            for (int i = 0; i < 15; i++)
            {
                string color = GetColorString(sugar, Application.Context);
                entries.Add(new ChartEntry(sugar)
                {
                    Label = DateTime.Now.Add(TimeSpan.FromMinutes(i)).ToString("HH:mm"),
                    ValueLabel = sugar.ToString(),
                    Color = SKColor.Parse(color)
                });
                sugar += rnd.Next(-30, 30);
            }

            var chart = new LineChart
            {
                Entries = entries,
                BackgroundColor = SKColor.Parse("#FFFFFF"),
                LineMode = LineMode.Spline,
                LineSize = 5,
                LabelTextSize = 30,
                PointMode = PointMode.Circle,
                PointSize = 10,
                ValueLabelOrientation = Microcharts.Orientation.Horizontal,
            };

            chartView.Chart = chart;

            return view;
        }

        private string GetColorString(int sugar, Context context)
        {
            int colorResId;

            if (sugar > 70 && sugar < 180)
                colorResId = Resource.Color.sea_green;
            else if (sugar >= 180)
                colorResId = Resource.Color.xanthous;
            else
                colorResId = Resource.Color.bittersweet_shimmer;

            int colorInt = ContextCompat.GetColor(context, colorResId);
            string hex = $"#{colorInt & 0xFFFFFF:X6}"; // Format as hex string like "#3CB371"
            return hex;
        }
    }
}

