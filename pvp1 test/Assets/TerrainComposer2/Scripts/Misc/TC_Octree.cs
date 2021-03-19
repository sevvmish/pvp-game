using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace TerrainComposer2
{
    public class Octree
    {
        public Cell cell;
        
        public class SpawnedObject
        {
            public int objectIndex;
            public int listIndex;
            public Vector3 position;
            public MaxCell cell;

            public SpawnedObject(int objectIndex, Vector3 position)
            {
                this.objectIndex = objectIndex;
                this.position = position;
            }

            public void Remove()
            {
                cell.objects[listIndex] = cell.objects[cell.objects.Count - 1];
                cell.objects.RemoveAt(cell.objects.Count - 1);

                if (cell.objects.Count == 0)
                {
                    cell.parent.RemoveCell(cell.cellIndex);
                }
            }
        }
        
        public class MaxCell : Cell
        {
            public List<SpawnedObject> objects = new List<SpawnedObject>();

            public MaxCell(Cell parent, int cellIndex, Bounds bounds): base(parent, cellIndex, bounds) { }

            ~MaxCell()
            {
                objects.Clear();
                objects = null;
            }
        }

        public class Cell
        {
            public Cell mainParent;
            public Cell parent;
            public Cell[] cells;
            public bool[] cellsUsed;

            public Bounds bounds;
            public int cellIndex;
            public int cellCount;
            public int level;
            public int maxLevels;

            public Cell(Cell parent, int cellIndex, Bounds bounds)
            {
                if (parent != null)
                {
                    maxLevels = parent.maxLevels;
                    mainParent = parent.mainParent;
                    level = parent.level + 1;
                }
                this.parent = parent;
                this.cellIndex = cellIndex;
                this.bounds = bounds;
            }

            ~Cell()
            {
                mainParent = parent = null;
                cells = null;
            }

            int AddCell(Vector3 position)
            {
                Vector3 localPos = position - bounds.min;

                int x = (int)(localPos.x / bounds.extents.x);
                int y = (int)(localPos.y / bounds.extents.y);
                int z = (int)(localPos.z / bounds.extents.z);

                int index = x + (y * 4) + (z * 2);
                
                if (cells == null) { cells = new Cell[8]; cellsUsed = new bool[8]; }

                // Reporter.Log("index "+index+" position "+localPos+" x: "+x+" y: "+y+" z: "+z+" extents "+bounds.extents);

                if (!cellsUsed[index])
                {
                    Bounds subBounds = new Bounds(new Vector3(bounds.min.x + (bounds.extents.x * (x + 0.5f)), bounds.min.y + (bounds.extents.y * (y + 0.5f)), bounds.min.z + (bounds.extents.z * (z + 0.5f))), bounds.extents);

                    if (level == maxLevels - 1) cells[index] = new MaxCell(this, index, subBounds); else cells[index] = new Cell(this, index, subBounds);

                    cellsUsed[index] = true;
                    ++cellCount;
                }
                return index;
            }

            public void RemoveCell(int index)
            {
                cells[index] = null;
                cellsUsed[index] = false;
                --cellCount;
                if (cellCount == 0)
                {
                    if (parent != null) parent.RemoveCell(cellIndex);
                }
            }

            public bool InsideBounds(Vector3 position)
            {
                position -= bounds.min;
                if (position.x >= bounds.size.x || position.y >= bounds.size.y || position.z >= bounds.size.z || position.x < 0 || position.y < 0 || position.z < 0) { return false; }
                return true;
            } //===============================================================================================================================


            public void AddObject(SpawnedObject obj)
            {
                if (bounds.Contains(obj.position)) AddObjectInternal(obj);
            }

            void AddObjectInternal(SpawnedObject obj)
            {
                if (level == maxLevels)
                {
                    obj.cell = (MaxCell)this;
                    obj.cell.objects.Add(obj);
                    obj.listIndex = obj.cell.objects.Count - 1;
                }
                else
                {
                    int index = AddCell(obj.position);
                    cells[index].AddObjectInternal(obj);
                }
            }

            public void Reset()
            {
                if (cells == null) return;

                for (int i = 0; i < 8; i++)
                {
                    cells[i] = null;
                    cellsUsed[i] = false;
                }
            }

            public void Draw(bool onlyMaxLevel)
            {
                if (!onlyMaxLevel || level == maxLevels)
                {
                    Gizmos.DrawWireCube(bounds.center, bounds.size);
                    if (level == maxLevels) return;
                }

                for (int i = 0; i < 8; i++)
                {
                    if (cellsUsed[i]) cells[i].Draw(onlyMaxLevel);
                }
            }
        }


    }
}
