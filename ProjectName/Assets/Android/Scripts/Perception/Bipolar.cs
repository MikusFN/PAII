using System.Collections;
using System.Collections.Generic;
using Assets.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Android
{
    public class Bipolar : SingletonBehaviour<Bipolar>
    {
        private Text infoText;
        private Card lastCardPlayed;

        protected override void Awake()
        {
            base.Awake();
            infoText = GameObject.Find("BipolarInfoText")?.GetComponent<Text>();
            UIManagerAndroid.Instance.ShowMentalText("You are now bipolar! You must keep playing good cards, and bad cards in sequence!", 3);
        }

        public bool CanPlay(Card c)
        {
            if (lastCardPlayed == null || c.reputationDeltaImpact == 0 || lastCardPlayed.reputationDeltaImpact == 0 || lastCardPlayed.reputationDeltaImpact * c.reputationDeltaImpact < 0)
            {
                lastCardPlayed = c;
                return true;
            }

            string text = (lastCardPlayed.reputationDeltaImpact > 0)? "You must play a bad card!" : "You must play a good card!" ;
            UIManagerAndroid.Instance.ShowMentalText(text, 3);
            DebugApplication.DebugConsole.Instance.Log("YOU ARE BIPOLAR ACT LIKE ONE");
            return false;
        }
    }
}
