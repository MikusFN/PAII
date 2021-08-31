using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Network
{
    [Serializable]
    public class DiseaseCardWrapper
    {
        public string cardID;
        public  float xPosition;
        public float zPosition;

        public DiseaseCardWrapper(string cardID, Vector2 pos){
            this.cardID = cardID;
            xPosition = pos.x;
            zPosition = pos.y;
        }
        public Vector3 GetPos(){
            return new Vector3(xPosition, 0, zPosition);
        }
    }
}

