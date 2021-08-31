using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Network
{
    /// <summary>
    /// This is simple version of Client. This version only contains data that other players can see.
    /// </summary>
    [Serializable]
    public class ShareableClient
    {
        public string Ip; //Player ip.
        public string Name; //Player public name.
        public ClientColor ClientColor;

        public ShareableClient(Client clone, string Ip){
            this.Ip = Ip;
            this.Name = clone.Name;
            this.ClientColor = clone.ClientColor;
        }
    }
}
