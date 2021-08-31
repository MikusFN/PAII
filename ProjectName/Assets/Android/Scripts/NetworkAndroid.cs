using Assets.Generic;
using Assets.Network;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Assets.Android
{
    public class NetworkAndroid : NetworkBase<NetworkAndroid>
    {
        public List<ShareableClient> OtherClients { get; private set; }
        public List<ShareableDisease> AllDiseases { get; private set; }
        public Client CurrentClient { get; private set; }
        public int ServerConnectionID { get; private set; }
        public bool isConnected {get; private set; }

        protected override void Awake()
        {
            CurrentClient = new Client(0);
            OtherClients = new List<ShareableClient>();
            AllDiseases = new List<ShareableDisease>();
            isConnected = false;
            base.Awake();
        }
        private void Start()
        {
            StartSocket(false);
            AutoConnect();
        }

        public void AutoConnect()
        {
            if(udpClient == null)
                udpClient = new UdpClient(port + 1);
            udpClient.BeginReceive(ReceiveBroadcast, null);
            DebugApplication.DebugConsole.Instance.Log("Listening For messages...");
        }
        public void Connect(IPAddress ipAddress)
        {
            byte error;
            CurrentClient = new Client(NetworkTransport.Connect(socketId, ipAddress.ToString(), port, 0, out error));
            DebugApplication.DebugConsole.Instance.Log(string.Format("IP: {0}:{1} ID: {2} Error: {3}", ipAddress.ToString(), port, CurrentClient.ConnectionID, ((NetworkError)error).ToString()));
        }
        public void Disconnect()
        {
            byte error;
            NetworkTransport.Disconnect(socketId, ServerConnectionID, out error);
            DebugApplication.DebugConsole.Instance.Log(string.Format("Disconnected | Error: {0}", ((NetworkError)error).ToString()));
        }
        public void Reset()
        {
            AllDiseases.Clear();
            CurrentClient.Reset();
            HandController.Instance.Reset();
            SetGameState(GameState.MenuState);
            DebugApplication.DebugConsole.Instance.Log("Reseting Game.");
            UIManagerAndroid.Instance.SetCurrentScene(AndroidScene.connect);
        }

        private void ReceiveBroadcast(IAsyncResult ar)
        {
            IPEndPoint ip = new IPEndPoint(IPAddress.Any, port + 1);
            byte[] bytes = udpClient.EndReceive(ar, ref ip);
            string message = Encoding.ASCII.GetString(bytes);
            DebugApplication.DebugConsole.Instance.Log("Broadcast Recieved: " + message);

            if (message.Equals(ProjectString))
            {
                Connect(ip.Address);
            }
            else
            {
                udpClient.BeginReceive(ReceiveBroadcast, null);
            }
        }

        public void RecieveOtherClients(List<ShareableClient> others)
        {
            if (OtherClients.Count != 0)
            {
                OtherClients.AddRange(others);
            }
            else
                OtherClients = others;
        }
        public void RemoveFromOtherClients(List<ShareableClient> other)
        {
            for(int i = 0; i < OtherClients.Count; i ++){
                if(OtherClients[i].Ip == other[0].Ip){
                    OtherClients.RemoveAt(i);
                    return;
                }
            }
        }
        public void RecieveSharebleDisease(List<ShareableDisease> diseases){
            if (AllDiseases.Count != 0)
            {
                AllDiseases.AddRange(diseases);
            }
            else
                AllDiseases = diseases;
        }
        public void RemoveShareableDisease(List<ShareableDisease> disease){
            for(int i = 0; i < AllDiseases.Count; i ++){
                if(AllDiseases[i].diseaseCardID == disease[0].diseaseCardID){
                    AllDiseases.RemoveAt(i);
                    return;
                }
            }
        }

        //Overrides
        protected override void OnConnect(int connectionID, int channelID)
        {
            isConnected = true;
            UIManagerAndroid.Instance.ShowButtonReady();
            ServerConnectionID = connectionID;
            DebugApplication.DebugConsole.Instance.Log(string.Format("Something connected: {0}", connectionID));
        }
        protected override void OnDisconnect(int connectionID)
        {
            isConnected = false;
            ServerConnectionID = -1;
            Reset();
            OtherClients.Clear();
            DebugApplication.DebugConsole.Instance.Log(string.Format("Something disconnected: {0}", connectionID));
        }
        protected override void OnDataReceived(Network.MessageBase message, int connectionID)
        {
            DebugApplication.DebugConsole.Instance.Log(string.Format("IM: {1} : {0}", message.type.ToString(), connectionID));
            switch (message.type)
            {
                case MessageType.GameEnd:
                    Reset();
                    return;
                case MessageType.ClientJoined:
                    RecieveOtherClients(message.GetData<List<ShareableClient>>());
                    break;
                case MessageType.ClientLeft:
                    RemoveFromOtherClients(message.GetData<List<ShareableClient>>());
                    break;
            }
            currentGameState.OnDataReceived(message, connectionID);
        }

        public override void SetGameState(GameState newState)
        {
            CurrentClient.CurrentState = newState;
            switch (newState)
            {
                case GameState.MenuState:
                    currentGameState = new AndroidMenuState();
                    break;
                case GameState.InGameState:
                    currentGameState = new AndroidInGameState();
                    break;
                default:
                    DebugApplication.DebugConsole.Instance.Log("Error on changing GameState");
                    break;
            }
        }
    }
}

