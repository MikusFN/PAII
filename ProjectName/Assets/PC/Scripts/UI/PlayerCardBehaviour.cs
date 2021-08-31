using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.PC
{
    public class PlayerCardBehaviour : MonoBehaviour
    {
        private GameObject currentCard;
        public int connectionID;

        void Start()
        {
            connectionID = -1;
        }

        public void SetCurrentCard(GameObject template, int connectionID)
        {
            this.connectionID = connectionID;
            currentCard = Instantiate(template);

            RectTransform t = this.transform as RectTransform;
            //currentCard.transform.position = t.TransformPoint(t.anchoredPosition + new Vector2(t.sizeDelta.x / 2, 0));
            currentCard.transform.position = t.TransformPoint(new Vector2(t.sizeDelta.x / 2, 0));
        }
        public void RemoveCurrentCard(){
            connectionID = -1;
            Destroy(currentCard);
        }
    }
}
