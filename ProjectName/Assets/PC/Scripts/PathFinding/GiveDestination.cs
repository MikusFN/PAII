using Assets.PC.PCG;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.PC.PathFinding
{
    public class GiveDestination : MonoBehaviour
    {
        public List<Tuple<CellType, Vector3>> destinations;

        private void Start()
        {
            destinations = new List<Tuple<CellType, Vector3>>();
            destinations = ProceduralCity.Instance.nonBuildings;
        }

        public Tuple<CellType, Vector3> NextDestination(Vector3 currentDestination)
        {
            int index = Random.Range(0, ProceduralCity.Instance.nonBuildings.Count);

            if (currentDestination != destinations[index].Item2)
            {
                return new Tuple<CellType, Vector3>(destinations[index].Item1, destinations[index].Item2);
            }
            else
            {
                return NextDestination(currentDestination);
            }
        }        
    }
}
