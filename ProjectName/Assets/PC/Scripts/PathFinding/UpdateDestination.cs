using Assets.PC.PCG;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace Assets.PC.PathFinding
{
    public class UpdateDestination : MonoBehaviour
    {
        private NavMeshAgent agent;
        private GiveDestination giveDestination;
        private Tuple<CellType, Vector3> nextDest;
        private Vector3 lastPosition;
        private bool HasNewDest;
        private const int CONSTANT_RADIUS = 3;
        public int radius = 3;

        private void OnEnable()
        {
            agent = GetComponent<NavMeshAgent>();
        }
        void Start()
        {
            giveDestination = GetComponent<GiveDestination>();
            nextDest = giveDestination.NextDestination(Vector3.zero);
            MoveToLocation(nextDest.Item2);
            lastPosition = transform.position;
            HasNewDest = false;
        }

        void Update()
        {
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
                GetInside(nextDest.Item1);
                nextDest = giveDestination.NextDestination(nextDest.Item2);
                MoveToLocation(nextDest.Item2);
                radius = CONSTANT_RADIUS;
                HasNewDest = true;
            }
            if (HasNewDest)
            {
                lastPosition = transform.position;
            }
        }

        private void GetInside(CellType targetType)
        {
            RaycastHit hit= new RaycastHit();

            if(Physics.Raycast(transform.position, transform.forward, out hit, 5))
            {                
                if(hit.collider.tag == targetType.ToString())
                {
                    this.GetComponent<MeshRenderer>().enabled = false;
                    this.GetComponent<CapsuleCollider>().enabled = false;
                }                
            }
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
        public void MoveToLocation(Vector3 targetPos)
        {
            Vector3 newTarget = Vector3.zero;
            if (RandomPoint(targetPos, 2, out newTarget)
                && agent != null)
            {
                agent.SetDestination(newTarget);
                agent.isStopped = false;
                this.GetComponent<MeshRenderer>().enabled = true;
                this.GetComponent<CapsuleCollider>().enabled = true;
            }
        }
    }
}
