using Assets.Network;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Android
{
    public class PlayerNameIDTemplate : MonoBehaviour
    {
        public Text namePlayer;
        public Button colorImg;
        private string playerID;

        public void Init(ShareableClient client)
        {
            Debug.Log("olah o name" + client.ClientColor.ToColor());
            this.namePlayer.text = client.Name;
            this.playerID = client.Ip;
            this.colorImg.image.color = client.ClientColor.ToColor();
        }

        public void Acusing()
        {
            UIManagerAndroid.Instance.ACP.text = namePlayer.text;
            UIManagerAndroid.Instance.PlayerIDAcused = playerID;
            Debug.Log("Acusei " + namePlayer.text + " que tinha o ID " + playerID + " a cor" + this.colorImg.image.color);
        }
    }
}
