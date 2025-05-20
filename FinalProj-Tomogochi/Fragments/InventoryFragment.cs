
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
using FinalProj_Tomogochi.Adapters;

namespace FinalProj_Tomogochi.Fragments
{
    [Obsolete]
    public class InventoryFragment : AndroidX.Fragment.App.Fragment
    {
		private Character character;


		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
            character = User.GetUserInstance().ActiveCharacter;

            View view = inflater.Inflate(Resource.Layout.inventory_screen, container, false);
            var recyclerView = view.FindViewById<RecyclerView>(Resource.Id.inventoryRecyclerView);
            recyclerView.SetLayoutManager(new LinearLayoutManager(Application.Context));

            var inventory = character.Inventoiry;
            var adapter = new FoodIncentoryAapter(inventory);
            recyclerView.SetAdapter(adapter);

            return view;
        }
    }
}

