using System;
using Firebase.Firestore;
using FinalProj_Tomogochi.Classes;

namespace FinalProj_Tomogochi.Classes
{
    public class BalanceUpdateFBlistener : Java.Lang.Object, IEventListener
    {
        public double Balance;
        public event EventHandler<BalanceArgs> onBalanceRetrieved;

        public class BalanceArgs
        {
            internal double balance;
        }

        public BalanceUpdateFBlistener()
        {
            FirebaseHelper.GetFirestore().Collection($"characters").Document(User.GetUserInstance().Character.Name).AddSnapshotListener(this);
        }

        public void OnEvent(Java.Lang.Object value, FirebaseFirestoreException error)
        {
            var snapshot = (DocumentSnapshot)value;
            Balance = double.Parse(snapshot.Data["balance"].ToString());

            if (onBalanceRetrieved != null)
            {
                BalanceArgs e = new BalanceArgs();
                e.balance = Balance;
                onBalanceRetrieved.Invoke(this, e);
            }
        }
    }
}
