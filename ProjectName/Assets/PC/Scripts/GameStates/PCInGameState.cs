using System.Collections;
using System.Collections.Generic;
using Assets.Generic;
using Assets.Network;
using Assets.PC.PCG;
using UnityEngine;

namespace Assets.PC
{
    public class PCInGameState : IGameState
    {
        public void OnDataReceived(MessageBase message, int connectionID)
        {
            switch (message.type)
            {
                case MessageType.DrawCards:
                    byte cardCount = message.GetData<byte>();
                    PlayerDrawCard(cardCount, connectionID);
                    break;
                case MessageType.PlayDiseaseCard:
                    var dcw = message.GetData<DiseaseCardWrapper>();

                    Vector3 infectPos = dcw.GetPos() * ProceduralCity.Instance.Size;
                    ServerFunctionManager.touchPos = infectPos;
                    
                    Card disease = AssetManager.Instance.GetObject(dcw.cardID) as Card;
                    PlayerPlayCard(disease, connectionID);
                    UIManagerPC.Instance.PlayerPlaysCard(connectionID, disease);
                    break;
                case MessageType.PlayMentalDiseaseCard:
                    var mdcw = message.GetData<MentalDiseaseCardWrapper>();
                    Card mentalDisease = AssetManager.Instance.GetObject(mdcw.cardID) as Card;
                    PlayerPlayCard(mentalDisease, connectionID);
                    UIManagerPC.Instance.PlayerPlaysCard(connectionID, mentalDisease);

                    GameManagerPC.Instance.AddMentalDisease(mdcw.playerIP, mentalDisease);
                    break;
                case MessageType.PlayCard:
                    string playCardID = message.GetData<string>();
                    Card card = AssetManager.Instance.GetObject(playCardID) as Card;
                    PlayerPlayCard(card, connectionID);
                    UIManagerPC.Instance.PlayerPlaysCard(connectionID, card);
                    break;
                case MessageType.PlayerHoldsCard:
                    UIManagerPC.Instance.PlayerHoldsCard(connectionID);
                    break;
                case MessageType.PlayerDroppedCard:
                    UIManagerPC.Instance.PlayerDropsCard(connectionID);
                    break;
                case MessageType.DrawSpecificCard:
                    string cardID = message.GetData<string>();
                    PlayerDrawCardSpecificCard(cardID, connectionID);
                    break;
                case MessageType.AcusingAPlayer:
                    DiseasePlayerAcusationWarper acusation = message.GetData<DiseasePlayerAcusationWarper>();

                    GameManagerPC.Instance.setGuilt(acusation.playerID, acusation.diseaseId);
                    SendAcusationToAll(acusation, connectionID);
                    break;
                case MessageType.DecsionVote:
                    decision(message.GetData<bool>(), connectionID);
                    break;
            }
        }

        private static void PlayerDrawCard(byte cardCount, int connectionID)
        {
            bool isStart = false;
            if (cardCount == 0)
            {
                isStart = true;
                cardCount = GameManagerPC.Instance.START_CARDS;
            }
            string[] cardIDs = new string[cardCount];
            for (int i = 0; i < cardCount; i++)
            {
                Card c = AssetManager.Instance.GetRandomCard();
                cardIDs[i] = c.id;
                GameManagerPC.Instance.AddCardToPlayer(connectionID, cardIDs[i]);
            }

            if (!isStart)
                GameManagerPC.Instance.IncrementPlayerMoney(connectionID, -GameManagerPC.Instance.DRAW_CARD_COST);
            NetworkPC.Instance.SendMessage(new MessageBase(cardIDs, MessageType.DrawCards), connectionID, NetworkPC.reliableChannelID);
        }

        private static void PlayerPlayCard(Card card, int connectionID)
        {
            DebugApplication.DebugConsole.Instance.Log(string.Format("{0} played card \"{1}\"!", connectionID, card.name));

            card.serverEvents.Invoke(connectionID, card);
            card.bothEvents.Invoke(connectionID, card);

            Client client = GameManagerPC.Instance.FindClientByID(connectionID);

            GameManagerPC.Instance.RemoveCardFromPlayer(connectionID, card.id);
            GameManagerPC.Instance.IncrementPlayerMoney(client, -card.cost);
            GameManagerPC.Instance.IncrementPlayerReputation(client, card.baseReputationImpact);  //TODO: MAKE THIS CLEAR (REPUTATION IS SHORT OR FLOAT?)
            client.MoneyDelta += card.moneyDeltaImpact;
            client.ReputationDelta += card.reputationDeltaImpact;

            UIManagerPC.Instance.UpdatePlayerResourcesUI(client);
        }

        private static void PlayerDrawCardSpecificCard(string cardId, int connectionID)
        {
            KeyValuePair<string, int> clinetReasechOnGoing = new KeyValuePair<string, int>(cardId, connectionID);
            GameManagerPC.Instance.AddCardToPlayer(connectionID, cardId);
            GameManagerPC.Instance.reasearchsOnGoing.Enqueue(clinetReasechOnGoing);

            //Isto vai para o DeEnqueue
            //NetworkPC.Instance.SendMessage(new MessageBase(cardId, MessageType.DrawSpecificCard), connectionID, NetworkPC.reliableChannelID);
        }

        private void SendAcusationToAll(DiseasePlayerAcusationWarper acusation, int connectionID)
        {
            Client clientAcuser = GameManagerPC.Instance.FindClientByID(connectionID);
            Client clientSuspect = GameManagerPC.Instance.FindClientByIP(acusation.playerID);

            GameManagerPC.Instance.clientAcuserT = clientAcuser;
            GameManagerPC.Instance.clientSuspectT = clientSuspect;

            var allConections = GameManagerPC.Instance.GetAllConnections(clientAcuser, clientSuspect);

            foreach (var i in allConections)
            {
                NetworkPC.Instance.SendMessage(new MessageBase(acusation, MessageType.AskForVote), i, NetworkPC.reliableChannelID);
            }
     
        }

        private void decision( bool judging, int playerID)
        {
            GameManagerPC.Instance.playersDecsion.Add(playerID, judging);

            var allConections = GameManagerPC.Instance.GetAllConnections(GameManagerPC.Instance.clientAcuserT, GameManagerPC.Instance.clientSuspectT);

            bool testeGuilt = GameManagerPC.Instance.isGuity;
            //is this the last player?
            if (GameManagerPC.Instance.playersDecsion.Count == allConections.Count)
            {
                //Send verdict
                foreach (var i in allConections)
                {
                    if (GameManagerPC.Instance.playersDecsion[i] == testeGuilt)
                        GameManagerPC.Instance.IncrementPlayerReputation(GameManagerPC.Instance.FindClientByID(i), -1000);

                    else
                        GameManagerPC.Instance.IncrementPlayerReputation(GameManagerPC.Instance.FindClientByID(i), 1000);

                }
                GameManagerPC.Instance.cleanPlayersDecision();
            }

        }

  
    }
}