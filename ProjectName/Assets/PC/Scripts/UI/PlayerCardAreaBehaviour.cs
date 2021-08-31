using System.Collections;
using System.Collections.Generic;
using Assets.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.PC
{
    public class PlayerCardAreaBehaviour : MonoBehaviour
    {
        private CardBehaviour currentCard;
        public int connectionID;


        void Start()
        {
            connectionID = -1;
        }

        public void SetCurrentCard(CardBehaviour template, int connectionID)
        {
            this.connectionID = connectionID;
            currentCard = Instantiate(template);

            RectTransform t = this.transform as RectTransform;
            //currentCard.transform.position = t.TransformPoint(t.anchoredPosition + new Vector2(t.sizeDelta.x / 2, 0));
            currentCard.transform.position = t.TransformPoint(new Vector2(t.sizeDelta.x / 2, 0));

            currentCard.Init(this);
        }

        public void PlayCard(Card card){
            currentCard?.PlayCard(card);
        }
        public void DropCard(){
            currentCard?.DropCard();
        }

        public void RemoveCurrentCard(){
            connectionID = -1;
            Destroy(currentCard.gameObject);
        }
    }
}
