using System.Net;
using System.Net.Sockets;
using Assets.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Types;

namespace Assets.Network
{
    /// <summary>
    /// Network base class for both the client and the server implemention. DO NOT USE!!!
    /// 
    /// USE:
    ///     - AskFor - to call a function on the other device and asking for a returned callback.
    ///     - Set    - to change a value on the other device.
    ///     - On     - to respond to a message.
    /// 
    /// </summary>
    public abstract class NetworkBase<T> : SingletonBehaviour<T> where T : class
    {
        public int port;
        public int maxConnections;

        public static int reliableChannelID { get; private set; }
        public static int unreliableChannelID { get; private set; }
        protected static int socketId;
        protected static HostTopology topology;
        protected IGameState currentGameState;

        //AutoConnection:
        protected UdpClient udpClient;
        protected const string ProjectString = "TESTE-PROJECTO-02";

        //Unity Overloads:
        protected override void Awake()
        {
            base.Awake();
            NetworkTransport.Init();

            ConnectionConfig config = new ConnectionConfig();
            reliableChannelID = config.AddChannel(QosType.ReliableFragmented);
            unreliableChannelID = config.AddChannel(QosType.Unreliable);

            topology = new HostTopology(config, maxConnections);

            SetGameState(GameState.MenuState);
        }
        protected virtual void Update()
        {
            ListenMessage();
        }

        //Main Functions:
        /// <summary>
        /// Sends a messagebase object to a connected device.
        /// </summary>
        /// <param name="message">The message to be sent.</param>
        /// <param name="connectionId">The id of the connected device.</param>
        /// <param name="channelID">The id of the channel to transmit through. </param>
        public void SendMessage(MessageBase message, int connectionID, int channelID)
        {
            byte error;
            byte[] buffer = message.ToByteArray();
            NetworkTransport.Send(socketId, connectionID, channelID, buffer, buffer.Length, out error);
        }
        protected void StartSocket(bool isServer)
        {
            if (isServer)
                socketId = NetworkTransport.AddHost(topology, port, null);
            else
                socketId = NetworkTransport.AddHost(topology);
        }
        protected void ListenMessage()
        {
            int recHostId;
            int recConnectionId;
            int recChannelId;
            byte[] recBuffer = new byte[2048];
            int bufferSize = 2048;
            int dataSize;
            byte error;
            NetworkEventType recNetworkEvent = NetworkTransport.Receive(out recHostId, out recConnectionId, out recChannelId, recBuffer, bufferSize, out dataSize, out error);

            switch (recNetworkEvent)
            {
                case NetworkEventType.Nothing:
                    break;
                case NetworkEventType.ConnectEvent:
                    OnConnect(recConnectionId, recChannelId);
                    break;
                case NetworkEventType.DataEvent:
                    MessageBase message = MessageBase.FromByteArray(recBuffer);
                    OnDataReceived(message, recConnectionId);
                    break;
                case NetworkEventType.DisconnectEvent:
                    OnDisconnect(recConnectionId);
                    break;
            }
        }
        protected IPAddress GetIPAddressFromConnection(int connectionID){
            int port;
            string address;
            NetworkID network;
            NodeID dstNode;
            byte error;

            NetworkTransport.GetConnectionInfo(socketId, connectionID, out address, out port, out network, out dstNode, out error);
            address = address.TrimStart(':', 'f');
            return IPAddress.Parse(address);
        }

        //Abstracts:
        protected abstract void OnConnect(int connectionID, int channelID);
        protected abstract void OnDisconnect(int connectionID);
        protected abstract void OnDataReceived(MessageBase message, int connectionID);
        /// <summary>
        /// Changes the GameState of this device, and adjusts what messages it is listening to.
        /// </summary>
        /// <param name="newState">The enumerator that represent the state to change to.</param>
        public abstract void SetGameState(GameState newState);
    }
}
