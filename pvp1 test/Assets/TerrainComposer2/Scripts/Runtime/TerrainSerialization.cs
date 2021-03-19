using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;

public class TerrainSerialization : MonoBehaviour
{
    public bool serialize;
    public bool deserialize;

    public Terrain[] terrains;

    void Update()
    {
        if (serialize)
        {
            serialize = false;
            SaveTerrains(Application.dataPath + "/MyTerrain.dat", terrains);
        }

        if (deserialize)
        {
            deserialize = false;
            LoadTerrain(Application.dataPath + "/MyTerrain.dat");
        }
    }

    public void SaveTerrains(string path, Terrain[] terrains)
    {
        List<byte> bytes = new List<byte>();
        R_SerializationHelper.SerializeInt(bytes, terrains.Length);
        for (int i = 0; i < terrains.Length; i++) SerializeTerrain(bytes, terrains[i]);

        if (bytes.Count > 0)
        {
            FileStream file = new FileStream(path, FileMode.Create);
            file.Write(bytes.ToArray(), 0, bytes.Count);
            file.Close();
        }
    }

    public void SaveTerrain(string path, Terrain terrain)
    {
        List<byte> bytes = new List<byte>();
        R_SerializationHelper.SerializeInt(bytes, 1);
        SerializeTerrain(bytes, terrain);

        if (bytes.Count > 0)
        {
            FileStream file = new FileStream(path, FileMode.Create);
            file.Write(bytes.ToArray(), 0, bytes.Count);
            file.Close();
        }
    }

    public Terrain[] LoadTerrain(string path)
    {
        FileStream file = new FileStream(path, FileMode.Open);
        if (file == null)
        {
            Debug.Log(path + " not found.");
            return null;
        }
        byte[] bytes = new byte[file.Length];
        file.Read(bytes, 0, bytes.Length);
        
        int index = 0;
        int terrainLength = R_SerializationHelper.DeserializeInt(bytes, ref index);
        Terrain[] terrains = new Terrain[terrainLength];
        for (int i = 0; i < terrainLength; i++)
        {
            terrains[i] = DeserializeTerrain(bytes, ref index);
        }
        return terrains;
    }

    public void SerializeTerrain(List<byte> bytes, Terrain terrain)
    {
        if (terrain == null) return;
        if (terrain.terrainData == null) return;

        R_SerializationHelper.SerializeString(bytes, terrain.name);
        R_SerializationHelper.SerializeVector3(bytes, terrain.transform.position);
        
        R_SerializationHelper.SerializeFloat(bytes, terrain.basemapDistance);
        #if UNITY_5 || UNITY_2017 || UNITY_2018_1 || UNITY_2018_2 || UNITY_2018_3 || UNITY_2018_4
        R_SerializationHelper.SerializeBool(bytes, terrain.castShadows);
        #else
        R_SerializationHelper.SerializeInt(bytes, (int)terrain.shadowCastingMode);
        #endif
        R_SerializationHelper.SerializeBool(bytes, terrain.collectDetailPatches);
        R_SerializationHelper.SerializeFloat(bytes, terrain.detailObjectDensity);
        R_SerializationHelper.SerializeFloat(bytes, terrain.detailObjectDistance);
        R_SerializationHelper.SerializeBool(bytes, terrain.drawHeightmap);
        R_SerializationHelper.SerializeBool(bytes, terrain.drawTreesAndFoliage);
        R_SerializationHelper.SerializeInt(bytes, terrain.heightmapMaximumLOD);
        R_SerializationHelper.SerializeFloat(bytes, terrain.heightmapPixelError);
        #if UNITY_5 || UNITY_2017 || UNITY_2018_1 || UNITY_2018_2 || UNITY_2018_3 || UNITY_2019_1
        R_SerializationHelper.SerializeFloat(bytes, terrain.legacyShininess);
        R_SerializationHelper.SerializeColor(bytes, terrain.legacySpecular);
        #endif
        R_SerializationHelper.SerializeInt(bytes, terrain.lightmapIndex);
        R_SerializationHelper.SerializeVector4(bytes, terrain.lightmapScaleOffset);

        if (terrain.materialTemplate != null)
        {
            bytes.Add(1);
            R_SerializationHelper.SerializeString(bytes, terrain.materialTemplate.name);
        }
        else bytes.Add(0);

        #if UNITY_5 || UNITY_2017 || UNITY_2018_1 || UNITY_2018_2 || UNITY_2018_3 || UNITY_2019_1
        R_SerializationHelper.SerializeInt(bytes, (int)terrain.materialType);
        #endif
        R_SerializationHelper.SerializeInt(bytes, terrain.realtimeLightmapIndex); 
        R_SerializationHelper.SerializeVector4(bytes, terrain.realtimeLightmapScaleOffset);
        R_SerializationHelper.SerializeInt(bytes, (int)terrain.reflectionProbeUsage);
        R_SerializationHelper.SerializeFloat(bytes, terrain.treeBillboardDistance);
        R_SerializationHelper.SerializeFloat(bytes, terrain.treeCrossFadeLength);
        R_SerializationHelper.SerializeFloat(bytes, terrain.treeDistance);
        R_SerializationHelper.SerializeInt(bytes, terrain.treeMaximumFullLODCount);

        SerializeTerrainData(bytes, terrain.terrainData);

        // Debug.Log(bytes.Count);
    }

    public Terrain DeserializeTerrain(byte[] bytes, ref int index)
    {
        GameObject go = Terrain.CreateTerrainGameObject(null);
        Terrain terrain = go.GetComponent<Terrain>();
        TerrainCollider terrainCollider = go.GetComponent<TerrainCollider>();

        terrain.name = R_SerializationHelper.DeserializeString(bytes, ref index);
        terrain.transform.position = R_SerializationHelper.DeserializeVector3(bytes, ref index);

        terrain.basemapDistance = R_SerializationHelper.DeserializeFloat(bytes, ref index);
#if UNITY_5 || UNITY_2017 || UNITY_2018_1 || UNITY_2018_2 || UNITY_2018_3 || UNITY_2018_4
        terrain.castShadows = R_SerializationHelper.DeserializeBool(bytes, ref index);
#else
        terrain.shadowCastingMode = (UnityEngine.Rendering.ShadowCastingMode)R_SerializationHelper.DeserializeInt(bytes, ref index);
#endif
        terrain.collectDetailPatches = R_SerializationHelper.DeserializeBool(bytes, ref index);
        terrain.detailObjectDensity = R_SerializationHelper.DeserializeFloat(bytes, ref index);
        terrain.detailObjectDistance = R_SerializationHelper.DeserializeFloat(bytes, ref index);
        terrain.drawHeightmap = R_SerializationHelper.DeserializeBool(bytes, ref index);
        terrain.drawTreesAndFoliage = R_SerializationHelper.DeserializeBool(bytes, ref index);

        terrain.heightmapMaximumLOD = R_SerializationHelper.DeserializeInt(bytes, ref index);
        terrain.heightmapPixelError = R_SerializationHelper.DeserializeFloat(bytes, ref index);

#if UNITY_5 || UNITY_2017 || UNITY_2018_1 || UNITY_2018_2 || UNITY_2018_3 || UNITY_2019_1
        terrain.legacyShininess = R_SerializationHelper.DeserializeFloat(bytes, ref index);
        terrain.legacySpecular = R_SerializationHelper.DeserializeColor(bytes, ref index);
#endif
        terrain.lightmapIndex = R_SerializationHelper.DeserializeInt(bytes, ref index);

        terrain.lightmapScaleOffset = R_SerializationHelper.DeserializeVector4(bytes, ref index);
        if (bytes[index++] == 1)
        {
            R_SerializationHelper.DeserializeString(bytes, ref index);
        }
#if UNITY_5 || UNITY_2017 || UNITY_2018_1 || UNITY_2018_2 || UNITY_2018_3 || UNITY_2019_1
        terrain.materialType = (Terrain.MaterialType)R_SerializationHelper.DeserializeInt(bytes, ref index);
#endif
        terrain.realtimeLightmapIndex = R_SerializationHelper.DeserializeInt(bytes, ref index);
        terrain.realtimeLightmapScaleOffset = R_SerializationHelper.DeserializeVector4(bytes, ref index);
        terrain.reflectionProbeUsage = (UnityEngine.Rendering.ReflectionProbeUsage)R_SerializationHelper.DeserializeInt(bytes, ref index);
        terrain.treeBillboardDistance = R_SerializationHelper.DeserializeFloat(bytes, ref index);
        terrain.treeCrossFadeLength = R_SerializationHelper.DeserializeFloat(bytes, ref index);
        terrain.treeDistance = R_SerializationHelper.DeserializeFloat(bytes, ref index);
        terrain.treeMaximumFullLODCount = R_SerializationHelper.DeserializeInt(bytes, ref index);

        terrain.terrainData = DeserializeTerrainData(bytes, ref index);
        terrainCollider.terrainData = terrain.terrainData;

        return terrain;
    }
    
    public void SerializeTerrainData(List<byte> bytes, TerrainData terrainData)
    {
        R_SerializationHelper.SerializeString(bytes, terrainData.name);

        int heightmapResolution = terrainData.heightmapResolution;
        int detailResolution = terrainData.detailResolution;

        R_SerializationHelper.SerializeInt(bytes, heightmapResolution);
        R_SerializationHelper.SerializeInt(bytes, terrainData.baseMapResolution);
        R_SerializationHelper.SerializeInt(bytes, terrainData.alphamapResolution);
        R_SerializationHelper.SerializeInt(bytes, detailResolution);
        R_SerializationHelper.SerializeVector3(bytes, terrainData.size);

        // R_SerializationHelper.SerializeFloat(bytes, terrainData.thickness);
        R_SerializationHelper.SerializeFloat(bytes, terrainData.wavingGrassAmount);
        R_SerializationHelper.SerializeFloat(bytes, terrainData.wavingGrassSpeed);
        R_SerializationHelper.SerializeFloat(bytes, terrainData.wavingGrassStrength);
        R_SerializationHelper.SerializeColor(bytes, terrainData.wavingGrassTint);


        // Splat textures
#if UNITY_5 || UNITY_2017 || UNITY_2018_1 || UNITY_2018_2
        SplatPrototype[] splatPrototypes = terrainData.splatPrototypes;
        R_SerializationHelper.SerializeInt(bytes, splatPrototypes.Length);
        for (int i = 0; i < splatPrototypes.Length; i++)
        {
            SplatPrototype splat = splatPrototypes[i];
            R_SerializationHelper.SerializeFloat(bytes, splat.metallic);
            if (splat.normalMap != null)
            {
                bytes.Add(1);
                R_SerializationHelper.SerializeString(bytes, splat.normalMap.name);
            }
            else bytes.Add(0);

            R_SerializationHelper.SerializeFloat(bytes, splat.smoothness);
            R_SerializationHelper.SerializeString(bytes, splat.texture.name);
            R_SerializationHelper.SerializeVector2(bytes, splat.tileOffset);
            R_SerializationHelper.SerializeVector2(bytes, splat.tileSize);
        }
#else
        TerrainLayer[] terrainLayers = terrainData.terrainLayers;
        R_SerializationHelper.SerializeInt(bytes, terrainLayers.Length);
        for (int i = 0; i < terrainLayers.Length; i++)
        {
            TerrainLayer terrainLayer = terrainLayers[i];
            R_SerializationHelper.SerializeFloat(bytes, terrainLayer.metallic);
            if (terrainLayer.normalMapTexture != null)
            {
                bytes.Add(1);
                R_SerializationHelper.SerializeString(bytes, terrainLayer.normalMapTexture.name);
                R_SerializationHelper.SerializeFloat(bytes, terrainLayer.normalScale);
            }
            else bytes.Add(0);

            R_SerializationHelper.SerializeFloat(bytes, terrainLayer.smoothness);
            R_SerializationHelper.SerializeString(bytes, terrainLayer.diffuseTexture.name);
            R_SerializationHelper.SerializeVector2(bytes, terrainLayer.tileOffset);
            R_SerializationHelper.SerializeVector2(bytes, terrainLayer.tileSize);
        }

#endif

        // Tree Prototypes
        TreePrototype[] treePrototypes = terrainData.treePrototypes;
        R_SerializationHelper.SerializeInt(bytes, treePrototypes.Length);
        for (int i = 0; i < treePrototypes.Length; i++)
        {
            TreePrototype tree = treePrototypes[i];
            R_SerializationHelper.SerializeFloat(bytes, tree.bendFactor);
            R_SerializationHelper.SerializeString(bytes, tree.prefab.name);
        }
         
        // Grass
        DetailPrototype[] detailPrototypes = terrainData.detailPrototypes;
        int detailPrototypesLength = detailPrototypes.Length;
        R_SerializationHelper.SerializeInt(bytes, detailPrototypes.Length);
        for (int i = 0; i < detailPrototypes.Length; i++)
        {
            DetailPrototype detail = detailPrototypes[i];
            R_SerializationHelper.SerializeFloat(bytes, detail.bendFactor);
            R_SerializationHelper.SerializeColor(bytes, detail.dryColor);
            R_SerializationHelper.SerializeColor(bytes, detail.healthyColor);
            R_SerializationHelper.SerializeFloat(bytes, detail.maxHeight);
            R_SerializationHelper.SerializeFloat(bytes, detail.maxWidth);
            R_SerializationHelper.SerializeFloat(bytes, detail.minHeight);
            R_SerializationHelper.SerializeFloat(bytes, detail.minWidth);
            R_SerializationHelper.SerializeFloat(bytes, detail.noiseSpread);
            if (detail.prototype != null)
            {
                bytes.Add(1);
                R_SerializationHelper.SerializeString(bytes, detail.prototype.name);
            }
            else bytes.Add(0);

            if (detail.prototypeTexture != null)
            {
                bytes.Add(1);
                R_SerializationHelper.SerializeString(bytes, detail.prototypeTexture.name);
            }
            else bytes.Add(0);

            R_SerializationHelper.SerializeInt(bytes, (int)detail.renderMode);
        }

        // Heights
        float[,] heights = terrainData.GetHeights(0, 0, heightmapResolution, heightmapResolution);
        R_SerializationHelper.Serialize2DFloatArray(bytes, heights);

        // Splat maps
        R_SerializationHelper.SerializeInt(bytes, terrainData.alphamapTextures.Length);
        for (int i = 0; i < terrainData.alphamapTextures.Length; i++)
        {
            Texture2D tex = terrainData.alphamapTextures[i];
            byte[] texBytes = tex.EncodeToPNG();
            R_SerializationHelper.SerializeInt(bytes, texBytes.Length);
            bytes.AddRange(texBytes);
        }

        // Trees
        TreeInstance[] treeInstances = terrainData.treeInstances;
        R_SerializationHelper.SerializeInt(bytes, treeInstances.Length);
        for (int i = 0; i < treeInstances.Length; i++)
        {
            TreeInstance tree = treeInstances[i];
            R_SerializationHelper.SerializeColor(bytes, tree.color);
            R_SerializationHelper.SerializeFloat(bytes, tree.heightScale);
            R_SerializationHelper.SerializeColor(bytes, tree.lightmapColor);
            R_SerializationHelper.SerializeVector3(bytes, tree.position);
            R_SerializationHelper.SerializeInt(bytes, tree.prototypeIndex);
            R_SerializationHelper.SerializeFloat(bytes, tree.rotation);
            R_SerializationHelper.SerializeFloat(bytes, tree.widthScale);
        }

        // Grass
        for (int i = 0; i < detailPrototypesLength; i++)
        {
            int[,] detailMap = terrainData.GetDetailLayer(0, 0, detailResolution, detailResolution, i);
            R_SerializationHelper.Serialize2DIntArrayToBytes(bytes, detailMap);
        }
    }

    public TerrainData DeserializeTerrainData(byte[] bytes, ref int index)
    {
        TerrainData terrainData = new TerrainData();

        terrainData.name = R_SerializationHelper.DeserializeString(bytes, ref index);
        // Debug.Log(terrainData.name);

        int heightmapResolution = R_SerializationHelper.DeserializeInt(bytes, ref index);
        terrainData.heightmapResolution = heightmapResolution;
        terrainData.baseMapResolution = R_SerializationHelper.DeserializeInt(bytes, ref index);
        terrainData.alphamapResolution = R_SerializationHelper.DeserializeInt(bytes, ref index);
        int detailResolution = R_SerializationHelper.DeserializeInt(bytes, ref index);
        terrainData.SetDetailResolution(detailResolution, 16);

        terrainData.size = R_SerializationHelper.DeserializeVector3(bytes, ref index);

        // terrainData.thickness = R_SerializationHelper.DeserializeFloat(bytes, ref index);
        terrainData.wavingGrassAmount = R_SerializationHelper.DeserializeFloat(bytes, ref index);
        terrainData.wavingGrassSpeed = R_SerializationHelper.DeserializeFloat(bytes, ref index);
        terrainData.wavingGrassStrength = R_SerializationHelper.DeserializeFloat(bytes, ref index);
        terrainData.wavingGrassTint = R_SerializationHelper.DeserializeColor(bytes, ref index);

        // Splat Textures
#if UNITY_5 || UNITY_2017 || UNITY_2018_1 || UNITY_2018_2
        int splatLength = R_SerializationHelper.DeserializeInt(bytes, ref index);
        SplatPrototype[] splatPrototypes = new SplatPrototype[splatLength];
        for (int i = 0; i < splatLength; i++)
        {
            SplatPrototype splat = new SplatPrototype();
            splat.metallic = R_SerializationHelper.DeserializeFloat(bytes, ref index);
            if (bytes[index++] == 1)
            {
                string normalName = R_SerializationHelper.DeserializeString(bytes, ref index);
                splat.normalMap = (Texture2D)Resources.Load(normalName);
            }
            splat.smoothness = R_SerializationHelper.DeserializeFloat(bytes, ref index);
            string splatName = R_SerializationHelper.DeserializeString(bytes, ref index);
            splat.texture = (Texture2D)Resources.Load(splatName);
            splat.tileOffset = R_SerializationHelper.DeserializeVector2(bytes, ref index);
            splat.tileSize = R_SerializationHelper.DeserializeVector2(bytes, ref index);
            splatPrototypes[i] = splat;
        }
        terrainData.splatPrototypes = splatPrototypes;
#else
        int terrainLayerLength = R_SerializationHelper.DeserializeInt(bytes, ref index);
        TerrainLayer[] terrainLayers = new TerrainLayer[terrainLayerLength];
        for (int i = 0; i < terrainLayerLength; i++)
        {
            TerrainLayer terrainLayer = new TerrainLayer();
            terrainLayer.metallic = R_SerializationHelper.DeserializeFloat(bytes, ref index);
            if (bytes[index++] == 1)
            {
                string normalName = R_SerializationHelper.DeserializeString(bytes, ref index);
                terrainLayer.normalMapTexture = (Texture2D)Resources.Load(normalName);
                terrainLayer.normalScale = R_SerializationHelper.DeserializeFloat(bytes, ref index);
            }
            terrainLayer.smoothness = R_SerializationHelper.DeserializeFloat(bytes, ref index);
            string splatName = R_SerializationHelper.DeserializeString(bytes, ref index);
            terrainLayer.diffuseTexture = (Texture2D)Resources.Load(splatName);
            terrainLayer.tileOffset = R_SerializationHelper.DeserializeVector2(bytes, ref index);
            terrainLayer.tileSize = R_SerializationHelper.DeserializeVector2(bytes, ref index);
            terrainLayers[i] = terrainLayer;
        }
        terrainData.terrainLayers = terrainLayers;

#endif

        // Tree Prototypes
        int treeLength = R_SerializationHelper.DeserializeInt(bytes, ref index);
        TreePrototype[] treePrototypes = new TreePrototype[treeLength];
        for (int i = 0; i < treeLength; i++)
        {
            TreePrototype tree = new TreePrototype();
            tree.bendFactor = R_SerializationHelper.DeserializeFloat(bytes, ref index);
            string prefabName = R_SerializationHelper.DeserializeString(bytes, ref index);
            tree.prefab = (GameObject)Resources.Load(prefabName);
            treePrototypes[i] = tree;
        }
        terrainData.treePrototypes = treePrototypes;

        // Grass Prototypes
        int grassLength = R_SerializationHelper.DeserializeInt(bytes, ref index);
        DetailPrototype[] detailPrototypes = new DetailPrototype[grassLength];
        for (int i = 0; i < grassLength; i++)
        {
            DetailPrototype grass = new DetailPrototype();
            grass.bendFactor = R_SerializationHelper.DeserializeFloat(bytes, ref index);
            grass.dryColor = R_SerializationHelper.DeserializeColor(bytes, ref index);
            grass.healthyColor = R_SerializationHelper.DeserializeColor(bytes, ref index);
            grass.maxHeight = R_SerializationHelper.DeserializeFloat(bytes, ref index);
            grass.maxWidth = R_SerializationHelper.DeserializeFloat(bytes, ref index);
            grass.minHeight = R_SerializationHelper.DeserializeFloat(bytes, ref index);
            grass.minWidth = R_SerializationHelper.DeserializeFloat(bytes, ref index);
            grass.noiseSpread = R_SerializationHelper.DeserializeFloat(bytes, ref index);
            if (bytes[index++] == 1)
            {
                R_SerializationHelper.DeserializeString(bytes, ref index);
            }
            if (bytes[index++] == 1)
            {
                string textureName = R_SerializationHelper.DeserializeString(bytes, ref index);
                grass.prototypeTexture = (Texture2D)Resources.Load(textureName);
            }
            grass.renderMode = (DetailRenderMode)R_SerializationHelper.DeserializeInt(bytes, ref index);
            detailPrototypes[i] = grass;
        }
        terrainData.detailPrototypes = detailPrototypes;

        float[,] heights = R_SerializationHelper.Deserialize2DFloatArray(bytes, ref index);
        terrainData.SetHeights(0, 0, heights);

        int splatmapLength = R_SerializationHelper.DeserializeInt(bytes, ref index);
        Texture2D[] alphamapTextures = terrainData.alphamapTextures;

        for (int i = 0; i < splatmapLength; i++)
        {
            int length = R_SerializationHelper.DeserializeInt(bytes, ref index);
            byte[] texBytes = new byte[length];
            Array.Copy(bytes, index, texBytes, 0, length);
            index += length;
            alphamapTextures[i].LoadImage(texBytes);
            alphamapTextures[i].Apply();
        }

        int treeInstancesLength = R_SerializationHelper.DeserializeInt(bytes, ref index);
        TreeInstance[] trees = new TreeInstance[treeInstancesLength];

        for (int i = 0; i < trees.Length; i++)
        {
            TreeInstance tree = new TreeInstance();
            tree.color = R_SerializationHelper.DeserializeColor(bytes, ref index);
            tree.heightScale = R_SerializationHelper.DeserializeFloat(bytes, ref index);
            tree.lightmapColor = R_SerializationHelper.DeserializeColor(bytes, ref index);
            tree.position = R_SerializationHelper.DeserializeVector3(bytes, ref index);
            tree.prototypeIndex = R_SerializationHelper.DeserializeInt(bytes, ref index);
            tree.rotation = R_SerializationHelper.DeserializeFloat(bytes, ref index);
            tree.widthScale = R_SerializationHelper.DeserializeFloat(bytes, ref index);
            trees[i] = tree;
        }
        terrainData.treeInstances = trees;

        for (int i = 0; i < grassLength; i++)
        {
            int[,] grassMap = R_SerializationHelper.Deserialize2DByteArrayToInt(bytes, ref index);
            terrainData.SetDetailLayer(0, 0, i, grassMap);
        }

        return terrainData;
    }
}
