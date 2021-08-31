using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Network
{
    [Serializable]
    public class ClientColor
    {
        public float r;
        public float g;
        public float b;

        public ClientColor(Color c){
            this.r = c.r;
            this.g = c.g;
            this.b = c.b;
        }
        public Color ToColor()
        {
            return new Color(r,g,b);
        }
    }
}

