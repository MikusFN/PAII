using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.PC.Npcs
{
    public class ComponentListing : MonoBehaviour
    {
        public static ComponentListing componentListing;

        private NpcStats[] populationStats;
        private const int RADIUS = 5;
        private int i;
        private List<NpcStats> npcStats;
        private const int weakThreshold = 10;

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
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {                
                Vector3 hitPoint = Vector3.zero;

                Plane m_Plane = new Plane(Vector3.up, Vector3.zero);
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                float enter;
                if (m_Plane.Raycast(ray, out enter))
                {
                    hitPoint = ray.GetPoint(enter);
                }

                List<NpcStats> npcStats = GetNeighbours(hitPoint, 10);
                foreach (var item in npcStats)
                {
                    if (item.State != NpcState.Dead)
                    {
                        item.State = NpcState.Dead;
                        item.Life = 0;
                        Debug.Log("KILLED A MODAFUCKA");
                    }
                }                
            }

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
                if (item.IsInfectable)
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
    }
}
