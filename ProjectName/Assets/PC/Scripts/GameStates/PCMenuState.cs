using System.Collections.Generic;
using Assets.Generic;
using Assets.Network;

namespace Assets.PC
{
    public class PCMenuState : IGameState
    {
        public void OnDataReceived(MessageBase message, int connectionID)
        {
            switch (message.type)
            {
                case MessageType.ClientReady:
                    //GameManagerPC.Instance.OnClientSetReady(connectionID);
                    Client clientUpdate = message.GetData<Client>();
                    clientUpdate.ConnectionID = (byte)connectionID;
                    GameManagerPC.Instance.OnClientUIUpdate(clientUpdate);

                    NetworkPC.Instance.OnClientReady(clientUpdate);
                    break;
                case MessageType.GameStart:
                    GameManagerPC.Instance.OnGameStart();
                    break;
                case MessageType.ClientUIUpdate:
                    Client clientUpdate2 = message.GetData<Client>();
                    clientUpdate2.ConnectionID = (byte)connectionID;
                    GameManagerPC.Instance.OnClientUIUpdate(clientUpdate2);
                    break;
            }
        }
    }
}
