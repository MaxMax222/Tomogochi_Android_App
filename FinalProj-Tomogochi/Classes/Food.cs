using System;
namespace FinalProj_Tomogochi.Classes
{
	public class Food
	{
		public string Name { get; }
		public float BG_IncreaseChance { get; }
		public int IncreaseImpact { get; }
		public float BG_DecreaseChance { get; }
		public int DecreaseImpact { get; }
        public Food(string name, float raiseChance, int raiseImpact, float lowerChance, int lowerImpact)
        {
            Name = name;
            BG_IncreaseChance = raiseChance;
            IncreaseImpact = raiseImpact;
            BG_DecreaseChance = lowerChance;
            DecreaseImpact = lowerImpact;
        }
    }
}

