using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Generic;

namespace Assets.Android
{

    [System.Serializable]
    public class PerceptionWrapper{
        public PerceptionEnum type;
        public GameObject prefab;
    }
    public class AndroidAssetManager : AssetManager
    {
        public List<PerceptionWrapper> allPercepetionDisease;


        public GameObject GetDisease(PerceptionEnum key)
        {
            foreach (var wrapper in allPercepetionDisease)
            {
                if(wrapper.type == key)
                    return wrapper.prefab;
            }
            return null;
        }

    }
}
