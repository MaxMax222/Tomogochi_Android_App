using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using FinalProj_Tomogochi.Classes;

namespace FinalProj_Tomogochi.Adapters
{
    public class ShopAdapter : BaseAdapter<Food>
    {
        private readonly Context _context;
        private readonly List<Food> _foods;
        private Dialog dialog;

        public ShopAdapter(Context context, List<Food> foods)
        {
            _context = context;
            _foods = foods;
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

            var price_txt = view.FindViewById<TextView>(Resource.Id.price_txt);
            price_txt.Text = $"{food.Price}$";

            var info_txt = view.FindViewById<TextView>(Resource.Id.food_info_txt);
            info_txt.Text = $"BG raise: {food.IncreaseImpact} \n Raise chance: {food.BG_IncreaseChance} \n BG decrease: {food.DecreaseImpact} \n Decrease chance: {food.BG_DecreaseChance}";

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

            if (User.GetUserInstance().ActiveCharacter.Balance > food.Price)
            {
                User.GetUserInstance().ActiveCharacter.Balance -= food.Price;
                // add food to inventory firebase inventory, update balance on firebase
            }
            else
            {
                Toast.MakeText(_context, "insufficent funds", ToastLength.Short);
            }
        }
    }
}

