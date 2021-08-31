using Assets.Generic;
using Assets.Network;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Android
{
    public class DieaseNameIDPlayerTemplate : MonoBehaviour
    {
        public Text nameDisease;
        public Button colorImg;
        public string cardId;


        public void Init(ShareableDisease disease)
        {
         Card d = AssetManager.Instance.GetObject(disease.diseaseCardID) as Card;
         this.nameDisease.text = d.name;
         this.cardId = d.id;
         this.colorImg.image.color = disease.color.ToColor();                     
        }

        public void Acusing()
        {
            UIManagerAndroid.Instance.ACD.text = nameDisease.text;
            UIManagerAndroid.Instance.DeasesIDAcused = cardId;
            Debug.Log("Acusei da doença X " + nameDisease.text + " a cor" + this.colorImg.image.color);
        }
    }
}
