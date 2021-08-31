using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Generic;
using System;
using Assets.PC.WeatherSystem;
using Random = UnityEngine.Random;

namespace Assets.PC.Npcs
{
    //public class ComponentListing : SingletonBehaviour<ComponentListing>
    public class NpcStatsManager : MonoBehaviour
    {
        public static NpcStatsManager componentListing;

        private NpcStats[] populationStats;
        private const int RADIUS = 5;
        private int i;
        private const int weakThreshold = 10;
        public float taxaContagio;
        private float prevTaxaContagio;

        public NpcStats[] PopulationStats { get => populationStats; set => populationStats = value; }

        void Start()
        {
            //Singleton
            if (componentListing == null)
            {
                componentListing = this;
            }
            else if (componentListing != this)
            {
                Destroy(gameObject);
            }
            DontDestroyOnLoad(gameObject);

            PopulationStats = GetComponentsInChildren<NpcStats>();
            i = 0;
            foreach (NpcStats item in PopulationStats)
            {
                item.Index = i;
                i++;
            }
            taxaContagio = 65;
            prevTaxaContagio = taxaContagio;
            //foreach (NpcStats item in PopulationStats)
            //{
            //    if (item.SickMeter + item.HappinessMeter < weakThreshold)
            //    {
            //        foreach (var some in item.Diseases)
            //        {
            //            Debug.Log(some);

            //        }
            //    }
            //}

            //List<NpcStats> npcStats = GetNeighbours(0, RADIUS);
            //foreach (var item in npcStats)
            //{
            //    item.Life = 0;
            //}
        }
        private void Update()
        {
            if (taxaContagio != prevTaxaContagio)
            {
                foreach (var item in populationStats)
                {
                    item.TaxaContagio = taxaContagio;
                }
            }
            //if (Input.GetKeyDown(KeyCode.Mouse0))
            //{
            //    Vector3 hitPoint = Vector3.zero;

            //    Plane m_Plane = new Plane(Vector3.up, Vector3.zero);
            //    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //    float enter;
            //    if (m_Plane.Raycast(ray, out enter))
            //    {
            //        hitPoint = ray.GetPoint(enter);
            //    }
            //    GetCurable(hitPoint, 5, new Card());
            //}
            //     List<NpcStats> npcStats = GetNeighbours(hitPoint, 10);
            //     foreach (var item in npcStats)
            //     {
            //         if (item.State != NpcState.Dead)
            //         {
            //             item.State = NpcState.Dead;
            //             item.Life = 0;
            //             Debug.Log("KILLED A MODAFUCKA");
            //         }
            //     }                
            // }
            prevTaxaContagio = taxaContagio;
        }

        public List<NpcStats> GetNeighbours(int index, int radius)
        {
            NpcStats target = PopulationStats[index];
            List<NpcStats> neighbours = new List<NpcStats>();

            foreach (NpcStats npc in PopulationStats)
            {
                if (Vector3.Distance(target.CurrentPosition, npc.CurrentPosition) < radius)
                {
                    neighbours.Add(npc);
                }
            }

            return neighbours;
        }
        public List<NpcStats> GetNeighbours(Vector3 position, float radius)
        {
            List<NpcStats> neighbours = new List<NpcStats>();
            foreach (NpcStats npc in PopulationStats)
            {
                if (Vector3.Distance(position, npc.transform.position) < radius)
                {
                    neighbours.Add(npc);
                }
            }
            return neighbours;
        }

        public int GetWeakling()
        {
            List<int> weaklingsIndex = new List<int>();
            int index = 0;
            foreach (var item in PopulationStats)
            {
                if (item.IsInfectable && (item.state == NpcState.Horny || item.state == NpcState.Wandering))
                {
                    weaklingsIndex.Add(index);
                    index++;
                }
            }
            if (weaklingsIndex.Count > 0)
            {
                return weaklingsIndex[Random.Range(0, weaklingsIndex.Count)];
            }
            else
            {
                return -1;
            }
        }
        public void GetCurable(Vector3 epicenter, float radius, Card curaCard)
        {
            List<NpcStats> curableNeighbours = GetNeighbours(epicenter, radius);

            foreach (NpcStats npc in curableNeighbours)
            {
                if (npc.Diseases.Count > 0)
                {
                    //npc.Diseases.FindAll(x => x.Item2.cura == curaCard.cura))
                    List<Tuple<WeatherSeasons, Card, int>> doencasCuraveis = npc.Diseases.FindAll(x => x.Item2 != null);
                    foreach (var item in doencasCuraveis)
                    {
                        //if(npc.Diseases.Contains(item))
                        if (item.Item2.strandType == curaCard.strandType)
                        {
                            npc.Diseases.Remove(item);
                            Debug.Log(npc.Index + " had all diseases removed by god");
                            NpcManager.RemoveDiseaseTrans(npc, doencasCuraveis);
                        }
                    }
                }
            }
            Debug.Log("in");
        }

        public void VacinateNpcs(Vector3 center, float range, Card card, bool isAdvanced){
             List<NpcStats> vacinatableNpcs = GetNeighbours(center, range);

            if(isAdvanced){
                foreach(var npc in vacinatableNpcs){
                    npc.isVacinatedAdvanced = true;
                }
            }
            else{
                foreach(var npc in vacinatableNpcs){
                    npc.isVacinated = true;
                }
            }
        }
    }
}
