using System;
using System.Collections.Generic;
using Android.App;
using Firebase.Firestore;
using Microcharts;
using SkiaSharp;
using static Java.Util.Jar.Attributes;

namespace FinalProj_Tomogochi.Classes
{
	public class BGupdateFBlistener : Java.Lang.Object, IEventListener
	{
        public List<ChartEntry> BGs;

        public class BGEntriesEventArgs
        {
            internal List<ChartEntry> BGEntries { get; set; }
        }

        public event EventHandler<BGEntriesEventArgs> OnBGEntryRetrieved;

		public BGupdateFBlistener()
		{
            FirebaseHelper.GetFirestore().Collection($"characters/{User.GetUserInstance().Character.Name}/lastBGs").OrderBy("label").AddSnapshotListener(this);
        }

        public void OnEvent(Java.Lang.Object value, FirebaseFirestoreException error)
        {
            var spanshot = (QuerySnapshot)value;
            BGs = new List<ChartEntry>();
            foreach(DocumentSnapshot BG in spanshot.Documents)
            {
                string label = BG.Get("label").ToString();
                string BGvalue = BG.Get("value").ToString();
                BGs.Add(new ChartEntry(int.Parse(BGvalue)) {
                    Label = label,
                    ValueLabel = BGvalue,
                    Color = SKColor.Parse(User.GetColorString(int.Parse(BGvalue), Application.Context))
                });
            }

            if(OnBGEntryRetrieved != null)
            {
                BGEntriesEventArgs e = new BGEntriesEventArgs();
                e.BGEntries = BGs;
                OnBGEntryRetrieved.Invoke(this, e);
            }
        }
    }

    
}

