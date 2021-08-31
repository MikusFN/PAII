using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Generic
{
    [CreateAssetMenu(menuName = "Card")]
    public class Card : NetworkObject
    {
        public string name;
        public Sprite texture;
        public string description;
        public string textForTheNews;
        public float cost;
        public short baseReputationImpact;
        public float moneyDeltaImpact;
        public short reputationDeltaImpact;


        public CardEvent serverEvents;
        public CardEvent clientEvents;
        public CardEvent bothEvents;
        public NpcEvent npcEvents;

        
        public CardType cardType;

        /**
         * disease
         */
        public float incubationTime;
        public float mortalityRate;
        public float contagiousRate;
        public float initialContagiousRadius;
        public VectorsOfTransmission vectorsOfTransmission;
        public TypeOfAgent typeOfAgent;
        public bool AdvancedVaccination;
        public bool BasicVaccination;



        /**
         * healthCarePolicy
         */
        public HealthCarePolicyType healthCarePolicyType;
        public DiseaseStrandType strandType;
        public float effectRange;

        /**
         * fun cards
         */
         

        /**
         * mentalDisease
         */
         public PerceptionEnum mentalDiseaseType;
    }

    public enum CardType : byte
    {
        disease,
        mentalDisease,
        healthCarePolicy,
        funCards
    }

    public enum HealthCarePolicyType : byte
    {
        vaccination,
        antibiotics,
        infrastructureImprovement
    }
    public enum DiseaseStrandType : byte{
        none,
        grainA,
        grainB,
    }

    [Flags]
    public enum VectorsOfTransmission
    {
        none = 0,
        rodents = 1,
        water = 2,
        organicFluids = 4,
        sexual = 8,
        lowHigiene = 16
    }

    public enum TypeOfAgent
    {
        viurs,
        bacteriumPlus,
        bacteriumNegative,
        fungi,
        parasites
    }
}