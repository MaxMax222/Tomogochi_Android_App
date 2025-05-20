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
            character = User.GetUserInstance().ActiveCharacter;
            View view = inflater.Inflate(Resource.Layout.shop_screen, container, false);
            listener.onBalanceRetrieved += Listener_onBalanceRetrieved;

            balance = view.FindViewById<TextView>(Resource.Id.balance_txt);
            balance.Text = $"Current Balance: {character.Balance}$";
            var context = Application.Context;
            foods = new List<Food>
            {
                new Food(context, "Apple",          0.1f,  5,  0.7f, 10, 4),
                new Food(context, "Banana",         0.5f, 15,  0.2f,  5, 5),
                new Food(context, "Orange",         0.2f,  8,  0.6f,  9, 5),
                new Food(context, "Pizza",          0.8f, 40,  0.0f,  0, 3),
                new Food(context, "Salad",          0.1f,  3,  0.7f, 12, 6),
                new Food(context, "Rice",           0.6f, 30,  0.1f,  3, 3),
                new Food(context, "Candy",          0.9f, 50,  0.0f,  0, 2),
                new Food(context, "Yogurt",         0.4f, 12,  0.3f,  6, 4),
                //new Food(context, "Soda",           0.95f,60,  0.0f,  0, 2),
                //new Food(context, "Grilled Chicken",0.2f, 10,  0.5f, 15, 7),
                //new Food(context, "Burger",         0.85f,45,  0.05f, 2, 3),
                //new Food(context, "Steak",          0.5f, 20,  0.3f,  7, 5),
                //new Food(context, "Broccoli",       0.1f,  4,  0.7f, 14, 7),
                //new Food(context, "Ice Cream",      0.9f, 55,  0.0f,  0, 2),
                //new Food(context, "Oatmeal",        0.3f, 18,  0.4f,  9, 5),
                //new Food(context, "Chocolate",      0.75f,35,  0.1f,  5, 3),
                //new Food(context, "Tuna",           0.2f, 10,  0.6f, 10, 6),
                //new Food(context, "Pasta",          0.6f, 25,  0.1f,  4, 3),
                //new Food(context, "Avocado",        0.2f,  8,  0.5f, 11, 6),
                //new Food(context, "Carrot",         0.15f,6,  0.6f, 13, 6)

            };
            listView = view.FindViewById<ListView>(Resource.Id.shop_lstvw);
            listView.Adapter = new ShopAdapter(context, foods);
            return view;

        }

        private void Listener_onBalanceRetrieved(object sender, BalanceUpdateFBlistener.BalanceArgs e)
        {
            balance.Text = $"Current Balance: {character.Balance}$";
        }
    }
}
