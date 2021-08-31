using Assets.Network;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Android { 

public class drawOneCard : MonoBehaviour
    {
        public void drawOne()
        {
            if (NetworkAndroid.Instance.CurrentClient.Money >= 100)
            {
                NetworkAndroid.Instance.SendMessage(new Network.MessageBase(((byte)1), MessageType.DrawCards), NetworkAndroid.Instance.ServerConnectionID, NetworkAndroid.reliableChannelID);
                Hysteria.Instance?.ResetTimer();
            }
        
        }
    }
}
