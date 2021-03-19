using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;

namespace TerrainComposer2
{
    [ExecuteInEditMode]
    public class TC_Settings : MonoBehaviour
    {
        public enum ImageExportFormat { JPG, PNG }

        static public TC_Settings instance;

        // Node Window
        public float version;
        public Vector2 scrollOffset = Vector2.zero;
        public Vector2 scrollAdd = Vector2.zero;
        public float scale = 1;

        public bool drawDefaultInspector;
        public bool debugMode = true;
        public bool hideTerrainGroup = true;
        public bool useTCRuntime = false;
        public float defaultTerrainHeight = 1000;
        public Vector3 generateOffset;
        public float resExpandBorderPercentage = 0.0625f;

        public bool showFps = true;
        public bool hideMenuBar = false;
        public Transform dustbinT;
        public Transform selectionOld;
        public Terrain masterTerrain;
        public bool hasMasterTerrain;
        public PresetMode presetMode;
        public float seed = 0;
        
        public string lastPath = "";

        public bool preview;
        public int previewResolution = 128;

        public ImageExportFormat imageExportFormat;
        public bool combineHeightmapImage = true;
        public string heightmapFilename = "Heightmap";
        public string normalmapFilename = "Normalmap";
        public string splatmapFilename = "Splatmap";
        public string colormapFilename = "Colormap";

        public float normalmapStrength = 0.5f;

        public List<TC_RawImage> rawFiles = new List<TC_RawImage>();
        public List<TC_Image> imageList = new List<TC_Image>();

        public Int2 importTiles = new Int2(1, 1);
        public enum ImportSource { TC2_TerrainArea, TerrainData_Files };
        public ImportSource importSource;
        public List<TerrainData> importTerrains = new List<TerrainData>();

        public bool isRTPDetected;
        public bool autoNormalmapRTP = true, autoColormapRTP = true;

        public TC_GlobalSettings global;
        
        public string exportPath;

        void Awake()
        {
            if (instance == null) instance = this;
            if (exportPath == "") exportPath = Application.dataPath;
        }

        void OnEnable()
        {
            if (instance == null) instance = this;
            if (transform.parent != null) transform.parent.hideFlags = HideFlags.NotEditable;
        }

        void OnDestroy()
        {
            TC_Reporter.Log("OnDestroy");
            if (instance == this) instance = null;
        }

        static public void GetInstance()
        {
            GameObject go = GameObject.Find("Settings");
            if (go != null)
            {
                instance = go.GetComponent<TC_Settings>();
            }
        }

        public void DisposeTextures()
        {
            for (int i = 0; i < rawFiles.Count; i++)
            {
                if (rawFiles[i] != null)
                {
                    #if UNITY_EDITOR
                        DestroyImmediate(rawFiles[i].gameObject);
                    #else
                        Destroy(rawFiles[i].gameObject);
                    #endif
                }
            }
        }

        public void HasMasterTerrain()
        {
            if (masterTerrain == null)
            {
                TC_Area2D area2D = TC_Area2D.instance;

                if (area2D.currentTerrainArea != null)
                {
                    if (area2D.currentTerrainArea.terrains.Count > 0)
                    {
                        masterTerrain = area2D.currentTerrainArea.terrains[0].terrain;
                    }
                }
                
                if (masterTerrain == null)
                {
                    hasMasterTerrain = false;
                    return;
                }
            }
            if (masterTerrain.terrainData == null) { hasMasterTerrain = false; return; }
            hasMasterTerrain = true;
        }

        public TC_RawImage AddRawFile(string fullPath, bool isResourcesFolder)
        {
            for (int i = 0; i < rawFiles.Count; i++)
            {
                if (rawFiles[i] == null) { rawFiles.RemoveAt(i); i--; continue; }
                
                if (rawFiles[i].path == fullPath)
                {
                    ++rawFiles[i].referenceCount;

                    if (rawFiles[i].tex == null) rawFiles[i].LoadRawImage(fullPath);
                    return rawFiles[i];
                }
            }

            string label = TC.GetFileName(fullPath);

            GameObject go = new GameObject(label);
            go.transform.parent = transform;
            TC_RawImage rawImage = go.AddComponent<TC_RawImage>();

            // Debug.Log(fullPath);
            rawImage.isResourcesFolder = isResourcesFolder;
            rawImage.LoadRawImage(fullPath);
            rawImage.referenceCount = 1;
            rawFiles.Add(rawImage);

            return rawImage;
        }

        public TC_Image AddImage(Texture2D tex)
        {
            TC_Image image;
            for (int i = 0; i < imageList.Count; i++)
            {
                image = imageList[i];
                // if (image.tex == tex) return image;
            }

            image = new TC_Image();
            // image.tex = tex;

            imageList.Add(image);
            return image;
        }
        
        public void CreateDustbin()
        {
            GameObject go = new GameObject("Dustbin");

            dustbinT = go.transform;
            dustbinT.parent = transform.parent;
        }

        static public Transform GetTransformFromLevel(int index, Transform t)
        {
            Transform root = t.root;
            List<Transform> transforms = new List<Transform>();
            Transform parent = t;

            do
            {
                parent = parent.parent;
                transforms.Insert(0, parent);
            }
            while (parent != root);

            return transforms[index];
        }
    }

    [Serializable]
    public class CachedTransform
    {
        public Vector3 position;
        public Vector3 posOffset;
        public Quaternion rotation;
        public Vector3 scale;
        public float positionYOld;

        public void CopySpecial(TC_ItemBehaviour item)
        {
            TC_Node node = item as TC_Node;

            bool maskNode = false;

            if (node != null)
            {
                if (node.nodeType == NodeGroupType.Mask) maskNode = true;
            }

            posOffset = item.posOffset;

            bool lockPosParent = item.lockPosParent;

            if (item.lockTransform || lockPosParent)
            {
                Vector3 posTemp = Vector3.zero;
                Quaternion rotTemp;
                Vector3 scaleTemp = Vector3.zero;

                if (!(item.lockPosX || lockPosParent)) posTemp.x = item.t.position.x; else posTemp.x = position.x;
                if (!(item.lockPosY || lockPosParent)) posTemp.y = item.posY * scale.y; else posTemp.y = position.y;
                if (!(item.lockPosZ || lockPosParent)) posTemp.z = item.t.position.z; else posTemp.z = position.z;

                if (!(item.lockRotY && item.lockTransform)) rotTemp = Quaternion.Euler(0, item.t.eulerAngles.y, 0); else rotTemp = rotation;

                if (!(item.lockScaleX && item.lockTransform)) scaleTemp.x = item.t.lossyScale.x; else scaleTemp.x = scale.x;
                if (!(item.lockScaleY && item.lockTransform))
                {
                    if (maskNode) scaleTemp.y = item.t.localScale.y; else scaleTemp.y = item.t.lossyScale.y * item.opacity;
                }
                else scaleTemp.y = scale.y;
                if (!(item.lockScaleZ && item.lockTransform)) scaleTemp.z = item.t.lossyScale.z; else scaleTemp.z = scale.z;

                position = posTemp;
                rotation = rotTemp;
                scale = scaleTemp;

                if (item.t.position != position) item.t.position = position;
                if (item.t.rotation != rotation) item.t.rotation = rotation;

                item.t.hasChanged = false;
            }
            else
            {
                rotation = Quaternion.Euler(0, item.t.eulerAngles.y, 0);
                scale.x = item.t.lossyScale.x;
                scale.z = item.t.lossyScale.z;

                if (maskNode) scale.y = item.t.localScale.y; else scale.y = item.t.lossyScale.y;

                scale.y *= item.opacity;
                position = item.t.position;
                position.y = item.posY * scale.y;
            }

            // if (scale.x == 0) scale.x = 1;
            // if (scale.y == 0) scale.y = 1;
            // if (scale.z == 0) scale.z = 1;
        }

        public void Copy(TC_ItemBehaviour item)
        {
            position.x = item.t.position.x;
            position.z = item.t.position.z;
            posOffset = item.posOffset;
            rotation = item.t.rotation;
            scale = item.t.lossyScale;
            positionYOld = item.posY;
        }

        public void Copy(Transform t)
        {
            position = t.position;
            rotation = t.rotation;
            scale = t.lossyScale;
        }

        public bool hasChanged(Transform t)
        {
            if (t == null) return false;
            if (position != t.position || rotation != t.rotation || scale != t.lossyScale) return true; else return false;
        }

        public bool hasChanged(TC_ItemBehaviour item)
        {
            // Debug.Log(item.posY + " " + posYOld);
            if (position.x != item.t.position.x || position.z != item.t.position.z || item.posY != positionYOld)
            {
                // if (position.x != item.t.position.x) Debug.Log("x " + position.x + ", " + item.t.position.x);
                // if (position.y != item.t.position.y) Debug.Log("y " + position.y + ", " + item.t.position.y);
                // if (position.z != item.t.position.z) Debug.Log("z " + position.z + ", " + item.t.position.z);

                // if (positionYOld != item.positionY) Debug.Log("posY " + positionYOld + ", " + item.positionY);
                // Debug.Log(TC.outputNames[item.outputId]);
                return true;
            }
            // if (position != item.t.position) return false;
            if (rotation != item.t.rotation) return true;
            if (scale != item.t.lossyScale) return true;

            return false;
        }
    }
}
