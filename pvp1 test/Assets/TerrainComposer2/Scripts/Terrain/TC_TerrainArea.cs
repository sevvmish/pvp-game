using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;

using System.Runtime.Serialization.Formatters.Binary;
public enum ApplyChanges { Terrain, TerrainArea, AllTerrainAreas }

namespace TerrainComposer2
{
    [ExecuteInEditMode]
    public class TC_TerrainArea : MonoBehaviour
    {
        public RenderTexture[] rtSplatmaps;
        public RenderTexture rtColormap;
        // public RenderTexture rtHeight;

        public bool loaded = false;

        public ApplyChanges applyChanges;

        // terrain area class
        public Int2 totalHeightmapResolution, heightTexResolution;

        public List<TCUnityTerrain> terrains = new List<TCUnityTerrain>();
        public TCUnityTerrain[,] terrainArray;
        public bool createTerrainTab = false;
        public bool active = true;
        public Color color = new Color(1.5f, 1.5f, 1.5f, 1.0f);
        public int index;

        public bool terrains_active = true;
        public bool terrains_scene_active = true;
        public bool terrains_foldout = true;
        public bool sizeTab, resolutionsTab, settingsTab, splatTab, treeTab, grassTab, resetTab;

        public Int2 tiles;
        public Int2 selectTiles;
        public bool tileLink = true;
        public bool useSameTerrainLayersForAllTerrains = true;
        public bool drawInstanced;

        public Rect area;
        public Vector3 terrainSize = new Vector3(2048, 750, 2048);
        public Vector3 center = new Vector3(0.0f, 0.0f, 0.0f);

        public Rect menuRect;
        public bool display_short;

        public string terrainDataPath;
        public Transform parent;
        public string terrainName = "Terrain";
        public bool copy_settings = true;
        public int copy_terrain = 0;

        public int terrainSelect = 0;
        public bool settingsEditor = true;

        static public readonly string[] heightmapResolutionList = new string[] { "4097", "2049", "1025", "513", "257", "129", "65", "33" };
        static public readonly string[] splatmapResolutionList = new string[] { "2048", "1024", "512", "256", "128", "64", "32", "16" };
        static public readonly string[] detailmapResolutionList = new string[] { "2048", "1024", "512", "256", "128", "64", "32", "16", "8" };
        static public readonly string[] detailResolutionPerPatchList = new string[] { "8", "16", "32", "64", "128" };
        static public readonly string[] imageImportMaxSettings = new string[] { "32", "64", "128", "256", "512", "1024", "2048", "4096" };


        void OnEnable()
        {
            if (!loaded)
            {
                loaded = true;
                terrainDataPath = Application.dataPath;
            }

            SetNeighbors();
            // AssignMaterials();
            // GetTerrainsFromChildren();
            // if (terrains.Count == 0) terrains.Add(new TCTerrain());
        }

        void AssignMaterials()
        {
#if !UNITY_5 && !UNITY_2017 && !UNITY_2018 && !UNITY_2019_1
            Shader terrainShader = Shader.Find("HDRP/TerrainLit");
            if (terrainShader == null) terrainShader = Shader.Find("Universal Render Pipeline/Terrain/Lit");
            if (terrainShader == null) terrainShader = Shader.Find("Nature/Terrain/Diffuse");

            for (int i = 0; i < terrains.Count; i++)
            { 
                terrains[i].materialTemplate = new Material(terrainShader);
                if (terrains[i].terrain) terrains[i].terrain.materialTemplate = terrains[i].materialTemplate;
            }
#endif
        }

        public bool IsRTPAddedToTerrains()
        {
            Type t = TC.FindRTP();

            if (t == null) return false;

            for (int i = 0; i < terrains.Count; i++)
            {
                Terrain terrain = terrains[i].terrain;
                if (terrain == null) continue;

                Component c = terrain.GetComponent(t);
                if (c == null) return false;
            }

            return true;
        }

        public void AddRTPTOTerrains()
        {
            Type t = TC.FindRTP();

            if (t == null) return;

            for (int i = 0; i < terrains.Count; i++)
            {
                Terrain terrain = terrains[i].terrain;
                if (terrain == null) continue;

                Component c = terrain.GetComponent(t);
                if (c == null) terrain.gameObject.AddComponent(t);
            }
        }

        // TODO
        public void AssignTerrainArray()
        {
            bool create = false;
            if (terrainArray == null) create = true;
            else if (terrainArray.Length != tiles.x * tiles.y) create = true;

            if (create)
            {
                terrainArray = new TCUnityTerrain[tiles.x, tiles.y];

                int i = 0;
                for (int y = 0; y < tiles.y; y++)
                {
                    for (int x = 0; x < tiles.x; x++) terrainArray[x, y] = terrains[i++];
                }
            }
        }
        //void Start()
        //{
        //    // This is needed for custom terrain material to not turn black
        //    #if UNITY_EDITOR
        //    UnityEditor.AssetDatabase.Refresh();
        //    #endif

        //    // ReassignTerrainMaterial();
        //}

        //void ReassignTerrainMaterial()
        //{
        //    if (terrains == null) return;

        //    for (int i = 0; i < terrains.Count; i++)
        //    {
        //        Terrain terrain = terrains[i].terrain;
        //        if (terrain == null) continue;

        //        Material matTerrain = terrain.materialTemplate;
        //        if (matTerrain == null) continue;

        //        if (matTerrain.GetTexture("_Colormap") != null)
        //        {
        //            terrain.materialTemplate.SetTexture("_Colormap", terrains[i].texColormap);
        //            #if UNITY_EDITOR
        //            UnityEditor.EditorUtility.SetDirty(terrain.materialTemplate);
        //            #endif
        //        }
        //    }
        //}

        void OnDestroy()
        {
            DisposeTextures();
        }

        public void DisposeTextures()
        {
            TC_Compute.DisposeRenderTextures(ref rtSplatmaps);
            TC_Compute.DisposeRenderTexture(ref rtColormap);

            for (int i = 0; i < terrains.Count; i++) terrains[i].DisposeTextures();
        }

        public void Clear()
        {
            terrains.Clear();
        }

        public void ResetTextureRTP(string texName)
        {
            for (int i = 0; i < terrains.Count; i++)
            {
                terrains[i].AssignTextureRTP(texName, null);
            }
        }
        public void ClearToOne()
        {
            int length = terrains.Count;
            
            if (terrains[0].terrain != null) DestroyImmediate(terrains[0].terrain.gameObject);

            for (int i = 1; i < length; ++i)
            {
                if (terrains[1].terrain != null) DestroyImmediate(terrains[1].terrain.gameObject);
                terrains.RemoveAt(1);
            }
        }

        public void CalcTotalResolutions()
        {
            // Debug.Log("Calc total heightmap resolution"); 
            if (terrains.Count == 0) return;
            if (terrains[0].terrain == null) return;
            if (terrains[0].terrain.terrainData == null) return;

            int heightmapResolution = terrains[0].terrain.terrainData.heightmapResolution;

            totalHeightmapResolution = new Int2(tiles.x * heightmapResolution, tiles.y * heightmapResolution);

            heightTexResolution = new Int2(tiles.x * (heightmapResolution - 1), tiles.y * (heightmapResolution - 1));
        }

        public void CalcTotalArea()
        {
            Vector2 minPos = new Vector2(terrains[0].terrain.transform.localPosition.x, terrains[0].terrain.transform.localPosition.z);
            Vector2 maxPos = minPos + new Vector2(terrains[0].terrain.terrainData.size.x, terrains[0].terrain.terrainData.size.z);
            Vector2 pos;

            for (int i = 1; i < terrains.Count; i++)
            {
                if (terrains[i].terrain.transform.localPosition.x < minPos.x) minPos.x = terrains[i].terrain.transform.localPosition.x;
                if (terrains[i].terrain.transform.localPosition.z < minPos.y) minPos.y = terrains[i].terrain.transform.localPosition.z;

                pos = new Vector2(terrains[i].terrain.transform.localPosition.x + terrains[i].terrain.terrainData.size.x, terrains[i].terrain.transform.localPosition.z + terrains[i].terrain.terrainData.size.z);
                if (pos.x > maxPos.x) maxPos.x = pos.x;
                if (pos.y > maxPos.y) maxPos.y = pos.y;
            }

            area = new Rect();
            area.xMin = minPos.x;
            area.yMin = minPos.y;
            area.xMax = maxPos.x;
            area.yMax = maxPos.y;
        }

        public void FitTerrainsPosition()
        {
            if (terrains.Count == 0) return;
            int countTerrain = 0;

            Vector3 size = terrains[0].terrain.terrainData.size;
            area.xMin = -size.x * (tiles.x / 2.0f);
            area.yMin = -size.z * (tiles.y / 2.0f);

            for (int y = 0; y < tiles.y; y++)
            {
                for (int x = 0; x < tiles.x; x++)
                {
                    terrains[countTerrain].tasks = 0;
                    if (terrains[countTerrain].terrain.terrainData.size != size) terrains[countTerrain].terrain.terrainData.size = size;
                    terrains[countTerrain].terrain.transform.localPosition = new Vector3(area.xMin + (size.x * x), 0, area.yMin + (size.z * y));
                    if (++countTerrain > terrains.Count) goto exit;
                }
            }

            exit:;
            // tile_resolution = (int)(tiles.x * size.x);
            SetNeighbors();
        }

        public void SetNeighbors()
        {
#if UNITY_5 || UNITY_2017 || UNITY_2018_1 || UNITY_2018_2
            for (int i = 0; i < terrains.Count; i++)
            {
                TCUnityTerrain t = terrains[i];
                if (!t.CheckValidUnityTerrain()) continue;

                Terrain top = GetTerrainTile(t.tileX, t.tileZ + 1);
                Terrain right = GetTerrainTile(t.tileX + 1, t.tileZ);
                Terrain bottom = GetTerrainTile(t.tileX, t.tileZ - 1);
                Terrain left = GetTerrainTile(t.tileX - 1, t.tileZ);

                t.terrain.SetNeighbors(left, top, right, bottom);
            }
#endif
        }

        public void ResetNeighbors()
        {
#if UNITY_5 || UNITY_2017 || UNITY_2018_1 || UNITY_2018_2
            for (int i = 0; i < terrains.Count; i++)
            {
                TCUnityTerrain t = terrains[i];
                if (!t.CheckValidUnityTerrain()) continue;

                t.terrain.SetNeighbors(null, null, null, null);
            }
#endif
        }

        public Terrain GetTerrainTile(int tileX, int tileZ)
        {
            if (tileX < 0 || tileX > tiles.x - 1 || tileZ < 0 || tileZ > tiles.y - 1) return null;
            int index = (tileZ * tiles.x) + tileX;

            if (!terrains[index].CheckValidUnityTerrain()) return null;

            return terrains[index].terrain;
        }

        public TCUnityTerrain GetTCUnityTerrainTile(int tileX, int tileZ)
        {
            if (tileX < 0 || tileX > tiles.x - 1 || tileZ < 0 || tileZ > tiles.y - 1) return null;
            int index = (tileZ * tiles.x) + tileX;

            return terrains[index];
        }

        public void SetTerrainsActive(bool invert)
        {
            for (int i = 0; i < terrains.Count; ++i)
            {
                if (!invert) terrains[i].active = terrains_active; else terrains[i].active = !terrains[i].active;
            }
        }

        public void GetTerrainsFromChildren()
        {
            Terrain[] unityTerrains = GetComponentsInChildren<Terrain>(true);
            terrains.Clear();

            if (unityTerrains.Length > 0)
            {
                terrainSize = unityTerrains[0].terrainData.size;
            }

            for (int i = 0; i < unityTerrains.Length; i++)
            {
                terrains.Add(new TCUnityTerrain());
                terrains[terrains.Count - 1].terrain = unityTerrains[i];
            }

            tiles.x = tiles.y = (int)Mathf.Sqrt(terrains.Count);
        }

        public Int2 IndexToTile(int index)
        {
            Int2 tile = new Int2();
            tile.y = index / tiles.x;
            tile.x = index - (tile.y * tiles.x);
            return tile;
        }

        public int TileToIndex(Int2 tile)
        {
            return (tile.y * tiles.x) + tile.x;
        }

        public void CreateTerrains()
        { 
            TerrainData terrainData;
            GameObject terrainObject;
            Terrain terrain;
            TerrainCollider terrainCollider;

            string tileName;
            
            ClearToOne();

            terrains[0].size = terrainSize;
            int countTerrain = 0;

            index = terrains.Count - 1;

            Int2 tile = IndexToTile(terrainSelect);

            for (int y = 0; y < selectTiles.y; ++y)
            {
                for (int x = 0; x < selectTiles.x; ++x)
                {
                    // {
                    // }
                    // else if (terrains[countTerrain].terrain != null) { ++countTerrain; continue; }

                    if (countTerrain != 0)
                    {
                        terrains.Add(new TCUnityTerrain());
                        terrains[countTerrain] = CopyTerrain(terrains[0]);
                    }

                    terrains[countTerrain].tileX = x;
                    terrains[countTerrain].tileZ = y;

                    terrainObject = new GameObject();
                    terrainObject.transform.parent = transform;
                    // terrainObject.transform.hideFlags = HideFlags.NotEditable;
                    terrain = (Terrain)terrainObject.AddComponent(typeof(Terrain));
                    terrainCollider = (TerrainCollider)terrainObject.AddComponent(typeof(TerrainCollider));

#if !UNITY_5 && !UNITY_2017 && !UNITY_2018_1 && !UNITY_2018_2
                    terrain.allowAutoConnect = true;
#endif

                    tileName = "_x" + x.ToString() + "_y" + y.ToString();
                    terrain.name = terrainName + tileName;

                    terrainData = new TerrainData();
                    terrainData.size = terrains[0].size;
                    terrain.terrainData = terrainData;
                    terrainCollider.terrainData = terrainData;

                    terrains[countTerrain].terrain = terrain;
                    terrains[countTerrain].ApplyAllSettings(terrains[0], settingsEditor);
                    terrains[countTerrain].terrainSettingsScript = terrain.gameObject.AddComponent<TC_TerrainSettings>();

                    // script.set_terrain_settings(terrainArea.terrains[index], "(siz)(res)");
                    //if (script.settings.copy_terrain_material)
                    //{
                    //    terrain.materialTemplate = terrainArea.terrains[0].terrain.materialTemplate;
                    //}

#if UNITY_EDITOR
                    string path = "Assets" + terrainDataPath.Replace(Application.dataPath, String.Empty);
                    path += "/" + terrain.name + ".asset";
                    UnityEditor.AssetDatabase.DeleteAsset(path);
                    UnityEditor.AssetDatabase.CreateAsset(terrainData, path);
#endif

                    // Debug.Log(terrains[countTerrain].splatPrototypes.Count);

                    terrains[countTerrain].ApplySplatTextures(this, null);
                    terrains[countTerrain].ApplyTrees();
                    terrains[countTerrain].ApplyGrass();

                    terrains[countTerrain].GetSettings(settingsEditor);

                    // script.set_terrain_parameters(terrainArea.terrains[index], terrainArea.terrains[0]);
                    // if (terrainArea.terrains[0].rtp_script) { script.assign_rtp_single(terrainArea.terrains[index]); }
                    ++countTerrain;
                }
            }

            AssignMaterials();
            if (terrains[0].terrain != null) TC_Settings.instance.masterTerrain = terrains[0].terrain;

            // script.set_all_terrain_area(script.terrains[0]);
            // script.set_terrainArea_splat_textures(terrainArea, terrainArea.terrains[0]);
            // script.assign_all_terrain_splat_alpha();
            // script.set_terrainArea_trees(terrainArea, terrainArea.terrains[0]);
            // script.set_terrainArea_details(terrainArea, terrainArea.terrains[0]);

            tiles = selectTiles;
            FitTerrainsPosition();
            CalcTotalResolutions();

            terrainSelect = TileToIndex(tile);
            if (terrainSelect > tiles.x * tiles.y) terrainSelect = 0;
        }

        public TCUnityTerrain CopyTerrain(TCUnityTerrain tcTerrain)
        {
            GameObject go = new GameObject();
            SaveTemplate s = go.AddComponent<SaveTemplate>();
            s.tcTerrain = tcTerrain;

            GameObject go2 = (GameObject)Instantiate(go);
            DestroyImmediate(go);
            s = go2.GetComponent<SaveTemplate>();
            tcTerrain = s.tcTerrain;
            tcTerrain.rtHeight = null;
            tcTerrain.texHeight = null;
            tcTerrain.texColormap = null;
            // tcTerrain.materialTemplate = null;
            DestroyImmediate(go2);

            return tcTerrain;
        }

        public void GetAll()
        {
            if (terrains == null) terrains = new List<TCUnityTerrain>();
            if (terrains.Count == 0) terrains.Add(new TCUnityTerrain());

            if (terrainSelect >= terrains.Count) terrainSelect = 0;

            GetSize();
            GetSettings();
            GetSplatTextures();
            GetTrees();
            GetGrass();
            GetResolutions();
        }

        public void GetResolutions()
        {
            terrains[terrainSelect].GetResolutions();
            CalcTotalResolutions();
        }

        public void GetSize()
        {
            if (!terrains[terrainSelect].CheckValidUnityTerrain()) return;
            terrains[terrainSelect].size = terrains[terrainSelect].terrain.terrainData.size;
        }

        public void GetSettings()
        {
            terrains[terrainSelect].GetSettings(settingsEditor);
        }

        public void ApplySize()
        {
            if (applyChanges == ApplyChanges.Terrain) ApplySizeTerrain(); // TODO Remove
            else if (applyChanges == ApplyChanges.TerrainArea) ApplySizeTerrainArea();//TC_Generate.singleton.applyTerrainAreaSize = true;
                                                                                      // else if (applyChanges == ApplyChanges.AllTerrainAreas) GlobalManager.singleton.ApplySizeTerrainAreas();
        }

        public void ApplySizeTerrain()
        {
            if (!terrains[terrainSelect].CheckValidUnityTerrain()) return;
            terrains[terrainSelect].terrain.terrainData.size = terrainSize;
            FitTerrainsPosition();
        }

        public void ApplySizeTerrainArea()
        {
            if (!terrains[terrainSelect].CheckValidUnityTerrain()) return;
            if (!terrains[0].CheckValidUnityTerrain()) return;

            terrains[0].terrain.terrainData.size = terrainSize;
            FitTerrainsPosition();
            // !if (script.settings.colormap) { script.set_colormap(script.settings.colormap, false); }
        }

        public void ApplyResolution()
        {
            // if (applyChanges == ApplyChanges.Terrain) terrains[terrainSelect].ApplyResolutionTerrain(terrains[terrainSelect]);
            // else if (applyChanges == ApplyChanges.TerrainArea)
            ApplyResolutionTerrainArea(terrains[terrainSelect]);
            // else if (applyChanges == ApplyChanges.AllTerrainAreas) TC_Area2D.singleton.ApplyResolutionTerrainAreas(terrains[terrainSelect]);
        }

        public void ApplyResolutionTerrainArea(TCUnityTerrain sTerrain)
        {
            for (int i = 0; i < terrains.Count; i++) terrains[i].ApplyResolutionTerrain(sTerrain);
            CalcTotalResolutions();

            if (TC_Settings.instance.global.showResolutionWarnings)
            {
                if (sTerrain.heightmapResolution > 513) TC.AddMessage("Heightmap resolution is higher than 513, keep in mind that Auto generate will be too slow to work in realtime.");
                if (sTerrain.splatmapResolution > 1024) TC.AddMessage("Splatmap resolution is higher than 1024, keep in mind that Auto generate will be too slow to work in realtime.");
                if (sTerrain.detailResolution > 512) TC.AddMessage("Grass resolution is higher than 513, keep in mind that Auto generate will be too slow to work in realtime.");
            }
        }

        public void ApplySettings()
        {
            if (applyChanges == ApplyChanges.Terrain) terrains[terrainSelect].ApplySettings(terrains[terrainSelect], settingsEditor);
            else if (applyChanges == ApplyChanges.TerrainArea) ApplySettingsTerrainArea();
        }

        public void ApplySettingsTerrainArea()
        {
            for (int i = 0; i < terrains.Count; i++) terrains[i].ApplySettings(terrains[terrainSelect], settingsEditor);
        }

        public void ApplySplatTextures(TCUnityTerrain sTerrain)
        {
            for (int i = 0; i < terrains.Count; i++) terrains[i].ApplySplatTextures(this, sTerrain);
        }

        public void GetSplatTextures()
        {
            for (int i = 0; i < terrains.Count; i++) terrains[i].GetSplatTextures(this);
        }

        public void ApplyTrees()
        {
            for (int i = 0; i < terrains.Count; i++) terrains[i].ApplyTrees(terrains[terrainSelect]);
        }

        public void GetTrees()
        {
            for (int i = 0; i < terrains.Count; i++) terrains[i].GetTrees();
        }

        public void ApplyGrass() { for (int i = 0; i < terrains.Count; i++) terrains[i].ApplyGrass(terrains[terrainSelect]); }
        public void GetGrass() { for (int i = 0; i < terrains.Count; i++) terrains[i].GetGrass(); }

        public void ResetHeightmap() { for (int i = 0; i < terrains.Count; i++) terrains[i].ResetHeightmap(); }
        public void ResetSplatmap() { for (int i = 0; i < terrains.Count; i++) terrains[i].ResetSplatmap(); }
        public void ResetTrees() { for (int i = 0; i < terrains.Count; i++) terrains[i].ResetTrees(); }
        public void ResetGrass() { for (int i = 0; i < terrains.Count; i++) terrains[i].ResetGrass(); }
        public void ResetObjects() { for (int i = 0; i < terrains.Count; i++) terrains[i].ResetObjects(); }
    }
}



