using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.PC.PCG
{
    public class Cell
    {
        //Fields
        Vector3 worldPosition;
        int size, x, y;
        CellType type;
        GameObject prefab;
        bool instantialize;


        //Properties
        public Vector3 WorldPosition
        {
            get
            {
                return worldPosition;
            }

            set
            {
                worldPosition = value;
            }
        }
        public int Size
        {
            get
            {
                return size;
            }

            set
            {
                size = value;
            }
        }
        public CellType Type
        {
            get
            {
                return type;
            }

            set
            {
                type = value;
            }
        }
        public int X
        {
            get
            {
                return x;
            }

            set
            {
                x = value;
            }
        }
        public int Y
        {
            get
            {
                return y;
            }

            set
            {
                y = value;
            }
        }

        public GameObject Prefab
        {
            get
            {
                return prefab;
            }

            set
            {
                prefab = value;
            }
        }

        public bool Instantialize
        {
            get
            {
                return instantialize;
            }

            set
            {
                instantialize = value;
            }
        }

        //Contructor
        public Cell(Vector3 worldPosition, int x, int y, int size = 1, CellType type = CellType.None)
        {
            this.size = size;
            this.worldPosition = worldPosition;
            this.type = type;
            this.x = x;
            this.y = y;
        }
    }
}