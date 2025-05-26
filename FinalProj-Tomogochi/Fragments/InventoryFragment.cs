
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using FinalProj_Tomogochi.Adapters;
using FinalProj_Tomogochi.Classes;

namespace FinalProj_Tomogochi.Fragments
{
    [Obsolete]
    public class InventoryFragment : AndroidX.Fragment.App.Fragment
    {
		private Character character;
        private InventoryUpdateFBlistener listener = User.GetUserInstance().InventoryListener;

        private RecyclerView recyclerView;
        private FoodIncentoryAapter adapter;

        public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
            character = User.GetUserInstance().Character;

            View view = inflater.Inflate(Resource.Layout.inventory_screen, container, false);
            listener.OnInventoryRetrieved += Listener_OnInventoryRetrieved;


            recyclerView = view.FindViewById<RecyclerView>(Resource.Id.inventoryRecyclerView);
            recyclerView.SetLayoutManager(new LinearLayoutManager(Application.Context));

            var inventory = character.Inventoiry;
            adapter = new FoodIncentoryAapter(inventory);
            recyclerView.SetAdapter(adapter);

            return view;
        }

        private void Listener_OnInventoryRetrieved(object sender, InventoryUpdateFBlistener.InventoryArgs e)
        {
            character.Inventoiry = e.Inventory;
            adapter.UpdateInventory(e.Inventory);
        }
    }
}

