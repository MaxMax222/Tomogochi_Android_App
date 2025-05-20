using System;
using Android.Content;

namespace FinalProj_Tomogochi.Classes
{
    public class Food
    {
        public string Name { get; }
        public float BG_IncreaseChance { get; }
        public int IncreaseImpact { get; }
        public float BG_DecreaseChance { get; }
        public int DecreaseImpact { get; }
        public int imgResource { get; }
        public double Price { get; }
        public Food(Context context, string name, float raiseChance, int raiseImpact, float lowerChance, int lowerImpact, double price)
        {
            Price = price;
            Name = name;
            BG_IncreaseChance = raiseChance;
            IncreaseImpact = raiseImpact;
            BG_DecreaseChance = lowerChance;
            DecreaseImpact = lowerImpact;
            imgResource = context.Resources.GetIdentifier(name.ToLower(), "drawable", context.PackageName);
        }
        public Food(Context context, string name, float raiseChance, int raiseImpact, float lowerChance, int lowerImpact, double price, int imgId)
        {
            Price = price;
            Name = name;
            BG_IncreaseChance = raiseChance;
            IncreaseImpact = raiseImpact;
            BG_DecreaseChance = lowerChance;
            DecreaseImpact = lowerImpact;
            imgResource = imgId;
        }
        public override bool Equals(object obj)
        {
            if (obj is Food other)
            {
                return this.Name == other.Name;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}
