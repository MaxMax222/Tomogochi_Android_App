using System;
using Android.Content;
using AndroidX.Work;

namespace FinalProj_Tomogochi.Classes
{
	public class BGCalculationWorker : Worker
	{
		public BGCalculationWorker(Context context, WorkerParameters workerParams)
            : base(context, workerParams)
		{   }

        public override Result DoWork()
        {
            try
            {
                var user = User.GetUserInstance();
                var activeChar = user.ActiveCharacter;
                if (activeChar != null)
                {
                    activeChar.UpdateBG();
                    SaveToFirebase(activeChar);
                }
                return Result.InvokeSuccess();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Background worker error: " + ex.Message);
                return Result.InvokeFailure();
            }
        }
        private async void SaveToFirebase(Character character)
        {
            try
            {
                await User.GetUserInstance().SaveCharacterToFirestoreAsync(character);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Firebase update failed: " + e.Message);
            }
        }
    }
}

