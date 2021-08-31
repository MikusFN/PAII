using Assets.Generic;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Android
{
    public class ResearchCardTemplateManager : MonoBehaviour
    {
        private Card card;
        public Image c_image;
        public Text c_name;
        public Text c_price;
        public Text c_description;

        public float costExtra = 2;
        public void Init(Card c)
        {
            this.card = c;
            this.c_image.overrideSprite = c.texture;
            this.c_name.text = c.name;
            this.c_price.text = (c.cost * costExtra).ToString();
            this.c_description.text = c.description;
        }

        public void buyCard()
        {

            if (NetworkAndroid.Instance.CurrentClient.Money > (card.cost * costExtra) )
            {
            NetworkAndroid.Instance.CurrentClient.Money -= (card.cost * costExtra);
            NetworkAndroid.Instance.SendMessage( new Network.MessageBase(card.id, Network.MessageType.DrawSpecificCard) ,NetworkAndroid.Instance.ServerConnectionID, NetworkAndroid.reliableChannelID);
   
            }
            //else
            //{
                //mostrar algo
            //}
        }
    }

}

