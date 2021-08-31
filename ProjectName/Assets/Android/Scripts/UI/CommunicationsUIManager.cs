using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace Assets.Android
{

    public class CommunicationsUIManager : MonoBehaviour
    {
        public Button btnShowHideChat;
        public Text message;
        public Dropdown drop;
        Dictionary<int, string> idNome = new Dictionary<int, string>();


        public void dropFilling()
        {

        }

        public void sendMessage()
        {
            NetworkAndroid.Instance.SendMessage(new Network.MessageBase(new KeyValuePair<int, string>(drop.value, message.text), Network.MessageType.ClientSendText), NetworkAndroid.Instance.ServerConnectionID, NetworkAndroid.reliableChannelID);
            message.text = "";
        }
    }
}

