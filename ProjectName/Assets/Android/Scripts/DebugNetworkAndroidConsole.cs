using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Net;
using Assets.Network;

namespace Assets.Android
{
    public class DebugNetworkAndroidConsole : MonoBehaviour
    {
        public Button closeButton;
        private bool isClosed = false;
        public InputField ipInputField;
        public Button connectButton;
        public Button disconnectButton;
        public Button autoConnectButton;
        public Button readyButton;
        public Button unreadyButton;
        public Button drawCardButton;
        public Button playCardButton;
        public Button startButton;
        public Button forceTextButton;
        public Button turnOffText;

        private void Start()
        {
            connectButton.onClick.AddListener(() =>
            {
                IPAddress address;
                if (IPAddress.TryParse(ipInputField.text, out address))
                    NetworkAndroid.Instance.Connect(address);
            });
            disconnectButton.onClick.AddListener(() =>
            {
                NetworkAndroid.Instance.Disconnect();
            });
            autoConnectButton.onClick.AddListener(() =>
            {
                NetworkAndroid.Instance.AutoConnect();
            });
            readyButton.onClick.AddListener(() =>
            {
                NetworkAndroid.Instance.SendMessage(new MessageBase(null, MessageType.ClientReady), NetworkAndroid.Instance.ServerConnectionID, NetworkAndroid.reliableChannelID);
            });
            unreadyButton.onClick.AddListener(() =>
            {
                NetworkAndroid.Instance.SendMessage(new Network.MessageBase(null, MessageType.ClientUnready), NetworkAndroid.Instance.ServerConnectionID, NetworkAndroid.reliableChannelID);
            });
            drawCardButton.onClick.AddListener(() =>
            {
                NetworkAndroid.Instance.SendMessage(new Network.MessageBase(((byte)2), MessageType.DrawCards), NetworkAndroid.Instance.ServerConnectionID, NetworkAndroid.reliableChannelID);
            });
            playCardButton.onClick.AddListener(() =>
            {
                NetworkAndroid.Instance.SendMessage(new Network.MessageBase(null, MessageType.PlayCard), NetworkAndroid.Instance.ServerConnectionID, NetworkAndroid.reliableChannelID);
            });
            startButton.onClick.AddListener(() =>
            {
                NetworkAndroid.Instance.SendMessage(new Network.MessageBase(null, MessageType.GameStart), NetworkAndroid.Instance.ServerConnectionID, NetworkAndroid.reliableChannelID);
            });
            closeButton.onClick.AddListener(() =>
            {
                RectTransform t = this.transform as RectTransform;

                isClosed = !isClosed;
                if (isClosed)
                    t.localPosition += new Vector3(t.sizeDelta.x, 0, 0);
                else
                    t.localPosition += new Vector3(-t.sizeDelta.x, 0, 0);

            });
            forceTextButton.onClick.AddListener(() =>
            {
                DebugApplication.DebugConsole.Instance.ForceOn = !DebugApplication.DebugConsole.Instance.ForceOn;
            });
            turnOffText.onClick.AddListener(() => {
                DebugApplication.DebugConsole.Instance.ForceOn = false;
                DebugApplication.DebugConsole.Instance.Hide();
            });
        }
    }
}
