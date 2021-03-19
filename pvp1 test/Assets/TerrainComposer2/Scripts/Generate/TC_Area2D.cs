using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

// using System.Diagnostics;

namespace TerrainComposer2
{
    [ExecuteInEditMode]
    public class TC_Area2D : MonoBehaviour
    {
        static public TC_Area2D instance;
        public TC_TerrainArea[] terrainAreas;
        public TC_TerrainMeshArea[] meshTerrainAreas;
        [NonSerialized] public Terrain currentTerrain;
        [NonSerialized] public TC_Terrain currentTCTerrain;
        [NonSerialized] public TCUnityTerrain currentTCUnityTerrain;
        [NonSerialized] public MeshTerrain currentMeshTerrain;
        public TC_TerrainArea currentTerrainArea;
        [NonSerialized] public TC_TerrainMeshArea currentMeshTerrainArea;
        
        public TC_PreviewArea previewArea;
        public Rect area, totalArea;
        public Bounds totalTerrainBounds;
        public TC_TerrainLayer layerLevelC;
        public TC_TerrainLayer terrainLayer;
        public Vector2 resolution, resolutionPM, resToPreview, worldPos, localPos, pos, localNPos, previewPos, snapOffsetUV, outputOffsetV2;
        public Vector3 startPos, terrainSize, outputOffsetV3;
        public Bounds bounds;
        public Int2 intResolution;
        public float heightN, height, angle;
        public int splatLength;
        public int splatmapLength;
        public float frames;
        public float[] splatTotal;
        public float terrainsToDo, terrainsDone;
        public float progress;
        public bool showProgressBar;
        public int previewResolution;
        [NonSerialized] public int resExpandBorder;
        [NonSerialized] public float resExpandBorderSize;


        public Vector2 terrainHeightRange;

        // public List<SpawnObject> spawnObjectList = new List<SpawnObject>();

        void Awake() 
        { 
            if (instance == null) instance = this; 
        } 
        void Start() 
        { 
            if (instance == null) instance = this; 
        }

        void OnDestroy() 
        { 
            if (instance == this) instance = null; 
        }

        public void CalcProgress()
        {
            progress = (((localPos.y / area.height) + (localPos.x / (area.width * area.height))) / terrainsToDo) + (terrainsDone / terrainsToDo);
        }

        void OnEnable()
        {
            if (instance == null) instance = this;

            currentTerrainArea = terrainAreas[0];
        }

        void OnDisable()
        {
            if (instance == this) instance = null;
        }

        public void DisposeTextures()
        {
            for (int i = 0; i < terrainAreas.Length; i++)
            {
                if (terrainAreas[i] != null) terrainAreas[i].DisposeTextures();
            }
        }

        public void SetCurrentArea(TCUnityTerrain tcTerrain)
        {
            currentTerrain = tcTerrain.terrain;
            currentTCUnityTerrain = tcTerrain;
            currentTCTerrain = tcTerrain;
            currentTerrainArea = terrainAreas[0];
        }

        public bool SetCurrentArea(TCUnityTerrain tcTerrain, int outputId)
        {
            // Debug.Log(tcTerrain.terrain.name);
            SetCurrentArea(tcTerrain);
            Terrain terrain = currentTerrain;

            if (!currentTCUnityTerrain.active) return false;

            intResolution = new Int2();
            Int2 resolution2 = new Int2();

            if (terrain.terrainData.heightmapResolution > 2049 || (TC_Generate.instance.scanHeight && !terrainLayer.layerGroups[TC.heightOutput].active))
            {
                resExpandBorder = 0;
                resExpandBorderSize = 0;
            }
            else
            {
                resExpandBorder = Mathf.RoundToInt((terrain.terrainData.heightmapResolution - 1) * TC_Settings.instance.resExpandBorderPercentage);
                resExpandBorderSize = terrain.terrainData.size.x * TC_Settings.instance.resExpandBorderPercentage;
            }

            // Debug.Log(resExpandBorder);
            // Debug.Log(resExpandBorderSize);

            if (outputId == TC.heightOutput)
            {
                intResolution.x = intResolution.y = (terrain.terrainData.heightmapResolution) + (resExpandBorder * 2);
                resolution2 = new Int2(terrain.terrainData.heightmapResolution, terrain.terrainData.heightmapResolution);
            }
            else if (outputId == TC.splatOutput)
            {
                intResolution.x = intResolution.y = terrain.terrainData.alphamapResolution;
                resolution2 = intResolution;
                splatLength = TC.GetTerrainSplatTextureLength(currentTerrain);
                splatmapLength = currentTerrain.terrainData.alphamapTextures.Length;
            }
            else if (outputId == TC.treeOutput) { intResolution.x = intResolution.y = (int)(terrain.terrainData.size.x / terrainLayer.treeResolutionPM); resolution2 = intResolution; }
            else if (outputId == TC.grassOutput) { intResolution.x = intResolution.y = terrain.terrainData.detailResolution; resolution2 = intResolution; }
            else if (outputId == TC.objectOutput)
            {
                intResolution.x = intResolution.y = (int)(terrain.terrainData.size.x / terrainLayer.objectResolutionPM); resolution2 = intResolution;
                //if (false)
                //{
                //    area.center = new Vector2((int)terrainLayer.objectTransform.position.x, (int)terrainLayer.objectTransform.position.z);
                //    area.size = new Vector2(terrainLayer.objectResolutionPM, terrainLayer.objectResolutionPM);
                //    totalArea.position = area.position;
                //    totalArea.size = terrainLayer.objectAreaSize;
                //    resolutionPM = new Vector2(terrainLayer.objectAreaSize.x / (resolution2.x), terrainLayer.objectAreaSize.y / (resolution2.y));
                //}
            }
            else if (outputId == TC.colorOutput) { intResolution.x = intResolution.y = terrainLayer.colormapResolution; resolution2 = intResolution; }

            outputOffsetV2 = new Vector2(terrainLayer.layerGroups[outputId].t.position.x, terrainLayer.layerGroups[outputId].t.position.z);
            outputOffsetV3 = new Vector3(outputOffsetV2.x, 0, outputOffsetV2.y);

            resolution = intResolution.ToVector2();

            if (intResolution.x < TC_Settings.instance.previewResolution) { previewResolution = intResolution.x; TC_Reporter.Log("From " + TC_Settings.instance.previewResolution + " To " + previewResolution); }
            else previewResolution = TC_Settings.instance.previewResolution;

            resToPreview = new Vector2((previewResolution - 0) / (totalArea.width + 0), (previewResolution - 0) / (totalArea.height + 0));

            if (outputId == TC.heightOutput || outputId == TC.splatOutput) resolutionPM = new Vector2(terrain.terrainData.size.x / (resolution2.x - 1), terrain.terrainData.size.z / (resolution2.y - 1));
            else resolutionPM = new Vector2(terrain.terrainData.size.x / (resolution2.x - 0), terrain.terrainData.size.z / (resolution2.y - 0));

            // if (outputId == TC.objectOutput) Debug.Log(resolutionPM);

            // resolutionPM = new Vector2(terrain.terrainData.size.x / (resolution2.x - 1), terrain.terrainData.size.z / (resolution2.y - 1));

            if (outputId == TC.heightOutput)
            {
                // area = new Rect(terrain.transform.position.x - resolutionPM.x, terrain.transform.position.z - resolutionPM.y, intResolution.x - 0, intResolution.y - 0);
                area = new Rect(terrain.transform.position.x - (resolutionPM.x * resExpandBorder), terrain.transform.position.z - (resolutionPM.y * resExpandBorder), intResolution.x - 0, intResolution.y - 0);
            }
            else
            {
                // resolutionPM = new Vector2(terrain.terrainData.size.x / (resolution2.x), terrain.terrainData.size.z / (resolution2.y));
                Vector2 posSnap;
                posSnap.x = Mathw.Snap(terrain.transform.position.x, resolutionPM.x);
                posSnap.y = Mathw.Snap(terrain.transform.position.z, resolutionPM.y);

                if (outputId == TC.treeOutput || outputId == TC.objectOutput)
                {
                    // posSnap += resolutionPM / 2;
                }
                area = new Rect(posSnap.x, posSnap.y, intResolution.x, intResolution.y);

                snapOffsetUV = (new Vector2(terrain.transform.position.x, terrain.transform.position.z) - posSnap);
                snapOffsetUV.x /= terrain.terrainData.size.x;
                snapOffsetUV.y /= terrain.terrainData.size.z;
                
                // Debug.Log(area);
            }

            bounds = new Bounds(terrain.transform.position + terrain.terrainData.size / 2, terrain.terrainData.size);
            startPos = new Vector3(area.xMin, terrain.transform.position.y, area.yMin);
            terrainSize = terrain.terrainData.size;

            return true;
        }


        public void SetManualTotalArea()
        {
            totalArea = previewArea.area;
            resToPreview = new Vector2(instance.previewResolution / totalArea.width, previewResolution / totalArea.height);
        }

        public bool CalcTotalArea()
        {
            Terrain terrain;
            Vector2 minPos = new Vector2(Mathf.Infinity, Mathf.Infinity);
            Vector2 maxPos = new Vector2(-Mathf.Infinity, -Mathf.Infinity);
            Vector2 heightPos = new Vector2(Mathf.Infinity, -Mathf.Infinity);
            Vector2 heightPosTerrain;
            Vector2 pos;

            Vector3 position, size;


            for (int j = 0; j < terrainAreas.Length; j++)
            {
                for (int i = 0; i < terrainAreas[j].terrains.Count; i++)
                {
                    terrain = terrainAreas[j].terrains[i].terrain;
                    if (terrain == null) return false;
                    if (!terrain.gameObject.activeSelf) continue;

                    position = terrain.transform.position;
                    size = terrain.terrainData.size;

                    if (position.x < minPos.x) minPos.x = position.x;
                    if (position.z < minPos.y) minPos.y = position.z;

                    pos = new Vector2(position.x + size.x, position.z + size.z);
                    heightPosTerrain = new Vector2(position.y, position.y + size.y);
                    if (pos.x > maxPos.x) maxPos.x = pos.x;
                    if (pos.y > maxPos.y) maxPos.y = pos.y;
                    if (heightPosTerrain.x < heightPos.x) heightPos.x = heightPosTerrain.x;
                    if (heightPosTerrain.y > heightPos.y) heightPos.y = heightPosTerrain.y;
                }
            }

            totalArea = new Rect();
            totalArea.xMin = minPos.x;
            totalArea.yMin = minPos.y;
            totalArea.xMax = maxPos.x;
            totalArea.yMax = maxPos.y;

            totalTerrainBounds = new Bounds();
            totalTerrainBounds.min = new Vector3(minPos.x, heightPos.x, minPos.y);
            totalTerrainBounds.max = new Vector3(maxPos.x, heightPos.y, maxPos.y);

            return true;
        }

        public void SetTerrainAreasSize(Vector3 size)
        {
            for (int i = 0; i < terrainAreas.Length; i++)
            {
                if (terrainAreas[i].active) terrainAreas[i].terrainSize = size;
            }
        }

        public void ApplyTerrainAreasSize()
        {
            for (int i = 0; i < terrainAreas.Length; i++)
            {
                if (terrainAreas[i].active) terrainAreas[i].ApplySizeTerrainArea();
            }
        }

        public void AddTerrainArea(TC_TerrainArea terrainArea)
        {
            // if (!terrainAreas.Contains(terrainArea)) terrainAreas.Add(terrainArea);
        }

        public void RemoveTerrainArea(TC_TerrainArea terrainArea)
        {
            // int index = terrainAreas.IndexOf(terrainArea);
            // if (index != -1) terrainAreas.RemoveAt(index);
        }

        public void ApplySizeTerrainAreas()
        {
            for (int i = 0; i < terrainAreas.Length; i++) terrainAreas[i].ApplySizeTerrainArea();
        }

        public void ApplyResolutionTerrainAreas(TCUnityTerrain sTerrain)
        {
            for (int i = 0; i < terrainAreas.Length; i++) terrainAreas[i].ApplyResolutionTerrainArea(sTerrain);
        }

        public void ApplySplatTexturesTerrainAreas(TCUnityTerrain sTerrain)
        {
            for (int i = 0; i < terrainAreas.Length; i++) terrainAreas[i].ApplySplatTextures(sTerrain);
        }

    }
}