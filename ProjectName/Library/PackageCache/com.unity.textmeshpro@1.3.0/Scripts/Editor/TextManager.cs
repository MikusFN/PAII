using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Assets.PC.NewsFeed
{
    public class TextManager : MonoBehaviour
    {
        public TextMeshProUGUI textMesh;
        public float scrollingSpeed = 200;

        private TextMeshProUGUI childTextMesh;
        private RectTransform textMeshRect;
        private string sourceText;
        private string tempText;

        private void Awake()
        {
            textMeshRect = textMesh.GetComponent<RectTransform>();

            //childTextMesh = Instantiate(textMesh) as TextMeshProUGUI;
            //RectTransform childTextMeshRect = childTextMesh.GetComponent<RectTransform>();
            //childTextMeshRect.SetParent(textMeshRect);
            //childTextMeshRect.anchorMin = new Vector2(1, 0.5f);
            //childTextMeshRect.localScale = Vector3.one;
        }

        IEnumerator Start()
        {
            float width = textMesh.preferredWidth;
            Vector3 startPos = textMeshRect.position;

            float scrollPos = 0.0f;

            while (true)
            {
                if (textMesh.havePropertiesChanged)
                {
                    width = textMesh.preferredWidth;
                    childTextMesh.text = textMesh.text;
                }

                textMeshRect.position = new Vector3(-scrollPos % width, startPos.y, startPos.z);
                scrollPos += scrollingSpeed * Time.deltaTime;

                yield return null;
            }
        }
    }
}
