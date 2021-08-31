using Assets.Generic;
using Assets.PC.PathFinding;
using Assets.PC.PCG;
using Assets.PC.WeatherSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace Assets.PC.Npcs
{
    public class NpcManager : MonoBehaviour
    {
        private Renderer rend;
        private Color color;
        private NpcStats stats;
        private const int MAXVALUE = 100;
        private NavMeshAgent agent;
        private GiveDestination giveDestination;
        private Tuple<CellType, Vector3> nextDest, lastDest;
        private float time, timerUpper;
        public CellType t;
        private Vector3 lastPosition;
        [HideInInspector]
        public bool HasNewDest, HasHospitaldestination, HasLoveMakingLocal, burried;
        public int CONSTANT_RADIUS = 3;
        public int radius = 3;
        public float insideBuildingCounter = 0;
        [HideInInspector]
        public NpcStats lover = null;

        private void OnEnable()
        {
            agent = GetComponent<NavMeshAgent>();
            time = 0;
            timerUpper = 0;
            insideBuildingCounter = 0;
        }

        void Start()
        {
            giveDestination = GetComponent<GiveDestination>();
            nextDest = lastDest = giveDestination.NextDestination(Vector3.zero);
            MoveToLocation(nextDest.Item2);
            lastPosition = transform.position;
            burried = HasNewDest = HasHospitaldestination = HasLoveMakingLocal = false;
            stats = GetComponent<NpcStats>();
            rend = GetComponent<Renderer>();
            color = rend.material.color * (float)stats.Life / MAXVALUE;
        }

        void Update()
        {
            if (!burried)
            {
                NpcAction(stats.State);

                t = nextDest.Item1;
                if (stats.IsHazardous)
                {
                    TimeCounter(ref time);

                    if (time >= 1)
                    {
                        Infect(stats.InfectRadius);
                    }
                }
            }
        }

        private void NpcAction(NpcState npcState)
        {
            switch (npcState)
            {
                case NpcState.Wandering:
                    WanderingAction();
                    break;
                case NpcState.InsideBuilding:
                    InsideOfBuilding();
                    break;
                case NpcState.Horny:
                    GoingToBuildingAction(CellType.Building);
                    break;
                case NpcState.GoingToHospital:
                    GoingToBuildingAction(CellType.Hospital);
                    break;
                case NpcState.Dead:
                    DeadAction();
                    break;
            }
        }
        public void MoveToLocation(Vector3 targetPos)
        {
            Vector3 newTarget = Vector3.zero;
            if (RandomPoint(targetPos, 2, out newTarget)
                && agent.navMeshOwner != null)
            {
                agent.SetDestination(newTarget);
                agent.isStopped = false;
                this.GetComponent<MeshRenderer>().enabled = true;
                this.GetComponent<CapsuleCollider>().enabled = true;
            }
        }
        private void SetColor()
        {
            if (stats.Life > 0)
            {
                color = new Color((float)stats.SexDrive / MAXVALUE
                    , (float)stats.SickMeter / MAXVALUE
                    , (float)stats.HappinessMeter / MAXVALUE
                    , 1);
                rend.material.SetColor("_Color", color);
            }
        }

        private Vector3 GetBuilding()
        {
            RaycastHit hit = new RaycastHit();
            Vector3 buildingPos = Vector3.zero;
            if (Physics.Raycast(transform.position, transform.forward, out hit, 5)
                || Physics.Raycast(transform.position, -transform.forward, out hit, 5)
                || Physics.Raycast(transform.position, transform.right, out hit, 5)
                || Physics.Raycast(transform.position, -transform.right, out hit, 5))
            {
                if (hit.collider.tag == CellType.Building.ToString())
                {
                    RandomPoint(hit.collider.ClosestPoint(transform.position), 2, out buildingPos);
                }
            }
            return buildingPos;
        }
        private Vector3 FindingBuilding(CellType cellType)
        {
            float closest = ProceduralCity.Instance.Size;
            Vector3 buildingPos = Vector3.zero;
            if (cellType == CellType.Building)
            {
                buildingPos = GetBuilding();
            }
            else
            {
                foreach (var item in giveDestination.destinations)
                {
                    if (item.Item1 == CellType.Hospital
                        && Vector3.Distance(transform.position, item.Item2) < closest)
                    {
                        buildingPos = item.Item2;
                        closest = Vector3.Distance(transform.position, item.Item2);
                    }
                }
            }
            return buildingPos;
        }
        private void GoingToBuildingAction(CellType cellType)
        {
            nextDest = new Tuple<CellType, Vector3>(cellType, FindingBuilding(cellType));
            SetColor();

            if (!HasHospitaldestination && cellType == CellType.Hospital)
            {
                MoveToLocation(nextDest.Item2);
                HasHospitaldestination = true;
            }
            if (!HasLoveMakingLocal && cellType == CellType.Building)
            {
                lover = GetLover();

                if (lover != null)
                {
                    MoveToLocation(nextDest.Item2);
                    HasLoveMakingLocal = true;

                    lover.GetComponent<NpcManager>().lover = stats;
                    lover.GetComponent<NpcManager>().MoveToLocation(nextDest.Item2);
                    lover.GetComponent<NpcManager>().HasLoveMakingLocal = true;
                }
            }
            if (Vector3.Distance(nextDest.Item2,
                            new Vector3((int)transform.position.x
                            , (int)transform.position.y
                            , (int)transform.position.z)) < CONSTANT_RADIUS)
            {
                if (lover != null && cellType == CellType.Building)
                {
                    GetInside(nextDest.Item1);
                }
                else if (cellType == CellType.Hospital)
                {
                    GetInside(nextDest.Item1);
                }
            }
        }
        private void GetInside(CellType targetType)
        {
            //RaycastHit hit = new RaycastHit();

            //if (Physics.Raycast(transform.position, transform.forward, out hit, 5)
            //    || Physics.Raycast(transform.position, -transform.forward, out hit, 5)
            //    || Physics.Raycast(transform.position, transform.right, out hit, 5)
            //    || Physics.Raycast(transform.position, -transform.right, out hit, 5))
            //{
            //    if (hit.collider.tag == targetType.ToString())
            //    {
            stats.State = NpcState.InsideBuilding;
            //    }
            //}
        }
        private void InsideOfBuilding()
        {
            if (agent.navMeshOwner != null)
            {
                agent.isStopped = true;
            }
            this.GetComponent<MeshRenderer>().enabled = false;
            this.GetComponent<CapsuleCollider>().enabled = false;
            if (stats.SickMeter > 20)
            {
                LowerStats(ref stats, ref stats.sickMeter, ref insideBuildingCounter);
                if (stats.Diseases.Count >= 3)
                {
                    RemoveDiseaseTrans(stats, stats.Diseases);
                    stats.Diseases.Clear();
                    stats.IsInfectable = false;
                    stats.IsHazardous = false;
                }
            }
            else if (stats.sexDrive > 20)
            {
                LowerStats(ref stats, ref stats.sexDrive, ref insideBuildingCounter);
            }
        }

        private void LowerStats(ref NpcStats stats, ref int meter, ref float insideBuildingCounter)
        {
            InsideCounter(ref insideBuildingCounter, 1);
            if (meter <= 25)
            {
                this.GetComponent<MeshRenderer>().enabled = true;
                this.GetComponent<CapsuleCollider>().enabled = true;
                stats.state = NpcState.Wandering;
            }
            else if (insideBuildingCounter >= 1)
            {
                meter -= 1;
            }
        }

        private void UpperStats(ref NpcStats stats, ref int meter, ref float timerUpper)
        {
            TimeCounter(ref timerUpper);
            if (meter < 100 && timerUpper >= 1.0f)
            {
                meter += 1 * stats.Diseases.Count;
            }
        }

        private NpcStats GetLover()
        {
            int radius = 1;
            NpcStats lover = null;
            while (lover == null
                && radius < ProceduralCity.Instance.Size)
            {
                lover = NpcStatsManager.componentListing
                    .GetNeighbours(stats.Index, radius)
                    .Find(x => x.SexDrive > 75 && x.Index != stats.Index);
                radius++;
            }
            if (lover != null)
            {
                lover.state = NpcState.Horny;
            }
            return lover;
        }

        private void DeadAction()
        {
            if (stats.Life <= 0)
            {
                rend.material.SetColor("_Color", Color.black);
                color = Color.black;

                if (agent != null && agent.navMeshOwner != null)
                {
                    agent.isStopped = true;
                }
                if (stats.Diseases.Count > 0)
                {
                    RemoveDiseaseTrans(stats, stats.Diseases);
                    stats.Diseases.Clear();
                }
                CityStats.Instance.Update100();
                CityStats.Instance.Gender100Update();
                burried = true;
            }
        }

        private void WanderingAction()
        {
            HasHospitaldestination = false;

            if (!HasNewDest)
            {
                radius++;
            }
            if (Vector3.Distance(transform.position, lastPosition) < 0.01f)
            {
                HasNewDest = false;
            }
            if (Vector3.Distance(nextDest.Item2,
                new Vector3((int)transform.position.x, (int)transform.position.y, (int)transform.position.z)) < radius)
            {
                nextDest = giveDestination.NextDestination(nextDest.Item2);
                MoveToLocation(nextDest.Item2);
                radius = CONSTANT_RADIUS;
                HasNewDest = true;
            }
            if (HasNewDest)
            {
                lastPosition = transform.position;
            }
            if (stats.diseaseCount > 0)
            {
                UpperStats(ref stats, ref stats.sickMeter, ref timerUpper);
            }
            SetColor();
        }

        bool RandomPoint(Vector3 center, float range, out Vector3 result)
        {
            for (int i = 0; i < 30; i++)
            {
                Vector3 randomPoint = center + Random.insideUnitSphere * range;
                NavMeshHit hit;
                if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
                {
                    result = hit.position;
                    return true;
                }
            }
            result = Vector3.zero;
            return false;
        }

        private void Infect(float infectRadius)
        {
            if (stats.Diseases.Count > 0)
            {
                List<NpcStats> infected = new List<NpcStats>();
                infected = NpcStatsManager.componentListing.GetNeighbours(stats.Index, (int)infectRadius);
                Tuple<WeatherSeasons, Card, int> dis = stats.Diseases[Random.Range(0, stats.Diseases.Count)];
                float contagiosRate = dis.Item2.contagiousRate;
                foreach (var item in infected)
                {
                    if (stats.Diseases.Find(x => x.Item1 == WeatherManager.weatherManager.season) != null
                        && !item.Diseases.Contains(dis)
                        && item.IsInfectable && (item.state == NpcState.Wandering || item.state == NpcState.Horny)
                        && contagiosRate > stats.TaxaContagio
                        && (dis.Item2.BasicVaccination? !item.isVacinated : true)
                        && (dis.Item2.AdvancedVaccination? !item.isVacinatedAdvanced : true))
                    {
                        item.Diseases.Add(dis);
                        GameManagerPC.Instance.AddTransformToDisease(item.transform, dis.Item2);
                        if (contagiosRate > item.ContagiousRate)
                        {
                            item.ContagiousRate = contagiosRate;
                        }
                        item.IsHazardous = true;
                    }
                    contagiosRate--;
                }
                CityStats.Instance.Update100();
            }
        }
        private void TimeCounter(ref float time)
        {
            if (time > 1.0f)
            {
                time = 0;
            }
            else
            {
                time += Time.deltaTime;
            }
        }
        private bool InsideCounter(ref float insideBuildingCounter, float counterMax)
        {
            bool getout = false;
            if (insideBuildingCounter >= counterMax)
            {
                insideBuildingCounter = 0;
                getout = true;
            }
            else
            {
                insideBuildingCounter += Time.deltaTime;
                getout = false;
            }

            return getout;
        }

        public static void RemoveDiseaseTrans(NpcStats stats, List<Tuple<WeatherSeasons, Card, int>> diseases)
        {
            if (diseases.Count > 0)
            {
                foreach (var item in diseases)
                {
                    GameManagerPC.Instance.RemoveTransfromFromDisease(stats.GetComponentInParent<Transform>(), item.Item2);
                }
            }
        }
    }
}
