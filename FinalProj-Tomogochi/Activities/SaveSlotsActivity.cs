using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Bumptech.Glide;
using FinalProj_Tomogochi.Classes;

namespace FinalProj_Tomogochi.Activities
{
    [Activity(Label = "SaveSlotsActivity")]
    public class SaveSlotsActivity : Activity
    {
        private TextView[] textViews_info;
        private ImageView[] avatars;
        private LinearLayout[] slots;

        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.save_slots_screen);
            await User.GetUserInstance().LoadCharactersFromFirestoreAsync();
            Init();
        }

        private void Init()
        {
            textViews_info = new TextView[3];
            avatars = new ImageView[3];
            slots = new LinearLayout[3];

            for (int i = 0; i < 3; i++)
            {
                int slotId = Resources.GetIdentifier($"slot{i + 1}", "id", PackageName);
                int infoId = Resources.GetIdentifier($"info{i + 1}", "id", PackageName);
                int avatarId = Resources.GetIdentifier($"avatar_imgView{i + 1}", "id", PackageName);

                slots[i] = FindViewById<LinearLayout>(slotId);
                textViews_info[i] = FindViewById<TextView>(infoId);
                avatars[i] = FindViewById<ImageView>(avatarId);

                slots[i].Tag = i;
                slots[i].Click += LoadSlot;
            }

            var characters = User.GetUserInstance().characters;
            for (int i = 0; i < 3; i++)
            {
                if (i < characters.Count)
                {
                    SetSlotData(i, characters[i].LastActive.ToString("HH:mm"), characters[i].avatar_path);
                }
                else
                {
                    SetSlotData(i, "Empty slot", null);
                }
            }
        }

        private void SetSlotData(int index, string infoText, string avatarUrl)
        {
            textViews_info[index].Text = infoText;
            if (!string.IsNullOrEmpty(avatarUrl))
            {
                Glide.With(this)
                    .Load(avatarUrl)
                    .Error(Resource.Drawable.anonymus)
                    .Into(avatars[index]);
            }
            else
            {
                avatars[index].SetImageDrawable(null); // Clear image for empty slot
            }
        }

        private void LoadSlot(object sender, EventArgs e)
        {
            var slot = (LinearLayout)sender;
            int index = (int)slot.Tag;
            var characters = User.GetUserInstance().characters;

            if (index < characters.Count && characters[index] != null)
            {
                User.GetUserInstance().ActiveCharacter = characters[index];
                User.GetUserInstance().BGlistener =  new BGupdateFBlistener();
                User.GetUserInstance().BalanceListener =  new BalanceUpdateFBlistener();
                StartActivity(new Intent(this, typeof(MainActivity)));
            }
            else
            {
                StartActivityForResult(new Intent(this, typeof(CharCreationActivity)),1001);
            }
        }

        protected override async void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (requestCode == 1001 && resultCode == Result.Ok)
            {
                // Reload characters
                await User.GetUserInstance().LoadCharactersFromFirestoreAsync();

                // Re-initialize slots
                Init();
            }
        }
    }
}
