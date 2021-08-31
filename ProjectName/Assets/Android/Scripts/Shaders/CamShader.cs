
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//namespace Assets.Android { 
//    public class CamShader : MonoBehaviour
//    {

//        public Material mat;
//        public int times;

//        Camera AttachedCamera;
//        public Shader Post_Outline;

//        private void Awake()
//        {
//            AttachedCamera = GetComponent<Camera>();
//        }

//        private void OnRenderImage(RenderTexture source, RenderTexture destination)
//        {
//            if (mat != null)
//            {
//                renderDaltonic(source, destination);
//            }
//            if (mat != null)
//            {
//                renderCardsShaderLine(source, destination);
//            }

//            Graphics.Blit(source, destination);
//        }

//        public void renderDaltonic(RenderTexture source, RenderTexture destination)
//        {
//            RenderTexture tmp = RenderTexture.GetTemporary(source.width, source.height);
//            for (int i = 0; i < times; i++)
//            {
//                Graphics.Blit(source, tmp, mat);
//                Graphics.Blit(tmp, source, mat);
//            }
//        }
//        public void renderCardsShaderLine(RenderTexture source, RenderTexture destination)
//        {

//        }

//    }
//}

