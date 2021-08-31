using System.Collections;
using System.Collections.Generic;
using Assets.Generic;
using Assets.Network;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.PC
{
    public class DiseaseBehaviour : MonoBehaviour
    {
        public DiseaseData data;
        public Image colorImage;
        public Text nameText;
        public Text infectedCountText;
        public Text deadCountText;
        private Animator currentAnimator;

        public void Init(DiseaseData data){
            nameText.text = data.card.name;
            colorImage.color = data.color;
            this.data = data;
            UpdateValues(data);
            currentAnimator = GetComponent<Animator>();
        }

        public void SetSelected(bool value){
            currentAnimator.SetBool("selected", value);
        }

        public void UpdateValues(DiseaseData data){
            infectedCountText.text = $"{data.infectedCount}";
            deadCountText.text = $"{data.deadCount}";
        }
    }
}

