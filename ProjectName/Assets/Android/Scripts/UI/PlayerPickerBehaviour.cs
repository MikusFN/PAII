using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Network;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Android
{
    public class PlayerPickerBehaviour : MonoBehaviour, IPointerDownHandler
    {
        public Image color;
        public Text name;
        private ShareableClient client;
        private Action<ShareableClient> callback;
        public void Init(ShareableClient player, Action<ShareableClient> callback)
        {
            this.client = player;
            this.callback = callback;

            color.color = player.ClientColor.ToColor();
            name.text = player.Name;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            callback(client);
        }
    }
}
