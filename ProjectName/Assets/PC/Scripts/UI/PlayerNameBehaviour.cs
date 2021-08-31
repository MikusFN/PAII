using System.Collections;
using System.Collections.Generic;
using Assets.Generic;
using Assets.Network;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.PC
{
    public class PlayerNameBehaviour : MonoBehaviour
    {
        public Image backgroundImage;
        public Text nameText;
        public Text repText;
        public Text monText;

        public void SetNameAndColor(Client c){
            SetName(c.Name);
            SetColor(c.ClientColor.ToColor());
            SetReputation(c.Reputation);
            SetMoney(c.Money);
        }
        public void SetName(string name){
            nameText.text = name;
        }
        public void SetColor(Color color){
            backgroundImage.color = color;
        }


        public void SetReputation(ushort reputation){
            repText.text = "Rep: " + reputation.ToString();
        }
        public void SetMoney(float money){
            monText.text = money.ToString();
        }
    }
}
