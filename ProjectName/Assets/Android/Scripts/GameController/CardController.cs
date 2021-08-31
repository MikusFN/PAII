using System.Collections;
using System.Collections.Generic;
using Assets.Generic;
using UnityEngine;

namespace Assets.Android
{
    /// <summary>
    /// Hold a reference to the card, and controlls all its movements in game.
    /// </summary>
    public class CardController : MonoBehaviour
    {
        public Card currentCard { get; private set; }
        private bool dragging = false;
        private float distance;
        private bool colisionWithgame = false;
        private Vector3 point = new Vector3(-99, 0, 21);
        public bool playedAnimation = false;

        #region Input Control
        private void OnMouseOver() //shader para ampliar
        {

        }
        private void OnMouseEnter() //Aqui podemos por um shader nas bordas para brilhar e talvez ampliar a imagem
        {

        }
        private void OnMouseExit() //Aqui retirmaos o shader e pomos o normal da cena
        {

        }
        private void OnMouseDown()
        {
            UIManagerAndroid.Instance.ShowCardInfo(currentCard);

            distance = Vector3.Distance(transform.position, Camera.main.transform.position);
            dragging = true;
            HandController.Instance.RemoveCard(this);
            NetworkAndroid.Instance.SendMessage(new Network.MessageBase(null, Network.MessageType.PlayerHoldsCard), NetworkAndroid.Instance.ServerConnectionID, NetworkAndroid.reliableChannelID);
        }
        private void OnMouseUp()
        {
            UIManagerAndroid.Instance.HideCardInfo();

            dragging = false;

            Debug.Log("Disto to point" + Vector3.Distance(this.gameObject.transform.position, point));
            if (Vector3.Distance(this.gameObject.transform.position, HandController.Instance.cardPlayPoint.position) < 45 && NetworkAndroid.Instance.CurrentClient.Money >= this.currentCard.cost &&
                (Bipolar.Instance != null ? (Bipolar.Instance.CanPlay(currentCard)) : true) ) //&& Vector3.Distance(this.gameObject.transform.position, point) < 60)
            {
                Debug.Log("played");
                HandController.Instance.PlayCard(this);

            }
            else
            {
                HandController.Instance.AddCard(this);
                NetworkAndroid.Instance.SendMessage(new Network.MessageBase(null, Network.MessageType.PlayerDroppedCard), NetworkAndroid.Instance.ServerConnectionID, NetworkAndroid.reliableChannelID);
            }
        }
        #endregion

        /// <summary>
        /// This stars the card controller.
        /// </summary>
        /// <param name="card">The card to be created.</param>
        public void Init(Card card)
        {
            currentCard = card;
            this.gameObject.GetComponent<Renderer>().material.SetTexture("_MainTex", currentCard.texture.texture);
            //MUDAR TEXTURA DA CARTA.
        }

        //Unity Overrides:
        void Update()
        {

            if (dragging)
            {
                // this.transform.rotation = Quaternion.identity;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                Vector3 rayPoint = ray.GetPoint(distance);
                //transform.position = rayPoint;
                transform.position = new Vector3(rayPoint.x, this.transform.position.y, rayPoint.z);

            }
            if (playedAnimation == true)
            {
                this.GetComponent<Animator>().enabled = false;
                HandController.Instance.DisplayInFan();
                playedAnimation = false;
            }
        }
        public void stopAnimator()
        {
            playedAnimation = true;
        }
    }
}
