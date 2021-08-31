using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.DebugApplication
{
    public class DebugConsole : MonoBehaviour
    {
        public static DebugConsole Instance;
        private Text logText;
        private Coroutine currentWait;
        private bool forceOn;
        public bool ForceOn
        {
            set
            {
                alwaysOFF = false;
                forceOn = value;
                if (forceOn){
                    logText.transform.parent.gameObject.SetActive(true);
                    StopAllCoroutines();
                }
                else
                    StartCoroutine(WaitForHide());
            }
            get { return forceOn; }
        }

        private bool alwaysOFF;

        protected void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
            {
                Destroy(this.gameObject);
                return;
            }
            DontDestroyOnLoad(this);
            logText = GetComponentInChildren<Text>();
            forceOn = false;
            alwaysOFF = false;
        }
        public void Log(string message)
        {
            if(!alwaysOFF)
                logText.transform.parent.gameObject.SetActive(true);
            logText.text += string.Format("[{0}]: {1}\n", DateTime.Now.ToString("HH:mm:ss"), message);
            if (!forceOn)
            {
                if (currentWait != null)
                {
                    StopCoroutine(currentWait);
                }
                currentWait = StartCoroutine(WaitForHide());
            }
        }
        private IEnumerator WaitForHide()
        {
            yield return new WaitForSeconds(10);
            logText.transform.parent.gameObject.SetActive(false);
            currentWait = null;
        }
        public void Hide(){
            alwaysOFF = true;
            logText.transform.parent.gameObject.SetActive(false);
        }
    }
}
