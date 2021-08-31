using System.Collections;
using System.Collections.Generic;
using Assets.Generic;
using UnityEngine;

namespace Assets.Android
{
    public class Hysteria : SingletonBehaviour<Hysteria>
    {
        public float TIME_TO_PLAY_CARDS;
        private float timer;


        protected override void Awake() {
            base.Awake();
            ResetTimer();
            UIManagerAndroid.Instance.ShowMentalText("You are now hyseric! Play or draw card fast, or they will be played for you!", 3);
        }

        private void Update() {
            timer -= Time.deltaTime;

            if(timer <= 0){
                var cardToPlay = HandController.Instance.GetRandomCardFromHand();
                if(cardToPlay != null){
                    HandController.Instance.PlayCard(cardToPlay);
                    UIManagerAndroid.Instance.ShowMentalText("Too long! A card has been played for you", 2);
                }
            }
        }

        public void ResetTimer(){
                timer = TIME_TO_PLAY_CARDS;
        }
    }
}
