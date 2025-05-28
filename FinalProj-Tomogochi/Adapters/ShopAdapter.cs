using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Gms.Extensions;
using Android.Views;
using Android.Widget;
using FinalProj_Tomogochi.Classes;
using Firebase.Firestore;

namespace FinalProj_Tomogochi.Adapters
{
    public class ShopAdapter : BaseAdapter<Food>
    {
        private readonly Context _context;
        private readonly List<Food> _foods;
        private DocumentReference characterRef;
        private CollectionReference inventoryRef;

        public ShopAdapter(Context context, List<Food> foods)
        {
            _context = context;
            _foods = foods;

            characterRef = FirebaseHelper.GetFirestore()
                .Collection("characters")
                .Document(User.GetUserInstance().Character.Name);
            inventoryRef = characterRef.Collection("inventory");
        }

        public override Food this[int position]
        {
            get { return _foods[position]; }
        }

        public override int Count
        {
            get { return _foods.Count; }
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var food = _foods[position];
            var view = convertView;

            view ??= LayoutInflater.From(_context).Inflate(Resource.Layout.shop_food_item, parent, false);
            var icon = view.FindViewById<ImageView>(Resource.Id.item_image);
            icon.SetBackgroundResource(food.imgResource);

            var name_txt = view.FindViewById<TextView>(Resource.Id.food_name_txt);
            name_txt.Text = food.Name;

            var price_txt = view.FindViewById<TextView>(Resource.Id.price_txt);
            price_txt.Text = $"{food.Price}$";

            var info_txt = view.FindViewById<TextView>(Resource.Id.food_info_txt);
            info_txt.Text = $"BG raise: {food.IncreaseImpact} \n Raise chance: {food.BG_IncreaseChance * 100}% \n BG decrease: {food.DecreaseImpact} \n Decrease chance: {food.BG_DecreaseChance * 100}%";

            var purchase_btn = view.FindViewById<Button>(Resource.Id.purchase_btn);

            purchase_btn.Tag = position;
            purchase_btn.Click -= Purchase_btn_Click;
            purchase_btn.Click += Purchase_btn_Click;

            return view;
        }

        private void Purchase_btn_Click(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            int pos = (int)button.Tag;
            Food food = _foods[pos];

            if (User.GetUserInstance().Character.Balance >= food.Price)
            {
                // add food to inventory firebase inventory, update balance on firebase
                AddToInventory(food);
                ChargeCharacter(food);
            }
            else
            {
                Toast.MakeText(_context, "insufficent funds", ToastLength.Short).Show();
            }
        }

        private async void AddToInventory(Food food)
        {
            try
            {
                // Use the food name as the document ID
                var foodDocRef = inventoryRef.Document(food.Name);
                var snapshot = (DocumentSnapshot)await foodDocRef.Get();

                if (snapshot.Exists())
                {
                    User.GetUserInstance().Character.Inventoiry[food] += 1;
                    var currentQuantity = (int)snapshot.Get("quantity");
                    await foodDocRef.Update("quantity", currentQuantity + 1);
                }
                else
                {
                    User.GetUserInstance().Character.Inventoiry[food] = 1;
                    // Add new food document
                    var foodData = new Dictionary<string, Java.Lang.Object>
                    {
                        { "name", food.Name },
                        { "raiseChance", food.BG_IncreaseChance },
                        { "raiseImpact", food.IncreaseImpact },
                        { "lowerChance", food.BG_DecreaseChance },
                        { "lowerImpact", food.DecreaseImpact },
                        { "price", food.Price },
                        { "quantity", 1 },
                        {"id", food.imgResource }
                    };
                    await foodDocRef.Set(foodData);
                }

                Toast.MakeText(_context, $"{food.Name} added to inventory!", ToastLength.Short).Show();
            }
            catch (Exception ex)
            {
                Toast.MakeText(_context, $"Error adding to inventory: {ex.Message}", ToastLength.Long).Show();
            }
        }


        private void ChargeCharacter(Food food)
        {
            User.GetUserInstance().Character.Balance -= food.Price;
            characterRef.Update("balance", User.GetUserInstance().Character.Balance.ToString());
        }
    }
}

