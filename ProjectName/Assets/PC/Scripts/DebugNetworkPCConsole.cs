using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.PC
{
    public class DebugNetworkPCConsole : MonoBehaviour
    {
        public Button closeButton;
        private bool isClosed;
        public Button forceTextButton;
        public Button resetButton;
        public Button purgeButton;
        public Button textOffButton;

        void Start()
        {
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
            resetButton.onClick.AddListener(() => {
                GameManagerPC.Instance.ResetGame();
            });
            purgeButton.onClick.AddListener(()=> {
                GameManagerPC.Instance.DisconnectAll();
            });
            textOffButton.onClick.AddListener(() => {
                DebugApplication.DebugConsole.Instance.Hide();
            });
        }
    }
}
