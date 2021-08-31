using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.PC
{
    public class Dissolve : MonoBehaviour
    {
        private CardBehaviour parent;

        public int textureSize = 256;
        public float alphaScale = 1;
        private RenderTexture alphaBlendRenderTexture;
        private Texture2D alphaBlendTexture;
        private Material currentMaterial;
        private Action blendAction;

        [Range(0, 2)]
        public float mergeSpeed;
        private float clipValue = 0;

        //GPU:
        private ComputeShader computeShader;
        private int generateTextureIndex;

        public void Init(CardBehaviour parent)
        {
            this.parent = parent;
            currentMaterial = GetComponent<Renderer>().material;
            alphaBlendRenderTexture = new RenderTexture(textureSize, textureSize, 0, RenderTextureFormat.Default);
            alphaBlendRenderTexture.enableRandomWrite = true;
            alphaBlendRenderTexture.Create();
            alphaBlendTexture = new Texture2D(textureSize, textureSize);

            blendAction += BlendAdd;
            GenerateTexture();
        }
        private void Update()
        {
            blendAction?.Invoke();
        }

        private void BlendAdd()
        {
            if (clipValue >= 1)
            {
                blendAction -= BlendAdd;
                parent.FinishEffect();
                return;
            }
            clipValue += mergeSpeed * Time.deltaTime;
            currentMaterial.SetFloat("_MergeClipValue", clipValue);
        }

        private void GenerateTexture()
        {
            float offsetX = UnityEngine.Random.Range(-alphaScale * 100, 100 * alphaScale);
            float offsetY = UnityEngine.Random.Range(-alphaScale * 100, 100 * alphaScale);
            RenderTexture.active = alphaBlendRenderTexture;
            alphaBlendTexture.ReadPixels(new Rect(0, 0, alphaBlendRenderTexture.width, alphaBlendRenderTexture.height), 0, 0);
            for (float y = 0; y < alphaBlendRenderTexture.height; y++)
                for (float x = 0; x < alphaBlendRenderTexture.width; x++)
                {
                    float xCoord = (x + offsetX) / (float)alphaBlendRenderTexture.width * alphaScale;
                    float yCoord = (y + offsetY) / (float)alphaBlendRenderTexture.height * alphaScale;
                    float sample = Mathf.Max(0.01f, Mathf.PerlinNoise(xCoord, yCoord) * 0.5f + Mathf.PerlinNoise(xCoord * 0.5f, yCoord * 0.5f) * 0.5f);
                    //sample = 0.9f;
                    alphaBlendTexture.SetPixel((int)x, (int)y, new Color(sample, sample, sample));
                }
            alphaBlendTexture.Apply();
            RenderTexture.active = null;

            currentMaterial.SetTexture("_AlphaText", alphaBlendTexture);
        }

        // private void SetupTextureOnGPU()
        // {
        //     generateTextureIndex = computeShader.FindKernel("CSGenerateTexture");
        //     computeShader.SetTexture(generateTextureIndex, "Result", alphaBlendTexture);

        // }
        // private void GenerateTextureOnGPU()
        // {
        //     computeShader.Dispatch(generateTextureIndex, textureSize / 8, textureSize / 8, 1);
        // }
    }
}
