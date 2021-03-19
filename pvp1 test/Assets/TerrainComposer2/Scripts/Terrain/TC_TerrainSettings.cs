using UnityEngine;
using System;

namespace TerrainComposer2
{
    [ExecuteInEditMode]
    public class TC_TerrainSettings : MonoBehaviour
    {

        // Base Terrain
        public float heightmapPixelError = 5.0f;
        public int heightmapMaximumLOD = 0;

        // Tree & Detail Terrain
        public bool drawTreesAndFoliage = true;
        public float treeDistance = 2000.0f;
        public float detailObjectDistance = 80.0f;
        public float detailObjectDensity = 1.0f;
        public float treeBillboardDistance = 50.0f;
        public int treeMaximumFullLODCount = 50;
        public float basemapDistance = 20000;


        public void Start()
        {
            SetTerrainSettings();
        }

        public void SetTerrainSettings()
        {
            Terrain terrain = GetComponent<Terrain>();
            if (terrain == null) return;

            terrain.heightmapPixelError = heightmapPixelError;
            terrain.heightmapMaximumLOD = heightmapMaximumLOD;

            if (drawTreesAndFoliage)
            {
                terrain.treeDistance = treeDistance;
                terrain.detailObjectDistance = detailObjectDistance;
            }
            else
            {
                terrain.treeDistance = 0.0f;
                terrain.detailObjectDistance = 0.0f;
            }

            terrain.detailObjectDensity = detailObjectDensity;
            terrain.treeMaximumFullLODCount = treeMaximumFullLODCount;
            terrain.treeBillboardDistance = treeBillboardDistance;
            terrain.treeMaximumFullLODCount = treeMaximumFullLODCount;
            terrain.basemapDistance = basemapDistance;
            terrain.terrainData.wavingGrassAmount = 0.25f;
        }
    }
}