using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.PC.PCG
{
    public class Grid
    {
        //Class Fields
        Cell[] grid;
        int gridSize;

        public Grid(int gridSize, int cellSize = 1)
        {
            this.gridSize = gridSize;

            grid = new Cell[gridSize * gridSize];
            int j = 0;
            for (int i = 0; i < grid.Length; i++)
            {
                if (i != 0 && i % gridSize == 0)
                {
                    j++;
                }
                grid[i] = new Cell(new Vector3(i % gridSize * cellSize, 0, j * cellSize), i % gridSize, j, cellSize);
            }
        }

        public int NextDirection(Cell cell, Directions direction)
        {
            int pos = -1;
            switch (direction)
            {
                case Directions.N:
                    if (cell.Y + 1 < gridSize - 1)
                        pos = PositionXY(cell.X, cell.Y + 1);
                    break;

                case Directions.E:
                    if (cell.X - 1 > 0)
                        pos = PositionXY(cell.X - 1, cell.Y);
                    break;

                case Directions.W:
                    if (cell.X + 1 < gridSize - 1)
                        pos = PositionXY(cell.X + 1, cell.Y);
                    break;

                case Directions.S:
                    if (cell.Y - 1 > 0)
                        pos = PositionXY(cell.X, cell.Y - 1);
                    break;
                default:
                    Debug.Log("DEFAULT");
                    if (pos == -1)
                        pos = PositionXY(cell.X, cell.Y);
                    break;
            }
            return pos;
        }

        public int PositionXY(int x, int y)
        {
            return y * gridSize + x;
        }
        public Cell this[int index]
        {
            get { return grid[index]; }
        }
    }
}