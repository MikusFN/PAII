using System.Collections.Generic;
using Assets.Generic;
using Assets.Network;

namespace Assets.Android
{
    public class AndroidMenuState : IGameState
    {
        public void SetReady()
        {
            NetworkAndroid.Instance.SendMessage(new MessageBase(null, MessageType.ClientReady), NetworkAndroid.Instance.ServerConnectionID, NetworkAndroid.reliableChannelID);
        }

        public void OnDataReceived(MessageBase message, int connectionID)
        {
            switch (message.type)
            {
                case MessageType.ClientSendData:
                    Client clone = message.GetData<Client>();
                    Reconnect(clone);
                    break;
                case MessageType.SetAsFirstPlayer:
                    UIManagerAndroid.Instance.ShowButtonStart();
                    NetworkAndroid.Instance.CurrentClient.IsFirstPlayer = true;
                    break;
                case MessageType.GameStarting:
                    Client data = message.GetData<Client>();
                    NetworkAndroid.Instance.CurrentClient.Money = data.Money;
                    NetworkAndroid.Instance.CurrentClient.Reputation = data.Reputation;

                    DebugApplication.DebugConsole.Instance.Log("GAME START!");
                    UIManagerAndroid.Instance.SetCurrentScene(AndroidScene.game);
                    UIManagerAndroid.Instance.OnGameStart();
                    NetworkAndroid.Instance.SetGameState(GameState.InGameState);
                    NetworkAndroid.Instance.SendMessage(new Network.MessageBase(((byte)0), MessageType.DrawCards), NetworkAndroid.Instance.ServerConnectionID, NetworkAndroid.reliableChannelID);
                    break;
               
                    //TODO: ADD MESSAGES HERE
            }
        }

        private static void Reconnect(Client clone)
        {
            if (clone.CurrentState != NetworkAndroid.Instance.CurrentClient.CurrentState)
            {
                NetworkAndroid.Instance.SetGameState(clone.CurrentState);
                if(clone.CurrentState == GameState.InGameState){
                    UIManagerAndroid.Instance.SetCurrentScene(AndroidScene.game);
                    UIManagerAndroid.Instance.OnGameStart();
                }
            }
            if (clone.Cards.Count > 0)
            {
                HandController.Instance.Reset();
                foreach (string id in clone.Cards)
                {
                    Card c = AssetManager.Instance.GetObject(id) as Card;
                    HandController.Instance.AddCard(c);
                }
            }

            NetworkAndroid.Instance.CurrentClient.Clone(clone);
        }
    }
}
