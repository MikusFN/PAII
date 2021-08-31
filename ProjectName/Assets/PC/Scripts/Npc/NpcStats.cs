using Assets.PC.WeatherSystem;
using System;
using System.Collections.Generic;
using Assets.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.PC.Npcs
{
    public class NpcStats : MonoBehaviour
    {
        public int life, sexDrive, sickMeter, happinessMeter, diseaseCount;
        private GenderNpc gender;
        private List<Tuple<WeatherSeasons, Card, int>> diseases;
        //private Dictionary<Card, int> cardsPerPlayer;
        public NpcState state;
        private Vector3 currentPosition;
        public int index;
        private bool isHazardous;
        private bool isInfectable;
        private float time, infectRadius = 1, contagiousRate = 0, mortalityRate=0, taxaContagio=65;

        public bool isVacinated = false;
        public bool isVacinatedAdvanced = false;

        public int Life
        {
            get => life; set => life = value;
        }
        public int SexDrive
        {
            get
            {
                return sexDrive;
            }
        }

        public void AddCard(Card card, int playerID)
        {
            if (!HasCard(card))
            {
                diseases.Add (new Tuple<WeatherSeasons, Card, int>(WeatherManager.weatherManager.season,card, playerID));
                infectRadius = card.initialContagiousRadius;
                if (card.contagiousRate > contagiousRate)
                {
                    contagiousRate = card.contagiousRate;
                }
                if (card.mortalityRate > mortalityRate)
                {
                    mortalityRate = card.mortalityRate;
                }
                IsHazardous = true;
                CityStats.Instance.Update100();
            }
        }
        public void RemoveCard(Card card)
        {
            if (HasCard(card))
            {
                diseases.Remove(diseases.Find(x=>x.Item2==card));
                CityStats.Instance.Update100();
            }
        }
        public int CheckPlayer(Card card)
        {
            if (HasCard(card))
                return diseases.Find(x=>x.Item2==card).Item3;
            return -1;
        }
        public bool HasCard(Card card)
        {
            if (diseases.Find(x => x.Item2 == card) == null)
                return false;
            else
                return true;
        }

        // public Dictionary<DiseaseEnum, int> Diseases
        // {
        //     get
        //     {
        //         return diseasesPerPlayer;
        //     }
        // }
        public int SickMeter
        {
            get
            {
                return sickMeter;
            }
        }
        public GenderNpc Gender
        {
            get
            {
                return gender;
            }
        }
        public NpcState State
        { get => state; set => state = value; }
        public int HappinessMeter
        { get => happinessMeter; }
        public Vector3 CurrentPosition
        { get => currentPosition; }
        public int Index
        { get => index; set => index = value; }
        public bool IsHazardous
        { get => isHazardous; set => isHazardous = value; }
        public List<Tuple<WeatherSeasons, Card, int>> Diseases
        { get => diseases; set => diseases = value; }
        public bool IsInfectable { get => isInfectable; set => isInfectable = value; }
        public float InfectRadius { get => infectRadius;}
        public float ContagiousRate { get => contagiousRate; set => contagiousRate = value; }
        public float MortalityRate { get => mortalityRate; set => mortalityRate = value; }
        public float TaxaContagio { get => taxaContagio; set => taxaContagio = value; }

        private void OnEnable()
        {
            IsHazardous = false;
            time = 0;
            contagiousRate = 0;
            mortalityRate = 0;
            gender = (GenderNpc)Random.Range(0, 3);
            currentPosition = this.transform.position;
            sickMeter = Random.Range(0, 101);
            life = Random.Range(20, 101);
            sexDrive = Random.Range(0, 101);
            happinessMeter = Random.Range(20, 101);
            Diseases = new List<Tuple<WeatherSeasons, Card, int>>();
            //cardsPerPlayer = new Dictionary<Card, int>();
            state = NpcState.Wandering;
        }
        private void Update()
        {
            if (state != NpcState.Dead)
            {
                diseaseCount = Diseases.Count;
                currentPosition = this.transform.position;
                if (life > 0 && state != NpcState.InsideBuilding)
                {

                    if (sickMeter > 75)
                    {
                        TimeCounter(ref time);
                        state = NpcState.GoingToHospital;
                        IsInfectable = true;
                        if (time >= 5)
                        {
                            life -= 1 + (int)mortalityRate/10;
                        }
                    }
                    else if (sickMeter > 50)
                    {
                        TimeCounter(ref time);
                        IsInfectable = true;
                        if (time >= 5)
                        {
                            life -= 1 + (int)mortalityRate / 10;
                        }
                    }
                    else if (sexDrive > 75)
                    {
                        state = NpcState.Horny;
                    }
                    else
                    {
                        state = NpcState.Wandering;
                    }
                }
                else if (life <= 0)
                {
                    state = NpcState.Dead;
                }
            }
            foreach (var tuple in diseases)
            {
                tuple.Item2.npcEvents.Invoke(this.gameObject);
            }            
        }

        private void TimeCounter(ref float time)
        {
            if (time > 5.0f)
            {
                time = 0;
            }
            else
            {
                time += Time.deltaTime;
            }
        }
    }
}
