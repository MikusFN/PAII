using Assets.Generic;
using Assets.Network;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Android
{

    public class AndroidInGameState : IGameState
    {
        public void OnDataReceived(MessageBase message, int connectionID)
        {
            switch (message.type)
            {
                case MessageType.DrawCards:
                    DrawCards(message);
                    break;
                case MessageType.TimesUp:
                    bool haswon = message.GetData<bool>();
                    DebugApplication.DebugConsole.Instance.Log("Time's up!");
                    break;
                case MessageType.DrawSpecificCard:
                    DrawSpecific(message);
                    break;
                case MessageType.SetMoney:
                    NetworkAndroid.Instance.CurrentClient.Money = message.GetData<float>();
                    UIManagerAndroid.Instance.UpdatePlayerStats();
                    break;
                case MessageType.SetReputation:
                    NetworkAndroid.Instance.CurrentClient.Reputation = message.GetData<ushort>();
                    UIManagerAndroid.Instance.UpdatePlayerStats();
                    break;
                case MessageType.SetMentalDisease:
                    SetDisease(message.GetData<PerceptionEnum>());
                    break;
                case MessageType.SetDiseases:
                    var diseases = message.GetData<List<ShareableDisease>>();
                    NetworkAndroid.Instance.RecieveSharebleDisease(diseases);
                    break;
                case MessageType.AskForVote:
                    DiseasePlayerAcusationWarper acusation = message.GetData<DiseasePlayerAcusationWarper>();
                    string nameSuspect="", nameDisease="";
                    foreach (ShareableClient c in NetworkAndroid.Instance.OtherClients)
                    {
                        if (c.Ip.Equals(acusation.playerID)){
                            nameSuspect = c.Name;
                        }                     
                    }
                    foreach (ShareableDisease d in NetworkAndroid.Instance.AllDiseases)
                    {
                        if (d.diseaseCardID.Equals(acusation.diseaseId)){
                            nameDisease = (AssetManager.Instance.GetObject(d.diseaseCardID) as Card).name;
                        }
                    }
                    if(!nameDisease.Equals("") && !nameSuspect.Equals(""))
                    UIManagerAndroid.Instance.FillAndShowVote(nameSuspect, nameDisease);
                    break;
                default:
                    break;
            }
        }


        private static void SetDisease(PerceptionEnum disease){
            var obj = (AndroidAssetManager.Instance as AndroidAssetManager).GetDisease(disease);
            GameObject.Instantiate(obj);
        }

        private static void DrawCards(MessageBase message)
        {
            var cardIDs = message.GetData<string[]>();

            foreach (string i in cardIDs)
            {
                Card c = AssetManager.Instance.GetObject(i) as Card;
                HandController.Instance.AddCard(c);
            }
        }

        private static void DrawSpecific(MessageBase message)
        {
            Debug.Log("I did draw it");
            var cardID = message.GetData<string>();

            Card c = AssetManager.Instance.GetObject(cardID) as Card;
            HandController.Instance.AddCard(c);
        }
    }
}
