using System.Collections;
using System.Collections.Generic;
using Assets.Network;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Assets.Android
{
    public class ColorPickerBehaviour : MonoBehaviour, IPointerDownHandler
    {
        private Image currentImage;
        private Color color;

        public void Init(Color color)
        {
            currentImage = GetComponent<Image>();
            currentImage.color = color;
            this.color = color;
        }
        public void OnPointerDown(PointerEventData eventData)
        {
            UIManagerAndroid.Instance.SetPlayerColor(color);     
        }
    }
}
