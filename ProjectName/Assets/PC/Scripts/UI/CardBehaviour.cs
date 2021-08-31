using System.Collections;
using System.Collections.Generic;
using Assets.Generic;
using UnityEngine;

namespace Assets.PC
{
    public class CardBehaviour : MonoBehaviour
    {
        private Animator currentAnimator;
        private PlayerCardAreaBehaviour parent;
        private Dissolve dissolveEffect;
        private Material currentMaterial;

        public void Init(PlayerCardAreaBehaviour parent)
        {
            currentAnimator = GetComponent<Animator>();
            this.parent = parent;
            dissolveEffect = GetComponentInChildren<Dissolve>();
            currentMaterial = GetComponentInChildren<Renderer>().material;
            currentMaterial.mainTexture = AssetManager.Instance.cardTexture;
        }

        public void PlayCard(Card card)
        {
            currentAnimator.SetTrigger("play");
            //currentMaterial.mainTexture = card.texture.texture;
        }

        public void DropCard()
        {
            currentAnimator.SetTrigger("close");
        }

        public void FinishedPlay()
        {
            dissolveEffect.Init(this);
            dissolveEffect.enabled = true;
        }

        public void FinishEffect()
        {
            parent.RemoveCurrentCard();
        }
        public void FinishedClose()
        {
            parent.RemoveCurrentCard();
        }
    }
}
