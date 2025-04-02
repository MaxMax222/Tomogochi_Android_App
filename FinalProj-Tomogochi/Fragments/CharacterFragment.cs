
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

namespace FinalProj_Tomogochi.Fragments
{
    [Obsolete]
    public class CharacterFragment : Fragment
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
                string color = GetColorString(sugar);
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

        private string GetColorString(int sugar)
        {
            if (sugar > 70 && sugar < 180)
                return "ADFF2F"; // Green
            if (sugar >= 180)
                return "FFEF00"; // Yellow
            return "ED1B24"; // Red
        }
    }
}

