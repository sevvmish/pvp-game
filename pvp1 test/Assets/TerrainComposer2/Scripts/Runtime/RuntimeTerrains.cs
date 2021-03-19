using UnityEngine;
// using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace TerrainComposer2
{
    [ExecuteInEditMode]
    public class RuntimeTerrains : MonoBehaviour
    {
        float frames;

        public bool moveTerrainsWithCamera;
        public Transform mainCamera;

        float terrainSize;
        // float halfTerrainSize;
        float totalSize;

        Vector3[] initPos;
        Vector3 oldPos;
        float relativePos, newPos, offset;

        public List<TCUnityTerrain> tcTerrains = new List<TCUnityTerrain>();
        public TC_TerrainArea terrainArea;

        List<TCUnityTerrain> taskList = new List<TCUnityTerrain>();
        
        void Start()
        {
            if (moveTerrainsWithCamera)
            {
                if (mainCamera == null)
                {
                    Debug.Log("Assign the Main Camera");
                    Debug.Break();
                }

                UpdateMoveTerrain();
            }
        }

        void OnEnable()
        {
            terrainArea = TC_Area2D.instance.terrainAreas[0];
            tcTerrains = terrainArea.terrains;
            Start();
        }

        void Update()
        {
            UpdateMoveTerrain();
        }

        void UpdateMoveTerrain()
        {
            if (terrainArea == null || mainCamera == null) return;
            if (initPos == null || initPos.Length != tcTerrains.Count) StartMoveTerrains();

            UpdateTerrainPositionsX();
            UpdateTerrainPositionsZ();

            if (taskList.Count > 0)
            {
                // terrainArea.SetNeighbors();

                for (int i = 0; i < taskList.Count; i++)
                {
                    TC_Generate.instance.Generate(taskList[i], false);
                }

                taskList.Clear();
            }
        }

        void StartMoveTerrains()
        {
            terrainSize = tcTerrains[0].terrain.terrainData.size.x;
            totalSize = terrainArea.tiles.x * terrainSize;
            // halfTerrainSize = terrainSize / 2;

            initPos = new Vector3[tcTerrains.Count];

            terrainArea.AssignTerrainArray();

            for (int i = 0; i < tcTerrains.Count; i++)
            {
                initPos[i] = tcTerrains[i].terrain.transform.position;
            }

            offset = terrainSize / 2;

            if ((totalSize / terrainSize) % 2 != 0)
            {

            }
        }

        void UpdateTerrainPositionsX()
        {
            for (int i = 0; i < tcTerrains.Count; i++)
            {
                TCUnityTerrain tcTerrain = tcTerrains[i];
                Terrain terrain = tcTerrain.terrain;
                
                relativePos = mainCamera.position.x - initPos[i].x;
                newPos = (Mathf.Round((relativePos - offset) / totalSize) * totalSize) + initPos[i].x;

                if (terrain.transform.position.x != newPos)
                {
                    if (newPos > terrain.transform.position.x)
                    {
                        // Debug.Log("->");
                        
                        // terrainArea.terrainArray[terrainArea.tiles.x - 1, tcTerrain.tileZ] = 
                        // tcTerrain.tileX = terrainArea.tiles.x - 1;
                        // for (int x = 1; x < terrainArea.tiles.x; x++) terrainArea.GetTCUnityTerrainTile(x, tcTerrain.tileZ).tileX = x - 1;
                        // tcTerrains.RemoveAt(i);
                        // tcTerrains.Insert(i + 1, tcTerrain);
                    }
                    else
                    {
                        // Debug.Log("<-");
                        // tcTerrain.tileX = 0;
                        // for (int x = 0; x < terrainArea.tiles.x - 1; x++) terrainArea.GetTCUnityTerrainTile(x, tcTerrain.tileZ).tileX = x + 1;
                    }

                    // terrain.gameObject.SetActive(false);
                    tcTerrain.newPos = new Vector3(newPos, terrain.transform.position.y, terrain.transform.position.z);
                    tcTerrain.updateTerrainPos = true;
                    // terrains.Add(terrains[i]);
                    tcTerrain.active = true;
                    taskList.Add(tcTerrain);
                }
            }
            
            oldPos.x = Mathf.Round(mainCamera.position.x / terrainSize) * terrainSize;
        }

        void UpdateTerrainPositionsZ()
        {
            for (int i = 0; i < tcTerrains.Count; i++)
            {
                TCUnityTerrain tcTerrain = tcTerrains[i];
                Terrain terrain = tcTerrain.terrain;

                relativePos = mainCamera.position.z - initPos[i].z;
                newPos = (Mathf.Round((relativePos - offset) / totalSize) * totalSize) + initPos[i].z;

                if (terrain.transform.position.z != newPos)
                {
                    if (newPos > terrain.transform.position.z)
                    {
                        tcTerrain.tileZ = terrainArea.tiles.y - 1;
                        for (int z = 1; z < terrainArea.tiles.y; z++)
                        {
                            terrainArea.GetTCUnityTerrainTile(tcTerrain.tileX, z).tileZ = z - 1;
                        }
                    }
                    else
                    {
                        tcTerrain.tileZ = 0;
                        for (int z = 0; z < terrainArea.tiles.y - 1; z++) terrainArea.GetTCUnityTerrainTile(tcTerrain.tileX, z).tileZ = z + 1;
                    }

                    // terrain.gameObject.SetActive(false);
                    tcTerrain.newPos = new Vector3(terrain.transform.position.x, terrain.transform.position.y, newPos);
                    tcTerrain.updateTerrainPos = true;
                    // terrains.Add(terrains[i]);
                    tcTerrain.active = true;
                    if (!taskList.Contains(tcTerrain)) taskList.Add(tcTerrain);
                }
            }
            oldPos.z = Mathf.Round(mainCamera.position.z / terrainSize) * terrainSize;
        }
    }
}