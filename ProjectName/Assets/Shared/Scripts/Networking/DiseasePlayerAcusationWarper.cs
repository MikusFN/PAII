using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Network
{
    [Serializable]
    public class DiseasePlayerAcusationWarper 
    {
        public string playerID;
        public string diseaseId;

        public DiseasePlayerAcusationWarper(string pID, string cID)
        {
            this.playerID = pID;
            this.diseaseId = cID;
        }

    }

}
