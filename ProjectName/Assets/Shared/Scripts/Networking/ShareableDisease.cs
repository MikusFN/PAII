using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Generic;
using UnityEngine;

namespace Assets.Network
{
    [Serializable]
    public class ShareableDisease
    {
        public string diseaseCardID;
        public ClientColor color;

        public ShareableDisease(Card c, Color color){
            this.diseaseCardID = c.id;
            this.color = new ClientColor(color);
        }
    }
}

