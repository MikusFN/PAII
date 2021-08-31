using Assets.Generic;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Assets.Network;
using System;
using UnityEngine.UI;

namespace Assets.Android
{
    /// <summary>
    /// This class will controll all CardControllers in the client. It is responsible for managing each one and checking with the player currency if its playable.
    /// </summary>
    /// <typeparam name="HandController"></typeparam>
    public class HandController : SingletonBehaviour<HandController>
    {
        public CardController template;
        public float sideLimit;
        public float cardHeightTopLimit;
        public float cardHeightBottomLimit;
        public float cardHeightOffset;
        public float rotationValue;
        public float cardYCoordintate;
        private List<CardController> cardsList;
        float ypos = 0;

        public Transform cardPlayPoint;
        public Card currentCard;

        //public Camera camera;
        protected override void Awake()
        {
            base.Awake();
            cardsList = new List<CardController>();
            FindObjectOfType<Camera>();

        }

        //Public
        /// <summary>
        /// Adds card to the singleton. This function instanciates a new CardController Object.
        /// </summary>
        /// <param name="c">The card to be instanciated.</param>
        public void AddCard(Card c)
        {
            CardController controller = Instantiate(template, this.transform);
            controller.Init(c);
            AddCard(controller);
            controller.GetComponent<Animator>().SetTrigger("Draw");


        }
        /// <summary>
        /// Adds a card to the singleton. This function does not instanciate a new CardController.
        /// </summary>
        /// <param name="c">The controller to be added.</param>
        public void AddCard(CardController c)
        {
            cardsList.Add(c);
            DisplayInFan();
        }
        public CardController GetRandomCardFromHand()
        {
            if (cardsList.Count > 0)
            {
                var card = cardsList[UnityEngine.Random.Range(0, cardsList.Count - 1)];
                RemoveCard(card);
                return card;
            }
            return null;
        }

        /// <summary>
        /// This functions checks wheater if the card can be played, destroys it and manages player currency.
        /// </summary>
        /// <param name="c"></param>
        public void PlayCard(CardController c)
        {
            c.currentCard.clientEvents.Invoke(NetworkAndroid.Instance.CurrentClient.ConnectionID, c.currentCard);
            c.currentCard.bothEvents.Invoke(NetworkAndroid.Instance.CurrentClient.ConnectionID, c.currentCard);

            switch (c.currentCard.cardType)
            {
                case CardType.disease:
                    currentCard = c.currentCard;
                    UIManagerAndroid.Instance.ShowAreaPicker();
                    break;
                case CardType.mentalDisease:
                    currentCard = c.currentCard;
                    UIManagerAndroid.Instance.ShowMentalDiseasePlayerPicker();
                    break;
                case CardType.healthCarePolicy:
                    if(c.currentCard.healthCarePolicyType == HealthCarePolicyType.antibiotics || c.currentCard.healthCarePolicyType == HealthCarePolicyType.vaccination){
                        currentCard = c.currentCard;
                        UIManagerAndroid.Instance.ShowAreaPicker();
                    }
                    else{
                        AskForPlay(c.currentCard.id);
                    }
                    break;
                default:
                    AskForPlay(c.currentCard.id);
                    break;
            }
            Destroy(c.gameObject);

            Hysteria.Instance?.ResetTimer();

            //NetworkAndroid.Instance.CurrentClient.Money -= c.currentCard.cost;
            //NetworkAndroid.Instance.CurrentClient.Reputation -= (ushort)c.currentCard.baseReputationImpact;
            //UIManagerAndroid.Instance.UpdatePlayerStats();
        }
        /// <summary>
        /// This cards removes a cards from the list of managed cards.
        /// </summary>
        /// <param name="c"></param>
        public void RemoveCard(CardController c)
        {
            if (cardsList.Contains(c))
            {
                cardsList.Remove(c);
            }
            DisplayInFan();
        }
        /// <summary>
        /// Clears all data from the hand of the player.
        /// </summary>
        public void Reset()
        {
            foreach (CardController controler in cardsList)
            {
                Destroy(controler.gameObject);
            }
            cardsList = new List<CardController>();
        }

        //Mathematics:
        public void DisplayInFan()
        {
            List<Vector2> points = ReturnPoint(-155 + sideLimit, -50 - sideLimit, cardsList.Count);

            foreach (CardController g in cardsList)
            {
                g.transform.position = new Vector3(points[cardsList.IndexOf(g)].x, cardYCoordintate, points[cardsList.IndexOf(g)].y);
                // Debug.Log(points[cardsList.IndexOf(g)]);

                if (cardsList.IndexOf(g) > cardsList.Count / 2) g.transform.localRotation = Quaternion.Euler(new Vector3(90f, rotationValue, 0));
                else if (cardsList.IndexOf(g) < cardsList.Count / 2) g.transform.localRotation = Quaternion.Euler(new Vector3(90f, -rotationValue, 0));
                else g.transform.localRotation = Quaternion.Euler(new Vector3(90f, 0f, 0));
            }
        }
        private List<Vector2> ReturnPoint(float initX, float endX, float ncards)
        {
            List<Vector2> myPoints = new List<Vector2>();
            for (int i = 0; i < ncards; i++)
            {
                /*float interpolationValue;
                if (i == 0)
                    interpolationValue = 0;
                else if (i == (ncards - 1))
                    interpolationValue = 1;
                else
                    interpolationValue = i / (ncards - 1);

                float heightValue;
                if(i > (ncards - 1) / 2)
                {
                    heightValue = Mathf.Lerp(cardHeightTopLimit, cardHeightBottomLimit, ((i - ncards / 2) + 1) / (ncards / 2));
                }
                else
                {
                    heightValue = Mathf.Lerp(cardHeightBottomLimit, cardHeightTopLimit, i / (ncards / 2));
                }
                myPoints.Add(new Vector2(Mathf.Lerp(initX, endX, interpolationValue), heightValue - 80));*/

                float interpolationValue = 0;

                if (ncards == 1)
                {
                    interpolationValue = 0.5f;
                }
                else if (ncards == 3)
                {
                    switch (i)
                    {
                        case 0:
                            interpolationValue = 0.20f;
                            break;
                        case 1:
                            interpolationValue = 0.5f;
                            break;
                        case 2:
                            interpolationValue = 0.80f;
                            break;
                    }
                }
                else if (ncards == 2)
                {
                    switch (i)
                    {
                        case 0:
                            interpolationValue = 0.33f;
                            break;
                        case 1:
                            interpolationValue = 0.66f;
                            break;

                    }
                }
                else if (ncards > 3)
                {
                    if (i == 0)
                        interpolationValue = 0;
                    else if (i == (ncards - 1))
                        interpolationValue = 1;
                    else
                        interpolationValue = i / (ncards - 1);
                }


                Vector2 init = new Vector2(initX, cardHeightBottomLimit - cardHeightOffset);
                Vector2 end = new Vector2(endX, cardHeightBottomLimit - cardHeightOffset);
                Vector2 mid = new Vector2((initX + endX) / 2, cardHeightTopLimit - cardHeightOffset);

                myPoints.Add(Vector2.Lerp(Vector2.Lerp(init, mid, interpolationValue), Vector2.Lerp(mid, end, interpolationValue), interpolationValue));
            }
            return myPoints;
        }

        //NETWORK:
        private void AskForCards(byte numCardsToDraw)
        {
            NetworkAndroid.Instance.SendMessage(new MessageBase(numCardsToDraw, MessageType.DrawCards),
                NetworkAndroid.Instance.ServerConnectionID, NetworkAndroid.reliableChannelID);
        }
        private void AskForPlay(string cardID)
        {
            NetworkAndroid.Instance.SendMessage(new MessageBase(cardID, MessageType.PlayCard),
                NetworkAndroid.Instance.ServerConnectionID, NetworkAndroid.reliableChannelID);
        }
    }
}
