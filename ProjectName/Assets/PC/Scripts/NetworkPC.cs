using Assets.Generic;
using Assets.Network;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.PC
{
    public class NetworkPC : NetworkBase<NetworkPC>
    {
        private IPAddress broadcastAddress;
        private void Start()
        {
            StartSocket(true);
            broadcastAddress = GetBroadcastAddress();
            InvokeRepeating("BroadcastConnectMessage", 0, 1);
        }

        void OnDestroy()
        {
            udpClient?.Close();
        }
        public void DisconnectClient(Client c)
        {
            byte error;
            NetworkTransport.Disconnect(socketId, c.ConnectionID, out error);
        }

        //Auto Connect
        private void BroadcastConnectMessage()
        {
            udpClient = new UdpClient(port + 1);
            udpClient.EnableBroadcast = true;
            IPEndPoint ip = new IPEndPoint(broadcastAddress, port + 1);
            byte[] bytes = Encoding.ASCII.GetBytes(ProjectString);
            udpClient.Send(bytes, bytes.Length, ip);
            udpClient.Close();
        }
        private IPAddress GetBroadcastAddress()
        {
            IPAddress ip = null;
            IPAddress subnetMask = IPAddress.Parse("255.255.255.0");
            var host = Dns.GetHostEntry("");
            foreach (IPAddress address in host.AddressList)
            {
                if (address.AddressFamily == AddressFamily.InterNetwork)
                {
                    ip = address;
                }
            }

            byte[] ipAdressBytes = ip.GetAddressBytes();
            byte[] subnetMaskBytes = subnetMask.GetAddressBytes();

            if (ipAdressBytes.Length != subnetMaskBytes.Length)
                throw new ArgumentException("Lengths of IP address and subnet mask do not match.");

            byte[] broadcastAddress = new byte[ipAdressBytes.Length];
            for (int i = 0; i < broadcastAddress.Length; i++)
            {
                broadcastAddress[i] = (byte)(ipAdressBytes[i] | (subnetMaskBytes[i] ^ 255));
            }
            return new IPAddress(broadcastAddress);
        }

        //Overrides
        protected override void OnConnect(int connectionID, int channelID)
        {
            string ip = GetIPAddressFromConnection(connectionID).ToString();
            if (!(currentGameState is PCMenuState) && !GameManagerPC.Instance.DoesIPExist(ip))
            {
                byte error;
                NetworkTransport.Disconnect(socketId, connectionID, out error);
                return;
            }


            Client c = GameManagerPC.Instance.SetClientID(ip, connectionID);
            UIManagerPC.Instance.AddPlayerArea(c);

            if (GameManagerPC.Instance.gameStarted)
            {
                OnClientReady(c);
                var diseases = GameManagerPC.Instance.GetAllShareabledDiseases();
                if (diseases.Count > 0)
                    NetworkPC.Instance.SendMessage(new Assets.Network.MessageBase(diseases, MessageType.SetDiseases), c.ConnectionID, reliableChannelID);
            }
            else
            {
                var allConnection = GameManagerPC.Instance.GetAllConnections(c);
                if (allConnection.Count == 0)
                {
                    SendMessage(new Network.MessageBase(null, MessageType.SetAsFirstPlayer), connectionID, reliableChannelID);
                }
            }
        }

        public void OnClientReady(Client clone)
        {

            Client client = GameManagerPC.Instance.FindClientByID(clone.ConnectionID);
            client.IsReady = true;
            DebugApplication.DebugConsole.Instance.Log(string.Format("{0} is ready", client.ConnectionID));

            var allConnection = GameManagerPC.Instance.GetAllConnections(client);
            List<ShareableClient> allPresentClients = GameManagerPC.Instance.GetAllShareableClients(client);

            List<ShareableClient> currentClient = new List<ShareableClient> { new ShareableClient(client, GameManagerPC.Instance.FindIPByClient(client)) };

            foreach (var i in allConnection)
            {
                SendMessage(new Network.MessageBase(currentClient, MessageType.ClientJoined), i, reliableChannelID);
            }

            if (allPresentClients.Count > 0)
                SendMessage(new Network.MessageBase(allPresentClients, MessageType.ClientJoined), client.ConnectionID, reliableChannelID);
            else
                SendMessage(new Network.MessageBase(null, MessageType.SetAsFirstPlayer), client.ConnectionID, reliableChannelID);
        }
        protected override void OnDisconnect(int connectionID)
        {
            Client c = GameManagerPC.Instance.RemoveClientID(connectionID);
            if (c != null)
            {
                string ip = GameManagerPC.Instance.FindIPByClient(c);
                List<ShareableClient> currentClient = new List<ShareableClient> { new ShareableClient(c, ip) };

                var allConnection = GameManagerPC.Instance.GetAllConnections();
                foreach (var i in allConnection)
                {
                    SendMessage(new Network.MessageBase(currentClient, MessageType.ClientLeft), i, reliableChannelID);
                }

                UIManagerPC.Instance.RemovePlayerUI(c);

                if (c.IsFirstPlayer && allConnection.Count > 0)
                {
                    SendMessage(new Network.MessageBase(null, MessageType.SetAsFirstPlayer), allConnection.First(), reliableChannelID);
                }
            }
        }
        protected override void OnDataReceived(Network.MessageBase message, int connectionID)
        {
            DebugApplication.DebugConsole.Instance.Log(string.Format("IM: {1} : {0}", message.type.ToString(), connectionID));
            currentGameState.OnDataReceived(message, connectionID);
        }
        public override void SetGameState(GameState newState)
        {
            switch (newState)
            {
                case GameState.MenuState:
                    currentGameState = new PCMenuState();

                    break;
                case GameState.InGameState:
                    currentGameState = new PCInGameState();
                    break;
                default:
                    DebugApplication.DebugConsole.Instance.Log("Error on changing GameState");
                    break;
            }
        }
    }
}