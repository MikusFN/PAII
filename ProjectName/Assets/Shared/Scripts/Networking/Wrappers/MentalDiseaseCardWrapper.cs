using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Generic;
using UnityEngine;

namespace Assets.Network
{
    [Serializable]
    public class MentalDiseaseCardWrapper
    {
        public string cardID;
        public string playerIP;
    
        public MentalDiseaseCardWrapper(Card c, string ip){
            cardID = c.id;
            playerIP = ip;
        }
    }
}
