using System;
using System.Collections.Generic;
using Assets.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace Assets.PC.PCG
{
    public class ProceduralCity : SingletonBehaviour<ProceduralCity>
    {

        //Editor Fields
        [SerializeField]
        int size, numberNpcs;
        [HideInInspector]
        int cellSize = 1, hospitals;
        [SerializeField]
        GameObject[] cityPrefabs;
        [SerializeField]
        int hospitalCells = 8, bankCells = 4, schoolCells = 2, SkyScrapperCells = 14;
        [SerializeField]
        GameObject roadsGO, nonBuildingsGO, buildingsGO, npcsGO;

        //Class fields
        private Grid grid;
        private int roads;

        //Constants
        const float roadsTown = 2.5f;
        const int minSize = 10;
        const float roadHeight = 0.3f;

        //Public fields
        [HideInInspector]
        public List<Tuple<CellType, Vector3>> nonBuildings;
        [HideInInspector]
        public bool[] roadsPicture;

        //Properties
        public int Size
        { get => size; }


        private void OnEnable()
        {
            //Constraints
            size = Size < minSize ? minSize : Size;
            roads = (int)(Size * 2) / 3;
            hospitalCells = hospitalCells % 2 == 0 && hospitalCells >= 4 ? hospitalCells : 6;
            bankCells = bankCells % 2 == 0 && bankCells >= 4 ? bankCells : 4;
            schoolCells = schoolCells % 2 == 0 && schoolCells >= 4 ? schoolCells : 2;
            numberNpcs = numberNpcs > Size * Size ? Size * Size : numberNpcs;

            //Prefab Scaling
            cityPrefabs[0].transform.localScale = new Vector3(cellSize, roadHeight, cellSize);
            cityPrefabs[1].transform.localScale = new Vector3(Size / 10 * cellSize, 1, Size / 10 * cellSize);

            //Grid Inicialization
            grid = new Grid(Size, cellSize);
            nonBuildings = new List<Tuple<CellType, Vector3>>();

            //Ground Instantiation
            GameObject.Instantiate<GameObject>(cityPrefabs[1],
                        new Vector3(grid[(Size / 2) - 1].WorldPosition.x + (float)cellSize / 2
                        , grid[(Size * Size / 2) - 1].WorldPosition.y
                        , grid[(Size * Size / 2) - 1].WorldPosition.z + (float)cellSize / 2)
                        , cityPrefabs[1].transform.rotation, this.transform);

            //City Making
            GridPopulation();
            RoadBuilding();
            ObjectsInstantion();

            //Array de bools que representam aas roads
            roadsPicture = new bool[Size * Size];
            for (int i = 0; i < Size * Size; i++)
            {
                if (grid[i].Type == CellType.Road)
                {
                    roadsPicture[i] = true;
                }
            }
        }
        public void NpsSpawn()
        {
            for (int i = 0; i < numberNpcs; i++)
            {
                int numberNPCPerCell = Random.Range(4, 10);

                if (grid.PositionXY(0, i % (Size - 1)) == i
                                || grid.PositionXY(i % (Size - 1), 0) == i
                                || grid.PositionXY(i % (Size - 1), Size - 1) == i
                                || grid.PositionXY(Size - 1, i % (Size - 1)) == i)
                {
                    for (int j = 0; j < numberNPCPerCell; j++)
                    {
                        int numberNPCPerCellX = Random.Range(1, numberNPCPerCell);
                        int numberNPCPerCellZ = Random.Range(1, numberNPCPerCell);
                        float evalX = Random.Range(-1, 1);
                        float evalZ = Random.Range(-1, 1);
                        if (grid.PositionXY(0, i % (Size - 1)) == i || grid.PositionXY(i % (Size - 1), 0) == i)
                        {
                            evalX = grid[i].WorldPosition.x + cityPrefabs[6].transform.position.x + evalX * ((float)cellSize / numberNPCPerCellX) < 0 ? 0.5f : evalX;
                            evalZ = grid[i].WorldPosition.z + cityPrefabs[6].transform.position.z + evalZ * ((float)cellSize / numberNPCPerCellZ) < 0 ? 0.5f : evalZ;
                        }
                        else if (grid.PositionXY(i % (Size - 1), Size - 1) == i || grid.PositionXY(Size - 1, i % (Size - 1)) == i)
                        {
                            evalX = grid[i].WorldPosition.x + cityPrefabs[6].transform.position.x + evalX * ((float)cellSize / numberNPCPerCellX) > 0 ? 0.5f : evalX;
                            evalZ = grid[i].WorldPosition.z + cityPrefabs[6].transform.position.z + evalZ * ((float)cellSize / numberNPCPerCellZ) > 0 ? 0.5f : evalZ;
                        }
                        var citizen = GameObject.Instantiate<GameObject>(cityPrefabs[6],
                                new Vector3(grid[i].WorldPosition.x + cityPrefabs[6].transform.position.x + evalX * ((float)cellSize / numberNPCPerCellX)
                                , grid[i].WorldPosition.y + cityPrefabs[6].transform.localScale.y / 2 + cityPrefabs[6].transform.position.y
                                , grid[i].WorldPosition.z + cityPrefabs[6].transform.position.z + evalZ * ((float)cellSize / numberNPCPerCellZ))
                                , Quaternion.identity, npcsGO.transform);
                        citizen.layer = LayerMask.NameToLayer("Citizens");
                    }
                    // GameObject.Instantiate<GameObject>(cityPrefabs[6],
                    //new Vector3(grid[i].WorldPosition.x + cityPrefabs[6].transform.position.x - (float)cellSize / 4
                    //, grid[i].WorldPosition.y + cityPrefabs[6].transform.localScale.y / 2 + cityPrefabs[6].transform.position.y
                    //, grid[i].WorldPosition.z + cityPrefabs[6].transform.position.z)
                    //, Quaternion.identity, npcsGO.transform);

                    // GameObject.Instantiate<GameObject>(cityPrefabs[6],
                    //new Vector3(grid[i].WorldPosition.x + cityPrefabs[6].transform.position.x + (float)cellSize / 4
                    //, grid[i].WorldPosition.y + cityPrefabs[6].transform.localScale.y / 2 + cityPrefabs[6].transform.position.y
                    //, grid[i].WorldPosition.z + cityPrefabs[6].transform.position.z)
                    //, Quaternion.identity, npcsGO.transform);

                    // GameObject.Instantiate<GameObject>(cityPrefabs[6],
                    //new Vector3(grid[i].WorldPosition.x + cityPrefabs[6].transform.position.x
                    //, grid[i].WorldPosition.y + cityPrefabs[6].transform.localScale.y / 2 + cityPrefabs[6].transform.position.y
                    //, grid[i].WorldPosition.z + cityPrefabs[6].transform.position.z - (float)cellSize / 4)
                    //, Quaternion.identity, npcsGO.transform);

                    // GameObject.Instantiate<GameObject>(cityPrefabs[6],
                    //new Vector3(grid[i].WorldPosition.x + cityPrefabs[6].transform.position.x
                    //, grid[i].WorldPosition.y + cityPrefabs[6].transform.localScale.y / 2 + cityPrefabs[6].transform.position.y
                    //, grid[i].WorldPosition.z + cityPrefabs[6].transform.position.z + (float)cellSize / 4)
                    //, Quaternion.identity, npcsGO.transform);
                }
            }
        }

        private void GridPopulation()
        {
            CellType next = CellType.None;

            for (int i = 0; i < Size * Size; i++)
            {
                next = CellType.Building;

                if (grid.PositionXY(0, i % (Size - 1)) == i
                    || grid.PositionXY(i % (Size - 1), 0) == i
                    || grid.PositionXY(i % (Size - 1), Size - 1) == i
                    || grid.PositionXY(Size - 1, i % (Size - 1)) == i)
                {
                    next = CellType.Road;
                }

                if (grid[i].Type == CellType.None)
                {
                    switch (next)
                    {
                        case CellType.Road:
                            grid[i].Type = CellType.Road;
                            break;
                        case CellType.Building:
                            //Posso tambem verificar se as celulas que estao à volta sao edificos e se forem passam a none
                            //if (Random.RandomRange(0, 10) < 9)
                            //{
                            //    grid[i].Type = CellType.Building;
                            //}
                            //else
                            //{
                            grid[i].Type = CellType.Building;
                            //}
                            break;
                    }
                }
            }
        }

        private void RoadBuilding()
        {
            int n1 = 0, n2 = 0, n3 = 0;
            //float n = 0;
            Directions nextPath = (Directions)Random.Range(0, 3);
            for (int i = roads; i > 0; i--)//, n += (float)Size / roads)
            {

                n2 = (int)Random.Range(1, Size - 4);
                n1 = (int)Random.Range(1, Size - hospitalCells + 3);
                
                //Criação das estradas
                while (grid.NextDirection(grid[grid.PositionXY(n1, n2)], nextPath) == -1)
                {
                    nextPath = (Directions)(i * Random.Range(0, 4) % 4);
                }
                Directions nextPath2 = nextPath;
                NonBuildingInstantion(Directions.E, (size / 2) - 1, (size / 2) - 1, true);
                if (grid[grid.NextDirection(grid[grid.PositionXY(n1, n2)], nextPath)].Type == CellType.Building
                    || grid[grid.NextDirection(grid[grid.PositionXY(n1, n2)], nextPath)].Type == CellType.None)
                {
                    NonBuildingInstantion(nextPath, n1, n2, false);
                }
                //Adição dos edificios para serem destinos dos NPCs
                Cell cell = grid[grid.NextDirection(grid[grid.PositionXY(n1, n2)], nextPath)];
                if (cell.Type != CellType.Road)
                {
                    Tuple<CellType, Vector3> cellTuple
                        = new Tuple<CellType, Vector3>(cell.Type, cell.WorldPosition + new Vector3(0, 0.2f, 0));
                    nonBuildings.Add(cellTuple);
                }
                
                int end = Random.Range(1, 4);
                for (int l = 0; l < 2; l++)
                {

                    nextPath = (Directions)(i * Random.Range(0, 4) % 4);

                    while (grid.NextDirection(cell, nextPath) != -1)
                    {
                        if (grid[grid.NextDirection(cell, nextPath)].Type == CellType.Building)
                            grid[grid.NextDirection(cell, nextPath)].Type = CellType.Road;

                        cell = grid[grid.NextDirection(cell, nextPath)];
                    }
                    cell = grid[grid.NextDirection(grid[grid.PositionXY(n1, n2)], nextPath2)];
                }
                if (n2 >= n3 && n3 < Size - 1)
                {
                    n3 = n2 + 2;
                }
            }
            grid[Size * Size - 1].Type = CellType.Road;
        }

        private void NonBuildingInstantion(Directions nextPath, int n1, int n2, bool skyScrapper)
        {
            CellType nonBuilding;
            int cells = 0;
            if (skyScrapper)
            {
                nonBuilding = CellType.SkyScrapper;
            }
            else if (hospitals == 0)
            {
                nonBuilding = CellType.Hospital;
            }
            else
            {
                nonBuilding = (CellType)UnityEngine.Random.Range((int)CellType.Hospital, (int)CellType.None);
            }

            switch (nonBuilding)
            {
                case CellType.Hospital:
                    hospitals++;
                    cells = hospitalCells;
                    break;
                case CellType.School:
                    cells = schoolCells;
                    break;
                case CellType.Bank:
                    cells = bankCells;
                    break;
                case CellType.SkyScrapper:
                    cells = SkyScrapperCells;
                    break;
            }

            for (int k = 1, j = grid.NextDirection(grid[grid.PositionXY(n1, n2)], nextPath)
                ; j < cells + grid.NextDirection(grid[grid.PositionXY(n1, n2)], nextPath)
                ; j++, k++)
            {
                if (k > cells / 2)
                {
                    grid[j + Size - (cells / 2)].Type = nonBuilding;
                    grid[j + Size - (cells / 2)].Instantialize = false;
                }
                else
                {
                    grid[j].Type = nonBuilding;
                    grid[j].Instantialize = false;
                }
            }
            grid[grid.NextDirection(grid[grid.PositionXY(n1, n2)], nextPath)].Type = nonBuilding;
            grid[grid.NextDirection(grid[grid.PositionXY(n1, n2)], nextPath)].Instantialize = true;
        }

        private void ObjectsInstantion()
        {
            float randomHeight = 0;
            float randomSize = 0;
            
            for (int i = 0; i < Size * Size; i++)
            {
                randomHeight = Random.Range((float)cellSize, cellSize * 3);
                randomSize = Random.Range((float)cellSize / 2, cellSize);

                switch (grid[i].Type)
                {
                    case CellType.Road:
                        grid[i].Prefab = GameObject.Instantiate<GameObject>(cityPrefabs[0],
                      new Vector3(grid[i].WorldPosition.x + cityPrefabs[0].transform.position.x
                      , grid[i].WorldPosition.y + cityPrefabs[0].transform.localScale.y / 2
                      , grid[i].WorldPosition.z + cityPrefabs[0].transform.position.z)
                      , Quaternion.identity, roadsGO.transform);
                        break;

                    case CellType.Building:
                        cityPrefabs[2].transform.localScale
                            = new Vector3((float)randomSize, randomHeight, (float)randomSize);
                        grid[i].Prefab = GameObject.Instantiate<GameObject>(cityPrefabs[2],
                    new Vector3(grid[i].WorldPosition.x + cityPrefabs[2].transform.position.x
                    , grid[i].WorldPosition.y + cityPrefabs[2].transform.localScale.y / 2
                    , grid[i].WorldPosition.z + cityPrefabs[2].transform.position.z)
                    , Quaternion.identity, buildingsGO.transform);
                        break;

                    case CellType.Hospital:
                        if (grid[i].Instantialize == true)
                        {
                            cityPrefabs[3].transform.localScale
                                = new Vector3((cellSize * (float)hospitalCells / 2)
                                , randomHeight,
                                cellSize * 2);
                            grid[i].Prefab = GameObject.Instantiate<GameObject>(cityPrefabs[3],
                        new Vector3(grid[i].WorldPosition.x + cityPrefabs[3].transform.position.x + (float)hospitalCells / 4 - (float)cellSize / 2
                        , grid[i].WorldPosition.y + cityPrefabs[3].transform.localScale.y / 2
                        , grid[i].WorldPosition.z + cityPrefabs[3].transform.position.z + ((float)cellSize / 2))
                        , Quaternion.identity, nonBuildingsGO.transform);
                        }
                        break;

                    case CellType.Bank:
                        if (grid[i].Instantialize)
                        {
                            cityPrefabs[4].transform.localScale
                                = new Vector3(cellSize * (float)bankCells / 2
                                , randomHeight,
                                cellSize * 2);
                            grid[i].Prefab = GameObject.Instantiate<GameObject>(cityPrefabs[4],
                        new Vector3(grid[i].WorldPosition.x + cityPrefabs[4].transform.position.x + (float)bankCells / 4 - (float)cellSize / 2
                        , grid[i].WorldPosition.y + cityPrefabs[4].transform.localScale.y / 2
                        , grid[i].WorldPosition.z + cityPrefabs[4].transform.position.z + ((float)cellSize / 2))
                        , Quaternion.identity, nonBuildingsGO.transform);
                        }
                        break;

                    case CellType.School:
                        if (grid[i].Instantialize)
                        {
                            cityPrefabs[5].transform.localScale
                                = new Vector3(cellSize * (float)schoolCells / 2
                                , randomHeight,
                                cellSize * 2);
                            grid[i].Prefab = GameObject.Instantiate<GameObject>(cityPrefabs[5],
                        new Vector3(grid[i].WorldPosition.x + cityPrefabs[5].transform.position.x + (float)schoolCells / 4 - (float)cellSize / 2
                        , grid[i].WorldPosition.y + cityPrefabs[5].transform.localScale.y / 2
                        , grid[i].WorldPosition.z + cityPrefabs[5].transform.position.z + ((float)cellSize / 2))
                        , Quaternion.identity, nonBuildingsGO.transform);
                        }
                        break;
                    case CellType.SkyScrapper:
                        if (grid[i].Instantialize)
                        {
                            cityPrefabs[7].transform.localScale
                                = new Vector3(cellSize * (float)SkyScrapperCells / 2
                                , cellSize * 8,
                                cellSize * 2);
                            grid[i].Prefab = GameObject.Instantiate<GameObject>(cityPrefabs[7],
                        new Vector3(grid[i].WorldPosition.x + cityPrefabs[7].transform.position.x + (float)SkyScrapperCells / 4 - (float)cellSize / 2
                        , grid[i].WorldPosition.y + cityPrefabs[7].transform.localScale.y / 2
                        , grid[i].WorldPosition.z + cityPrefabs[7].transform.position.z + ((float)cellSize / 2))
                        , Quaternion.identity, nonBuildingsGO.transform);
                        }
                        break;
                }
            }
        }
    
    }
}