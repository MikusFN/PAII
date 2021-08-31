
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Android { 
    public class CamShader : MonoBehaviour
    {

        private Material mat;
        public int times;

        private void Awake()
        {
            //mat = new Material(HandController.
        }

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
           
            if (mat != null)
            { 
            RenderTexture tmp = RenderTexture.GetTemporary(source.width, source.height);
            for (int i = 0; i < times; i++)
            {
                Graphics.Blit(source, tmp, mat);
                Graphics.Blit(tmp, source, mat);
            }
            Graphics.Blit(source, destination);
            }
        }

    }
}

