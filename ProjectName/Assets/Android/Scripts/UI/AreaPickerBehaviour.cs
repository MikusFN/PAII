using System.Collections;
using System.Collections.Generic;
using Assets.Network;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Android
{
    public class AreaPickerBehaviour : MonoBehaviour, IPointerDownHandler
    {
        public RectTransform pickerRect;

        public void OnPointerDown(PointerEventData eventData)
        {
            Vector2 position;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(pickerRect, eventData.position, Camera.main, out position);
            print("pos: " + position);
            position += pickerRect.rect.size / 2.000f;
            position /= pickerRect.rect.width;
            print("pos2: " + position);

            var dcw = new DiseaseCardWrapper(HandController.Instance.currentCard.id, position);
            NetworkAndroid.Instance.SendMessage(new MessageBase(dcw, MessageType.PlayDiseaseCard), NetworkAndroid.Instance.ServerConnectionID, NetworkAndroid.reliableChannelID);

            HandController.Instance.currentCard = null;

            this.gameObject.SetActive(false);
        }
    }
}

