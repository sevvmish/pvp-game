using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System;

namespace TerrainComposer2
{
    [System.Serializable]
    public class TC_Terrain
    {
        public Transform t;
        // public ReliefTerrain rtp;
        public Vector3 newPosition;
        public int tasks;
        public TC_Node[] nodes;
        public Material rtpMat;

        public RenderTexture rtHeight;
        public Texture2D texHeight;
        public Texture2D texColormap;
        public Texture2D texNormalmap;
        

        // public Texture2D texHeight;

        // [HideInInspector] public byte[] treemap;
        // [HideInInspector] public byte[] objectmap;

        public void DisposeTextures()
        {
            TC_Compute.DisposeRenderTexture(ref rtHeight);
            TC_Compute.DisposeTexture(ref texHeight);
            TC_Compute.DisposeTexture(ref texColormap);
            TC_Compute.DisposeTexture(ref texNormalmap);
        }

        public void SetNodesActive(bool active)
        {
            if (nodes == null) return;
            for (int i = 0; i < nodes.Length; i++)
            {
                nodes[i].autoGenerate = false;
                nodes[i].t.position = t.position;
                nodes[i].gameObject.SetActive(active);
            }
        }

        public void Init()
        {
            if (rtpMat == null)
            {
                MeshRenderer mr = t.GetComponent<MeshRenderer>();
                if (mr != null) rtpMat = mr.sharedMaterial;
            }
        }
    }

    [System.Serializable]
    public class MeshTerrain : TC_Terrain
    {
        public MeshTerrain(Transform t)
        {
            this.t = t;
            Init();
        }
    }

    [System.Serializable]
    public class TCUnityTerrain : TC_Terrain
    {
        public bool active = true;
        public int index;
        public int index_old;
        public bool on_row = false;
        public Color color_terrain = new Color(2.0f, 2.0f, 2.0f, 1.0f);
        public int copy_terrain = 0;
        public bool copy_terrain_settings = true;
        public byte generateStatus;

        public Transform objectsParent;
        public Vector3 newPos;
        public bool updateTerrainPos;

        public bool detailSettingsFoldout;
        public bool splatSettingsFoldout;
        public bool treeSettingsFoldout;

        // public RTP_script rtp_script;
        // public neighbor_class neighbor = new neighbor_class();

        // public Texture2D[] splatmaps;
        
        public Terrain terrain;

        public Color[] splatColors;
        public List<TC_SplatPrototype> splatPrototypes = new List<TC_SplatPrototype>();
        public List<TC_TreePrototype> treePrototypes = new List<TC_TreePrototype>();
        public List<TC_DetailPrototype> detailPrototypes = new List<TC_DetailPrototype>();

        public int heightmapResolutionList = 5;
        public int splatmapResolutionList = 4;
        public int basemapResolutionList = 4;
        public int detailResolutionPerPatchList = 1;

        public Vector3 size = new Vector3(1000.0f, 500.0f, 1000.0f);
        public int tileX;
        public int tileZ;

        // resolutions
        public int heightmapResolution = 512;
        public int splatmapResolution = 512;
        public int basemapResolution = 128;
        public int detailResolution = 512;
        public int detailResolutionPerPatch = 32;
        public int appliedResolutionPerPatch;

        public float grassScaleMulti = 1.0f;

        public float heightmapPixelError = 5.0f;
        public int heightmapMaximumLOD = 0;
        #if UNITY_5 || UNITY_2017 || UNITY_2018_1 || UNITY_2018_2 || UNITY_2018_3 || UNITY_2018_4
        public bool castShadows = false;
        #else
        public UnityEngine.Rendering.ShadowCastingMode shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
        #endif
        public float basemapDistance = 20000.0f;
        public float treeDistance = 2000.0f;
        public float detailObjectDistance = 80.0f;
        public float detailObjectDensity = 1.0f;
        public int treeMaximumFullLODCount = 50;
        public float treeBillboardDistance = 50.0f; 
        public float treeCrossFadeLength = 5.0f;

        public bool drawTreesAndFoliage = true;
        public UnityEngine.Rendering.ReflectionProbeUsage reflectionProbeUsage;
        public bool bakeLightProbesForTrees = true;
        public float thickness = 1;
        public float legacyShininess = 0.7812f;
        public Color legacySpecular = new Color(0.5f, 0.5f, 0.5f, 1);
        public TC_TerrainSettings terrainSettingsScript;
        #if UNITY_5 || UNITY_2017 || UNITY_2018_1 || UNITY_2018_2 || UNITY_2018_3 || UNITY_2019_1
        public Terrain.MaterialType materialType;
        #else
        public Material materialTemplate;
        #endif
        public bool drawHeightmap = true;
        public bool collectDetailPatches = true;

        public float wavingGrassSpeed = 0.5f;
        public float wavingGrassAmount = 0.5f;
        public float wavingGrassStrength = 0.5f;
        public Color wavingGrassTint = new Color(0.698f, 0.6f, 0.50f);

        public void AssignTextureRTP(string texName, Texture2D tex)
        {
            Type t = TC.FindRTP();

            if (t == null) return;
            if (terrain == null) return;

            Component c = terrain.GetComponent(t);

            if (c == null) return;
            
            FieldInfo fi = t.GetField(texName);
            if (fi == null) return;
            
            fi.SetValue(c, tex);

            #if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(c);
            #endif
        }
        
        public bool CheckValidUnityTerrain()
        {
            if (terrain == null) return false;
            if (terrain.terrainData == null) return false;
            return true;
        }

        public void AddSplatTexture(int index)
        {
            if (splatPrototypes.Count >= TC.splatLimit) { TC.AddMessage("TC2 supports generating maximum " + TC.splatLimit + " splat textures."); Debug.Log("TC2 supports generating maximum " + TC.splatLimit + " splat textures."); return; }
            splatPrototypes.Insert(index, new TC_SplatPrototype());
        }

        public void EraseSplatTexture(int splat_number)
        {
            if (splatPrototypes.Count > 0) { splatPrototypes.RemoveAt(splat_number); }
        } 
         
        public void ClearSplatTextures()
        {
            splatPrototypes.Clear();
        }

        public void ClearNullSplatPrototypes()
        {
            for (int i = 0; i < splatPrototypes.Count; ++i)
            {
                if (splatPrototypes[i].texture == null) { splatPrototypes.RemoveAt(i); --i; }
            }
        }

        public void AddTreePrototype(int index)
        {
            treePrototypes.Insert(index, new TC_TreePrototype());
        }

        public void EraseTreeProtoType(int tree_number)
        {
            if (treePrototypes.Count > 0) { treePrototypes.RemoveAt(tree_number); }
        }

        public void ClearTreePrototype()
        {
            treePrototypes.Clear();
        }

        public void ClearNullTreePrototype()
        {
            for (int i = 0; i < treePrototypes.Count; ++i)
            {
                if (treePrototypes[i].prefab == null) { treePrototypes.RemoveAt(i); --i; }
            }
        }

        public void AddDetailPrototype(int detail_number)
        {
            if (detailPrototypes.Count >= TC.grassLimit) { TC.AddMessage("TC2 supports generating maximum " + TC.grassLimit + " grass textures."); Debug.Log("TC2 supports generating maximum " + TC.grassLimit + " grass textures."); return; }
            detailPrototypes.Insert(detail_number, new TC_DetailPrototype());
        }

        public void EraseDetailPrototype(int detail_number)
        {
            if (detailPrototypes.Count > 0) { detailPrototypes.RemoveAt(detail_number); }
        }

        public void clear_detailprototype()
        {
            detailPrototypes.Clear();
        }

        public void clear_null_detailprototype()
        {
            for (int i = 0; i < detailPrototypes.Count; ++i)
            {
                if (detailPrototypes[i].prototype == null && detailPrototypes[i].prototypeTexture == null) { detailPrototypes.RemoveAt(i); --i; }
            }
        }

        public void SetTerrainResolutionsToList()
        {
            heightmapResolutionList = 12 - (int)Mathf.Log(heightmapResolution - 1, 2);
            splatmapResolutionList = 11 - (int)Mathf.Log(splatmapResolution, 2);
            basemapResolutionList = 11 - (int)Mathf.Log(basemapResolution, 2);
            detailResolutionPerPatchList = (int)Mathf.Log(detailResolutionPerPatch, 2) - 3;
        }

        public void SetTerrainResolutionFromList()
        {
            heightmapResolution = (int)(Mathf.Pow(2, 12 - heightmapResolutionList) + 1);
            splatmapResolution = (int)Mathf.Pow(2, 11 - splatmapResolutionList);
            basemapResolution = (int)Mathf.Pow(2, 11 - basemapResolutionList);
            detailResolutionPerPatch = (int)Mathf.Pow(2, detailResolutionPerPatchList + 3);
            SetTerrainResolutionsToList();
        }

        public void ApplyAllSettings(TCUnityTerrain terrain, bool settingsEditor)
        {
            ApplyResolutionTerrain(terrain);
            ApplySettings(terrain, settingsEditor);
        }


        public void ApplyResolutionTerrain(TCUnityTerrain sTerrain)
        {
            SetTerrainResolutionFromList();
            if (!CheckValidUnityTerrain()) return;

            //Debug.Log(sTerrain.heightmapResolution);
            //Debug.Log(sTerrain.splatmapResolution);
            //Debug.Log(sTerrain.detailResolution);
            //Debug.Log(sTerrain.detailResolutionPerPatch);
            //Debug.Log(sTerrain.basemapResolution);

            if (terrain.terrainData.heightmapResolution != sTerrain.heightmapResolution)
            {
                Vector3 oldSize = terrain.terrainData.size;
                terrain.terrainData.heightmapResolution = sTerrain.heightmapResolution;
                terrain.terrainData.size = oldSize;
            }
            if (terrain.terrainData.alphamapResolution != sTerrain.splatmapResolution) { terrain.terrainData.alphamapResolution = sTerrain.splatmapResolution; }
            if (terrain.terrainData.baseMapResolution != sTerrain.basemapResolution) terrain.terrainData.baseMapResolution = sTerrain.basemapResolution;

            if (terrain.terrainData.detailResolution != sTerrain.detailResolution || sTerrain.detailResolutionPerPatch != sTerrain.appliedResolutionPerPatch)
            {
                // Debug.Log(sTerrain.detailResolution);
                // Debug.Log(sTerrain.detailResolutionPerPatch);
                
                terrain.terrainData.SetDetailResolution(sTerrain.detailResolution, sTerrain.detailResolutionPerPatch);
                sTerrain.appliedResolutionPerPatch = sTerrain.detailResolutionPerPatch;
            }
        }

        public void GetResolutions()
        {
            if (terrain == null) return;
            if (terrain.terrainData == null) return;
            heightmapResolution = terrain.terrainData.heightmapResolution;
            basemapResolution = terrain.terrainData.baseMapResolution;
            splatmapResolution = terrain.terrainData.alphamapResolution;
            detailResolution = terrain.terrainData.detailResolution;
            SetTerrainResolutionsToList();
        }

        public void GetSettings(bool settingsEditor)
        {
            if (!CheckValidUnityTerrain()) return;

            #if UNITY_5 || UNITY_2017 || UNITY_2018_1 || UNITY_2018_2 || UNITY_2018_3 || UNITY_2019_1
            materialType = terrain.materialType;
            #else
            materialTemplate = terrain.materialTemplate;
            #endif

            basemapDistance = terrain.basemapDistance;
            #if UNITY_5 || UNITY_2017 || UNITY_2018_1 || UNITY_2018_2 || UNITY_2018_3 || UNITY_2018_4
            castShadows = terrain.castShadows;
            #else
            shadowCastingMode = terrain.shadowCastingMode;
            #endif
            treeCrossFadeLength = terrain.treeCrossFadeLength;

            #if UNITY_EDITOR
            bakeLightProbesForTrees = terrain.bakeLightProbesForTrees;
            #endif
            reflectionProbeUsage = terrain.reflectionProbeUsage;
            // thickness = terrain.terrainData.thickness;
            collectDetailPatches = terrain.collectDetailPatches;
            #if UNITY_5 || UNITY_2017 || UNITY_2018_1 || UNITY_2018_2 || UNITY_2018_3 || UNITY_2019_1
            legacyShininess = terrain.legacyShininess;
            legacySpecular = terrain.legacySpecular;
            #endif

            wavingGrassSpeed = terrain.terrainData.wavingGrassSpeed;
            wavingGrassAmount = terrain.terrainData.wavingGrassAmount;
            wavingGrassStrength = terrain.terrainData.wavingGrassStrength;
            wavingGrassTint = terrain.terrainData.wavingGrassTint;

            if (settingsEditor)
            {
                heightmapPixelError = terrain.heightmapPixelError;
                heightmapMaximumLOD = terrain.heightmapMaximumLOD;
                drawTreesAndFoliage = terrain.drawTreesAndFoliage;
                treeDistance = terrain.treeDistance;
                detailObjectDistance = terrain.detailObjectDistance;
                detailObjectDensity = terrain.detailObjectDensity;
                treeBillboardDistance = terrain.treeBillboardDistance;
                treeMaximumFullLODCount = terrain.treeMaximumFullLODCount;

            }
            else
            {
                terrainSettingsScript = terrain.gameObject.GetComponent<TC_TerrainSettings>();
                if (terrainSettingsScript == null)
                {
                    terrainSettingsScript = terrain.gameObject.AddComponent<TC_TerrainSettings>();
                }

                heightmapPixelError = terrainSettingsScript.heightmapPixelError;
                heightmapMaximumLOD = terrainSettingsScript.heightmapMaximumLOD;
                drawTreesAndFoliage = terrainSettingsScript.drawTreesAndFoliage;
                treeDistance = terrainSettingsScript.treeDistance;
                detailObjectDistance = terrainSettingsScript.detailObjectDistance;
                detailObjectDensity = terrainSettingsScript.detailObjectDensity;
                treeBillboardDistance = terrainSettingsScript.treeBillboardDistance;
                treeMaximumFullLODCount = terrainSettingsScript.treeMaximumFullLODCount;
            }
        }

        public void ApplySettings(TCUnityTerrain sTerrain, bool settingsEditor)
        {
            if (!CheckValidUnityTerrain()) return;

            terrain.drawHeightmap = sTerrain.drawHeightmap;
            #if UNITY_EDITOR
                terrain.bakeLightProbesForTrees = sTerrain.bakeLightProbesForTrees;
            #endif
            terrain.collectDetailPatches = sTerrain.collectDetailPatches;
            terrain.reflectionProbeUsage = sTerrain.reflectionProbeUsage;
            #if UNITY_5 || UNITY_2017 || UNITY_2018_1 || UNITY_2018_2 || UNITY_2018_3 || UNITY_2019_1
            terrain.legacyShininess = sTerrain.legacyShininess;
            terrain.legacySpecular = sTerrain.legacySpecular;
            terrain.materialType = sTerrain.materialType;
            #else
            terrain.materialTemplate = sTerrain.materialTemplate;
            #endif

            // terrain.terrainData.thickness = sTerrain.thickness;
            terrain.basemapDistance = sTerrain.basemapDistance;
            #if UNITY_5 || UNITY_2017 || UNITY_2018_1 || UNITY_2018_2 || UNITY_2018_3 || UNITY_2018_4
            terrain.castShadows = sTerrain.castShadows;
            #else
            terrain.shadowCastingMode = sTerrain.shadowCastingMode;
            #endif
            terrain.treeCrossFadeLength = sTerrain.treeCrossFadeLength;
            terrain.terrainData.wavingGrassSpeed = sTerrain.wavingGrassSpeed;
            terrain.terrainData.wavingGrassAmount = sTerrain.wavingGrassAmount;
            terrain.terrainData.wavingGrassStrength = sTerrain.wavingGrassStrength;
            terrain.terrainData.wavingGrassTint = sTerrain.wavingGrassTint;

            if (settingsEditor)
            {
                terrain.drawTreesAndFoliage = sTerrain.drawTreesAndFoliage;

                terrain.heightmapPixelError = sTerrain.heightmapPixelError;
                terrain.heightmapMaximumLOD = sTerrain.heightmapMaximumLOD;
                
                terrain.detailObjectDistance = sTerrain.detailObjectDistance;
                terrain.detailObjectDensity = sTerrain.detailObjectDensity;
                terrain.treeDistance = sTerrain.treeDistance;
                terrain.treeBillboardDistance = sTerrain.treeBillboardDistance;
                terrain.treeMaximumFullLODCount = sTerrain.treeMaximumFullLODCount;
            }
            else
            {
                if (terrainSettingsScript == null)
                {
                    terrainSettingsScript = terrain.gameObject.GetComponent<TC_TerrainSettings>();

                    if (terrainSettingsScript == null) terrainSettingsScript = terrain.gameObject.AddComponent<TC_TerrainSettings>();
                }

                terrainSettingsScript.heightmapPixelError = sTerrain.heightmapPixelError;
                terrainSettingsScript.heightmapMaximumLOD = sTerrain.heightmapMaximumLOD;
                terrainSettingsScript.drawTreesAndFoliage = sTerrain.drawTreesAndFoliage;
                terrainSettingsScript.treeDistance = sTerrain.treeDistance;
                terrainSettingsScript.detailObjectDistance = sTerrain.detailObjectDistance;
                terrainSettingsScript.detailObjectDensity = sTerrain.detailObjectDensity;
                terrainSettingsScript.treeBillboardDistance = sTerrain.treeBillboardDistance;
                terrainSettingsScript.treeMaximumFullLODCount = sTerrain.treeMaximumFullLODCount;
            }

        }

        public void SwapSplatTexture(int index1, int index2)
        {
            if (index2 > -1 && index2 < splatPrototypes.Count)
            {
                TC_SplatPrototype splatPrototype2 = splatPrototypes[index1];
                splatPrototypes[index1] = splatPrototypes[index2];
                splatPrototypes[index2] = splatPrototype2;
            }
        }

        public void ApplySplatTextures(TC_TerrainArea terrainArea, TCUnityTerrain sTerrain = null)
        {
            if (!CheckValidUnityTerrain()) return;
            if (sTerrain == null) sTerrain = this;

            // CleanSplatTextures(sTerrain);
            
            #if UNITY_5 || UNITY_2017 || UNITY_2018_1 || UNITY_2018_2
                List<SplatPrototype> splatPrototypesCleaned = new List<SplatPrototype>();
                bool tooManySplatsMessage = false;

                for (int i = 0; i < sTerrain.splatPrototypes.Count; i++)
                {
                    if (splatPrototypesCleaned.Count >= TC.splatLimit) { tooManySplatsMessage = true; break; }

                    TC_SplatPrototype s = sTerrain.splatPrototypes[i];

                    if (s.texture != null)
                    {
                        SplatPrototype d = new SplatPrototype();
                        d.texture = s.texture;
                        d.normalMap = s.normalMap;
                        d.metallic = s.metallic;
                        d.smoothness = s.smoothness;
                        d.tileOffset = s.tileOffset;
                        float tileSize = sTerrain.terrain.terrainData.size.x / Mathf.Round(sTerrain.terrain.terrainData.size.x / s.tileSize.x);
                        d.tileSize = new Vector2(tileSize, tileSize);

                        splatPrototypesCleaned.Add(d);
                        TC.SetTextureReadWrite(s.texture);
                    }
                }

                if (tooManySplatsMessage) { TC.AddMessage("TC2 supports generating maximum " + TC.splatLimit + " splat textures."); Debug.Log("TC2 supports generating maximum " + TC.splatLimit + " splat textures."); }

            
                terrain.terrainData.splatPrototypes = splatPrototypesCleaned.ToArray();
            #else
                List<TerrainLayer> splatPrototypesCleaned = new List<TerrainLayer>();
                bool tooManySplatsMessage = false;
            
                for (int i = 0; i < sTerrain.splatPrototypes.Count; i++)
                {
                    if (splatPrototypesCleaned.Count >= TC.splatLimit) { tooManySplatsMessage = true; break; }

                    TC_SplatPrototype s = sTerrain.splatPrototypes[i];
    
                    if (s.texture != null)
                    {
                        TerrainLayer d = new TerrainLayer();
                        d.diffuseTexture = s.texture;
                        d.normalMapTexture = s.normalMap;
                        d.normalScale = s.normalScale;
                        d.metallic = s.metallic;
                        d.smoothness = s.smoothness;
                        d.tileOffset = s.tileOffset;
                        float tileSize = sTerrain.terrain.terrainData.size.x / Mathf.Round(sTerrain.terrain.terrainData.size.x / s.tileSize.x);
                        d.tileSize = new Vector2(tileSize, tileSize);
                    
                        splatPrototypesCleaned.Add(d);
                        TC.SetTextureReadWrite(s.texture);
                    }
                }

                if (tooManySplatsMessage) { TC.AddMessage("TC2 supports generating maximum " + TC.splatLimit+ " splat textures."); Debug.Log("TC2 supports generating maximum " + TC.splatLimit +" splat textures."); }

                TerrainLayer[] newTerrainLayers = splatPrototypesCleaned.ToArray();
                
                #if UNITY_EDITOR
                string terrainPath = "Assets" + terrainArea.terrainDataPath.Replace(Application.dataPath, String.Empty);

                for (int i = 0; i < newTerrainLayers.Length; i++)
                {
                    string path;
                    if (terrainArea.useSameTerrainLayersForAllTerrains) path = terrainPath + "/" + terrainArea.terrainName + "_TerrainLayer_" + i + ".asset";
                    else path = terrainPath + "/" + terrain.name + "_TerrainLayer_" + i + ".asset";

                    TerrainLayer terrainLayer = UnityEditor.AssetDatabase.LoadAssetAtPath(path, typeof(TerrainLayer)) as TerrainLayer;
                    
                    if (terrainLayer == null)
                    {
                        UnityEditor.AssetDatabase.DeleteAsset(path);
                        UnityEditor.AssetDatabase.CreateAsset(newTerrainLayers[i], path);
                    }
                    else
                    {
                        CopyTerrainLayer(newTerrainLayers[i], terrainLayer);
                        newTerrainLayers[i] = terrainLayer;
                    }
                }
                #endif

                terrain.terrainData.terrainLayers = newTerrainLayers;
                #endif
        }

        #if !UNITY_5 && !UNITY_2017 && !UNITY_2018_1 && !UNITY_2018_2
        public void CopyTerrainLayer(TerrainLayer s, TerrainLayer d)
        {
            d.diffuseRemapMax = s.diffuseRemapMax;
            d.diffuseRemapMin = s.diffuseRemapMin;
            d.diffuseTexture = s.diffuseTexture;
            d.maskMapRemapMax = s.maskMapRemapMax;
            d.maskMapRemapMin = s.maskMapRemapMin;
            d.maskMapTexture = s.maskMapTexture;
            d.metallic = s.metallic;
            d.name = s.name;
            d.normalMapTexture = s.normalMapTexture;
            d.normalScale = s.normalScale;
            d.smoothness = s.smoothness;
            d.specular = s.specular;
            d.tileOffset = s.tileOffset;
            d.tileSize = s.tileSize;
        }
        #endif

        public void CleanSplatTextures(TCUnityTerrain sTerrain = null)
        {
            if (sTerrain == null) sTerrain = this;
            for (int i = 0; i < sTerrain.splatPrototypes.Count; i++) if (sTerrain.splatPrototypes[i].texture == null) { sTerrain.EraseSplatTexture(i); --i; }
        }

        public void GetSize()
        {
            if (terrain == null) return;
            size = terrain.terrainData.size;
        }

        public void GetSplatTextures(TC_TerrainArea terrainArea)
        {
            if (!CheckValidUnityTerrain()) return;
            splatPrototypes.Clear();

#if UNITY_5 || UNITY_2017 || UNITY_2018_1 || UNITY_2018_2
                SplatPrototype[] terrainSplatPrototypes = terrain.terrainData.splatPrototypes;
                for (int i = 0; i < terrainSplatPrototypes.Length; i++)
                {
                    SplatPrototype s = terrainSplatPrototypes[i];
                    TC_SplatPrototype d = new TC_SplatPrototype();
                    d.texture = s.texture;
                    d.normalMap = s.normalMap;
                    d.metallic = s.metallic;
                    d.smoothness = s.smoothness;
                    d.tileOffset = s.tileOffset;
                    d.tileSize = s.tileSize;
                    splatPrototypes.Add(d);
                }
#else
                TerrainLayer[] terrainLayers = terrain.terrainData.terrainLayers;
                if (terrainLayers == null) return;

                for (int i = 0; i < terrainLayers.Length; i++)
                {
                    TerrainLayer s = terrainLayers[i];
                    if (s == null) continue;

                TC_SplatPrototype d = new TC_SplatPrototype();
                    d.texture = s.diffuseTexture;
                    d.normalMap = s.normalMapTexture;
                    d.metallic = s.metallic;
                    d.smoothness = s.smoothness;
                    d.tileOffset = s.tileOffset;
                    d.tileSize = s.tileSize;
                    splatPrototypes.Add(d);
                }
#endif

            // if (splatColors == null) splatColors = new Color[splatPrototypes.Count];
            // if (splatColors.Length != splatPrototypes.Count) splatColors = new Color[splatPrototypes.Count];
            // Debug.Log("Getsplat texture colors");
            // for (int i = 0; i < splatColors.Length; i++)
            // {
            //     if (splatPrototypes[i].texture != null) splatColors[i] = GetTextureColor(splatPrototypes[i].texture, 1);
            // }
        }

        public void CopyTree(TC_TreePrototype treePrototype1, TC_TreePrototype treePrototype2)
        {
            treePrototype2.prefab = treePrototype1.prefab;
            treePrototype2.bendFactor = treePrototype1.bendFactor;
        }

        public void ApplyTrees(TCUnityTerrain sTerrain = null)
        {
            if (!CheckValidUnityTerrain()) return;
            if (sTerrain == null) sTerrain = this;

            if (sTerrain.treePrototypes.Count == 0) ResetTrees(); 

            List<TreePrototype> treePrototypesCleaned = new List<TreePrototype>();
            for (int i = 0; i < sTerrain.treePrototypes.Count; i++)
            {
                TC_TreePrototype s = sTerrain.treePrototypes[i];

                if (s.prefab == null) continue;

                TreePrototype d = new TreePrototype();
                d.bendFactor = s.bendFactor;
                d.prefab = s.prefab;
                treePrototypesCleaned.Add(d);
            }
            
            terrain.terrainData.treePrototypes = treePrototypesCleaned.ToArray();
        }

        
        public void GetTrees()
        {
            if (!CheckValidUnityTerrain()) return;
            treePrototypes.Clear();

            TreePrototype[] terrainTreePrototypes = terrain.terrainData.treePrototypes;
            for (int i = 0; i < terrainTreePrototypes.Length; i++)
            {
                TreePrototype s = terrainTreePrototypes[i];
                TC_TreePrototype d = new TC_TreePrototype();
                d.bendFactor = s.bendFactor;
                d.prefab = s.prefab;
                treePrototypes.Add(d);
            }
        }

        public void SwapTree(int index1, int index2)
        {
            if (index2 >= 0 && index2 < treePrototypes.Count)
            {
                TC_TreePrototype treePrototype2 = treePrototypes[index1];

                treePrototypes[index1] = treePrototypes[index2];
                treePrototypes[index2] = treePrototype2;
            }
        }

        public void ApplyGrass(TCUnityTerrain sTerrain = null)
        {
            if (!CheckValidUnityTerrain()) return;
            if (sTerrain == null) sTerrain = this;

            CleanGrassPrototypes(sTerrain);

            List<DetailPrototype> detailPrototypesCleaned = new List<DetailPrototype>();
            float multi = sTerrain.grassScaleMulti;

            bool tooManyGrassMessage = false;

            for (int i = 0; i < sTerrain.detailPrototypes.Count; i++)
            {
                if (detailPrototypesCleaned.Count >= TC.grassLimit) { tooManyGrassMessage = true; break; }

                TC_DetailPrototype s = sTerrain.detailPrototypes[i];
                DetailPrototype d = new DetailPrototype();
                
                d.bendFactor = s.bendFactor;
                d.dryColor = s.dryColor;
                d.healthyColor = s.healthyColor;

                d.minHeight = s.minHeight * multi;
                d.maxHeight = s.maxHeight * multi;
                d.minWidth = s.minWidth * multi;
                d.maxWidth = s.maxWidth * multi;

                d.noiseSpread = s.noiseSpread;
                d.usePrototypeMesh = s.usePrototypeMesh;
                d.prototype = s.prototype;
                d.prototypeTexture = s.prototypeTexture;
                d.renderMode = s.renderMode;
                TC.SetTextureReadWrite(d.prototypeTexture);
                detailPrototypesCleaned.Add(d);
            }

            if (tooManyGrassMessage) { TC.AddMessage("TC2 supports generating maximum " + TC.grassLimit + " grass textures."); Debug.Log("TC2 supports generating maximum " + TC.grassLimit + " grass textures."); }

            terrain.terrainData.detailPrototypes = detailPrototypesCleaned.ToArray();
        }

        public void CleanGrassPrototypes(TCUnityTerrain sTerrain)
        {
            if (sTerrain == null) sTerrain = this;

            for (int i = 0; i < sTerrain.detailPrototypes.Count; i++) if (sTerrain.detailPrototypes[i].prototypeTexture == null && sTerrain.detailPrototypes[i].prototype == null) { sTerrain.EraseDetailPrototype(i); --i; }
        }

        public void GetGrass()
        {
            if (!CheckValidUnityTerrain()) return;
            detailPrototypes.Clear();

            DetailPrototype[] terrainDetailPrototypes = terrain.terrainData.detailPrototypes;
            for (int i = 0; i < terrainDetailPrototypes.Length; i++)
            {
                DetailPrototype s = terrainDetailPrototypes[i];
                TC_DetailPrototype d = new TC_DetailPrototype();
                
                d.minHeight = s.minHeight / grassScaleMulti;
                d.minWidth = s.minWidth / grassScaleMulti;
                d.maxHeight = s.maxHeight / grassScaleMulti;
                d.maxWidth = s.maxWidth / grassScaleMulti;
                d.bendFactor = s.bendFactor;
                d.dryColor = s.dryColor;
                d.healthyColor = s.healthyColor;
                d.noiseSpread = s.noiseSpread;
                d.usePrototypeMesh = s.usePrototypeMesh;
                d.prototype = s.prototype;
                d.prototypeTexture = s.prototypeTexture;
                d.renderMode = s.renderMode;
                
                detailPrototypes.Add(d);
            }
        }

        public void ResetHeightmap(float[,] heights = null)
        {
            if (terrain == null) return;

            int resolution = terrain.terrainData.heightmapResolution;
            if (heights == null) heights = new float[resolution, resolution];
            terrain.terrainData.SetHeights(0, 0, heights);
            // TC_Compute.instance.RunTerrainTexFromTerrainData(terrain.terrainData, ref rtHeight);
        }

        public void ResetSplatmap(float[,,] splat = null)
        {
            if (terrain == null) return;

            int resolution = terrain.terrainData.alphamapResolution;
            if (splat == null)
            {
                splat = new float[resolution, resolution, terrain.terrainData.alphamapLayers];
                for (int y = 0; y < resolution; ++y)
                {
                    for (int x = 0; x < resolution; ++x) splat[x, y, 0] = 1;
                }
            }
            terrain.terrainData.SetAlphamaps(0, 0, splat);
        }

        public void ResetTrees()
        {
            if (terrain == null) return;

            TreeInstance[] trees = new TreeInstance[0];
            terrain.terrainData.treeInstances = trees;
        }

        public void ResetGrass()
        {
            if (terrain == null) return;

            int resolution = terrain.terrainData.detailResolution;
            int[,] grassmap = new int[resolution, resolution];

            int grassLength = terrain.terrainData.detailPrototypes.Length;

            for (int i = 0; i < grassLength; i++) terrain.terrainData.SetDetailLayer(0, 0, i, grassmap);
        }

        public void ResetObjects()
        {
            if (objectsParent == null) return;
            TC.DestroyChildrenTransform(objectsParent);
        }

        static public Color GetTextureColor(Texture2D tex, int scanPixelCount)
        {
            TC.SetTextureReadWrite(tex);

            Color32[] colors = tex.GetPixels32();

            double r, b, g;
            r = b = g = 0;

            scanPixelCount *= scanPixelCount;

            int length = colors.Length / scanPixelCount;

            int total = 0;

            for (int i = 0; i < colors.Length; i += length)
            {
                r += colors[i].r / 255.0f;
                g += colors[i].g / 255.0f;
                b += colors[i].b / 255.0f;
                ++total;
            }
            
            return new Color((float)(r / total),(float)(g / total),(float)(b / total), 1);
        }
    }

    [System.Serializable]
    public class TC_SplatPrototype
    {
        public Texture2D texture;
        public Texture2D normalMap;
#if !UNITY_5 && !UNITY_2017 && !UNITY_2018_1 && !UNITY_2018_2
        public float normalScale = 1;
#endif
        public float metallic;
        public float smoothness;
        public Vector2 tileOffset;
        public Vector2 tileSize = new Vector2(15, 15);
    }

    [System.Serializable]
    public class TC_TreePrototype
    {
        public GameObject prefab;
        public float bendFactor = 0.5f;
    }

    [System.Serializable]
    public class TC_DetailPrototype
    {
        public bool usePrototypeMesh = false;
        public float bendFactor = 0.1f;
        public Color dryColor = new Color(0.8039216f, 0.7372549f, 0.101960786f, 1f);
        public Color healthyColor = Color.white;
        public float maxHeight = 2;
        public float maxWidth = 2;
        public float minHeight = 1;
        public float minWidth = 1;
        public float noiseSpread = 0.1f;
        public GameObject prototype;
        public Texture2D prototypeTexture;
        public DetailRenderMode renderMode = DetailRenderMode.Grass;
    }
}