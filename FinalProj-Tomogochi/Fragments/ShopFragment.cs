using System;
using System.Collections.Generic;

using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using FinalProj_Tomogochi.Classes;
using FinalProj_Tomogochi.Adapters;

namespace FinalProj_Tomogochi.Fragments
{
    [Obsolete]
    public class ShopFragment : AndroidX.Fragment.App.Fragment
    {
        private Character character;
        private ListView listView;
        private TextView balance;
        private BalanceUpdateFBlistener listener = User.GetUserInstance().BalanceListener;
        private List<Food> foods;
        [Obsolete]
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            character = User.GetUserInstance().Character;
            View view = inflater.Inflate(Resource.Layout.shop_screen, container, false);
            listener.onBalanceRetrieved += Listener_onBalanceRetrieved;

            balance = view.FindViewById<TextView>(Resource.Id.balance_txt);
            balance.Text = $"Current Balance: {character.Balance}$";
            var context = Application.Context;
            foods = new List<Food>
            {
                new Food(context, "Apple",      0.15f,  5,  0.7f, 16, 10),
                new Food(context, "Banana",     0.35f, 15,  0.2f,  5, 13),
                new Food(context, "Orange",     0.20f,  8,  0.6f, 12, 13),
                new Food(context, "Pizza",      0.85f, 40,  0.0f,  0, 9),
                new Food(context, "Salad",      0.05f,  3,  0.7f, 20, 15),
                new Food(context, "Rice",       0.65f, 30,  0.1f,  3, 9),
                new Food(context, "Candy",      0.95f, 50,  0.0f,  0, 5),
                new Food(context, "Yogurt",     0.30f, 12,  0.3f,  8, 10),
                new Food(context, "Soda",       0.98f, 60,  0.0f,  0, 5),
                new Food(context, "Burger",     0.80f, 45,  0.05f, 2, 9),
                new Food(context, "Steak",      0.40f, 20,  0.3f,  7, 13),
                new Food(context, "Broccoli",   0.05f,  4,  0.7f, 22, 18),
                new Food(context, "Oatmeal",    0.25f, 18,  0.4f, 12, 13),
                new Food(context, "Chocolate",  0.80f, 35,  0.1f,  5, 9),
                new Food(context, "Tuna",       0.10f, 10,  0.6f, 18, 15),
                new Food(context, "Pasta",      0.60f, 25,  0.1f,  4, 9),
                new Food(context, "Avocado",    0.10f,  8,  0.5f, 17, 15),
                new Food(context, "Carrot",     0.12f,  6,  0.6f, 20, 15),
                new Food(context, "Insulin",    0.0f,  0,  1.0f, 50, 30)
            };



            listView = view.FindViewById<ListView>(Resource.Id.shop_lstvw);
            var screen_height = Resources.DisplayMetrics.HeightPixels;
            listView.LayoutParameters.Height = (int)(screen_height * 0.8);
            listView.Adapter = new ShopAdapter(context, foods);
            return view;

        }

        private void Listener_onBalanceRetrieved(object sender, BalanceUpdateFBlistener.BalanceArgs e)
        {
            balance.Text = $"Current Balance: {character.Balance}$";
        }
    }
}
