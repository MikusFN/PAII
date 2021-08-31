using System.Collections;
using System.Collections.Generic;
using Assets.Generic;
using Assets.Network;
using UnityEngine;

namespace Assets.PC
{
    public class CardPlayerWrapper
    {
        public string cardID;
        public string playerIP;

        public CardPlayerWrapper(string cardID, string playerIP){
            this.cardID = cardID;
            this.playerIP = playerIP;
        }

        public Card GetCard(){
            return AssetManager.Instance.GetObject(cardID) as Card;
        }

        public Client GetClient(){
            return GameManagerPC.Instance.FindClientByIP(playerIP);
        }
    }
}

