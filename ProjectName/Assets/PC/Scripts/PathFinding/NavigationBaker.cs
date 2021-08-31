using Assets.PC.PCG;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.PC.PathFinding
{
    public class NavigationBaker : MonoBehaviour
    {
        void Start()
        {
            GetComponent<NavMeshSurface>().BuildNavMesh();
            ProceduralCity.Instance.NpsSpawn();
        }
    }
}
