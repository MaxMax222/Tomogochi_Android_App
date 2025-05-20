using System;
using System.Collections.Generic;
using Firebase.Firestore;
using FinalProj_Tomogochi.Classes;
using Android.App;

namespace FinalProj_Tomogochi.Classes
{
    public class InventoryUpdateFBlistener : Java.Lang.Object, IEventListener
    {
        public Dictionary<Food, int> Inventory { get; private set; } = new Dictionary<Food, int>();
        public event EventHandler<InventoryArgs> OnInventoryRetrieved;

        public class InventoryArgs : EventArgs
        {
            public Dictionary<Food, int> Inventory { get; set; }
        }

        public InventoryUpdateFBlistener()
        {
            var character = User.GetUserInstance().ActiveCharacter;

            FirebaseHelper.GetFirestore()
                .Collection($"users/{FirebaseHelper.GetFirebaseAuthentication().CurrentUser.Uid}/characters")
                .Document(character.Name)
                .Collection("inventory")
                .AddSnapshotListener(this);
        }

        public void OnEvent(Java.Lang.Object value, FirebaseFirestoreException error)
        {
            var snapshot =(QuerySnapshot)value;
            var updatedInventory = new Dictionary<Food, int>();

            foreach (var foodDoc in snapshot.Documents)
            {
                var data = foodDoc.Data;

                if (data.TryGetValue("name", out var nameObj) &&
                    data.TryGetValue("raiseChance", out var raiseChanceObj) &&
                    data.TryGetValue("raiseImpact", out var raiseImpactObj) &&
                    data.TryGetValue("lowerChance", out var lowerChanceObj) &&
                    data.TryGetValue("lowerImpact", out var lowerImpactObj) &&
                    data.TryGetValue("price", out var priceObj) &&
                    data.TryGetValue("quantity", out var quantityObj) &&
                    data.TryGetValue("id", out var idObj))
                {
                    var food = new Food(
                        Application.Context,
                        nameObj.ToString(),
                        Convert.ToSingle(raiseChanceObj),
                        Convert.ToInt32(raiseImpactObj),
                        Convert.ToSingle(lowerChanceObj),
                        Convert.ToInt32(lowerImpactObj),
                        Convert.ToDouble(priceObj),
                        Convert.ToInt32(idObj)
                    );

                    int quantity = Convert.ToInt32(quantityObj);

                    updatedInventory[food] = quantity;
                }
            }

            if (OnInventoryRetrieved != null)
            {
                InventoryArgs e = new InventoryArgs();
                e.Inventory = updatedInventory;
                OnInventoryRetrieved.Invoke(this, e);
            }
            
        }
    }
}
