using System;
using System.Threading.Tasks;
using Microcharts;
using SkiaSharp;

namespace FinalProj_Tomogochi.Classes
{
	public static class BGUpdateManager
	{

		public static async Task PerformCalculationAndUpdateAsync(bool isForeground)
		{
			var user = User.GetUserInstance();
			var character = user.ActiveCharacter;

			if (character == null) return;

			character.UpdateBG();

            var chartEntry = new ChartEntry(character.CurrentBG)
            {
                Label = DateTime.Now.ToString("HH:mm"),
                ValueLabel = character.CurrentBG.ToString(),
                Color = SKColor.Parse("#00FF00") // Or use GetColorString
            };

            character.UpdateBG_List(chartEntry);

        }
    }
}

