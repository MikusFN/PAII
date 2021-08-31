using Assets.PC.Npcs;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.PC.NewsFeed
{
    public class NewsFeedTextManager : MonoBehaviour
    {
        public TextMeshProUGUI textMesh;
        public RectTransform imageRec;
        public RectTransform canvasRec;

        public float scrollingSpeed = 200;

        private TextMeshProUGUI childTextMesh;
        private RectTransform textMeshRect;
        private string sourceText;
        private string tempText;
        private float width;
        private Vector3 startPos;
        private float scrollPos;
        private TextMeshProUGUI[] messagesRunning;

        private void Start()
        {
            messagesRunning = new TextMeshProUGUI[10];

            textMeshRect = textMesh.GetComponent<RectTransform>();
            //Alignment 
            imageRec.position = new Vector3(imageRec.position.x
                , canvasRec.position.y
                //+canvasRec.rect.height/4
                , imageRec.position.z);
            textMeshRect.position = new Vector3(textMeshRect.position.x - canvasRec.position.x
                , imageRec.position.y - imageRec.rect.height / 4
                , textMeshRect.position.z);

            width = textMesh.preferredWidth + imageRec.rect.width;
            scrollPos = 0.0f;

            startPos = new Vector3(textMeshRect.position.x + width + canvasRec.position.x
                , textMeshRect.position.y
                , textMeshRect.position.z);

            for (int i = 1; i <= messagesRunning.Length; i++)
            {
                messagesRunning[i - 1] = Instantiate(textMesh);
                RectTransform childTextMeshRect = messagesRunning[i - 1].GetComponent<RectTransform>();


                childTextMeshRect.position = new Vector3((startPos.x) * i - width / 2, startPos.y, startPos.z);
                childTextMeshRect.anchorMin = textMeshRect.anchorMin;
                childTextMeshRect.localScale = Vector3.one;
            }
            foreach (var item in messagesRunning)
            {
                item.rectTransform.SetParent(textMeshRect);
            }
            //childTextMesh = Instantiate(textMesh) as TextMeshProUGUI;
            //RectTransform childTextMeshRect = childTextMesh.GetComponent<RectTransform>();
            //childTextMeshRect.SetParent(textMeshRect);                   

            //childTextMeshRect.position = new Vector3(startPos.x - width / 2
            //    , startPos.y
            //    , startPos.z);
            //childTextMeshRect.anchorMin = textMeshRect.anchorMin;
            //childTextMeshRect.localScale = Vector3.one;

        }

        void Update()
        {
            if (textMesh.havePropertiesChanged)
            {
                width = textMesh.preferredWidth + imageRec.rect.width;

                foreach (var item in messagesRunning)
                {
                    item.text = textMesh.text;

                }
            }
            //textMesh.text = $"{CityStats.Instance.Infected100}";

            //Debug.Log("here " + CityStats.Instance.Infected100);
            textMeshRect.position = new Vector3(-scrollPos % ((width * messagesRunning.Length * 1.55f)) + startPos.x, startPos.y, startPos.z);

            scrollPos += scrollingSpeed * Time.deltaTime;
        }
    }
}

