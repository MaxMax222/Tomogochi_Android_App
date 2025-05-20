using System;
using System.Collections.Generic;
using System.Linq;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using FinalProj_Tomogochi.Classes;

namespace FinalProj_Tomogochi.Adapters
{
	public class FoodIncentoryAapter : RecyclerView.Adapter
	{
        private List<KeyValuePair<Food, int>> items;

        public FoodIncentoryAapter(Dictionary<Food, int> inventory)
		{
            items = inventory.ToList();
		}

        public override int ItemCount => items.Count;

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var viewHolder = holder as FoodViewHolder;
            var item = items[position];

            viewHolder.FoodName.Text = item.Key.Name;
            viewHolder.Quantity.Text = $"Quantity: {item.Value}";
            viewHolder.Effects.Text = $"BG raise: {item.Key.IncreaseImpact} \n Raise chance: {item.Key.BG_IncreaseChance * 100}% \n BG decrease: {item.Key.DecreaseImpact} \n Decrease chance: {item.Key.BG_DecreaseChance * 100}%";
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.inventory_item, parent, false);
            return new FoodViewHolder(itemView);
        }
        public void UpdateInventory(Dictionary<Food, int> newInventory)
        {
            items = newInventory.ToList();
            NotifyDataSetChanged();
        }
    }

    public class FoodViewHolder : RecyclerView.ViewHolder
    {
        public TextView FoodName { get; }
        public TextView Quantity { get; }
        public TextView Effects { get; }

        public FoodViewHolder(View itemView) : base(itemView)
        {
            FoodName = itemView.FindViewById<TextView>(Resource.Id.foodNameText);
            Quantity = itemView.FindViewById<TextView>(Resource.Id.quantityText);
            Effects = itemView.FindViewById<TextView>(Resource.Id.effectsText);
        }
    }

    
}

