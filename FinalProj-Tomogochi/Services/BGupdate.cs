
using Android.App;
using Android.Content;
using Android.OS;
using AndroidX.Core.App;
using System.Timers;
using Firebase.Firestore;
using FinalProj_Tomogochi.Classes;
using Java.Util;
using System.Collections.Generic;
using Android.Widget;
using System;
using Android.Gms.Extensions;

namespace FinalProj_Tomogochi.Services
{
	[Service]
	public class BGupdate : Service
	{
        public static bool IsRunning { get; private set; }

        private System.Timers.Timer timer;
		private Character character;
		private DocumentReference characterRef;
		private CollectionReference BGcollectionRef;


        public override void OnCreate()
        {
            base.OnCreate();
            IsRunning = true;
            character = User.GetUserInstance().Character;
            characterRef = FirebaseHelper.GetFirestore()
                .Collection("characters")
                .Document(character.Name);

            BGcollectionRef = characterRef.Collection("lastBGs");

            StartTimer();
        }

        private void StartTimer()
        {
            timer = new System.Timers.Timer(60000); // 60 seconds
            timer.Elapsed += async (s, e) =>
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

                    var bgMap = new HashMap();
                    bgMap.Put("label", time.ToString("HH:mm"));
                    bgMap.Put("value", character.CurrentBG);
                    await BGcollectionRef.Add(bgMap);

                    QuerySnapshot snapshot = (QuerySnapshot)await BGcollectionRef.OrderBy("label").Get();
                    if (snapshot.Documents.Count > 10)
                    {
                        for (int i = 0; i < snapshot.Documents.Count - 10; i++)
                        {
                            await snapshot.Documents[i].Reference.Delete();
                        }
                    }
                }
                catch (Exception ex)
                {
                }
            };
            timer.Start();
        }


        public override StartCommandResult OnStartCommand(Android.Content.Intent intent, StartCommandFlags flags, int startId)
		{
            CreateNotificationChannel();

            var notification = new NotificationCompat.Builder(this, "bg_sync_channel")
                .SetContentTitle("Tamagotchi BG Sync")
                .SetContentText("Syncing blood glucose every minute")
                .SetSmallIcon(Resource.Drawable.anonymus)
                .SetOngoing(true)
                .Build();

            StartForeground(1001, notification);

            return StartCommandResult.Sticky;
        }

        private void CreateNotificationChannel()
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                var channel = new NotificationChannel("bg_sync_channel", "BG Sync Channel", NotificationImportance.Default)
                {
                    Description = "Channel for BG foreground service"
                };
                var manager = (NotificationManager)GetSystemService(NotificationService);
                manager.CreateNotificationChannel(channel);
            }
        }

        public override IBinder OnBind(Intent intent) => null;

        public override void OnDestroy()
        {
            base.OnDestroy();
            IsRunning = false;
            timer?.Stop();
            timer?.Dispose();
        }
    }		
}

