using System.Collections;
using System.Collections.Generic;
using Assets.Generic;
using UnityEngine;

namespace Assets.PC
{
    public class DiseaseData
    {
        public Card card;
        public string playerIP;
        public int infectedCount;
        public int deadCount;
        public List<Transform> infectedPeople;
        public Color color;

        public DiseaseData(Card card, string playerIP, int infected, List<Transform> infectedPeople, Color color){
            this.card = card;
            this.playerIP = playerIP;
            this.infectedCount = infected;
            this.infectedPeople = infectedPeople;
            deadCount = 0;
            this.color = color;
        }
    }
}
