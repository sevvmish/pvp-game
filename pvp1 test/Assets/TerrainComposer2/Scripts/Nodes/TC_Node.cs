using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace TerrainComposer2
{
    public class TC_Node : TC_ItemBehaviour
    {
        public InputKind inputKind;
        public InputTerrain inputTerrain;
        public InputNoise inputNoise;
        public InputShape inputShape;
        public InputFile inputFile;
        public InputCurrent inputCurrent;
        public InputPortal inputPortal;

        public NodeGroupType nodeType;
        public CollisionMode collisionMode;
        public CollisionDirection collisionDirection;

        public BlurMode blurMode;

        public int nodeGroupLevel;

        public ImageWrapMode wrapMode = ImageWrapMode.Repeat;
        public bool clamp;
        public float radius = 300;
        
        public TC_RawImage rawImage;

        public TC_Image image;
        public ImageSettings imageSettings;

        public bool square;
        public int splatSelectIndex;
        public Noise noise;
        public Shapes shapes;
        public int iterations = 1;
        public Vector2 detectRange = new Vector2(1.0f / 255f, 1.0f / 255f);
        public int mipmapLevel = 1;
        public ConvexityMode convexityMode;
        public float convexityStrength = 5;

        public Texture stampTex;
        // public Texture[] texArray;
        public string pathTexStamp = "";
        public bool isStampInResourcesFolder;
        public string resourcesFolder;

        public float posYOld;
        public int collisionMask = -1;
        public bool heightDetectRange;
        public bool includeTerrainHeight = true;

        public Vector2 range;
        public bool useConstant;

        public Vector3 size = new Vector3(2048, 0, 2048);

        [Serializable]
        public class Shapes
        {
            public Vector2 topSize = new Vector2(500, 500);
            public Vector2 bottomSize = new Vector2(1000,1000);
            public float size = 500;
        }

        public override void Awake()
        {
            base.Awake();
            if (rawImage != null) rawImage.referenceCount++;
            if (image != null) image.referenceCount++;
            // Debug.Log("Awake");
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            // Debug.Log("Node " + UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode + " --- "+ UnityEditor.EditorApplication.isPlaying);

            #if UNITY_EDITOR
                if (UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode || UnityEditor.EditorApplication.isPlaying) return;
            #endif

            if (rawImage != null) rawImage.UnregisterReference();
            if (image != null) image.UnregisterReference();
        }

        public void SetDefaultSettings()
        {
            size = TC_Settings.instance.global.defaultTerrainSize;

            if (transform.parent.GetSiblingIndex() == 0)
            {
                inputKind = InputKind.Shape;
                inputShape = InputShape.Circle;
                wrapMode = ImageWrapMode.Clamp;
            }
            else if (outputId == TC.heightOutput)
            {
                inputKind = InputKind.File;
                inputFile = InputFile.RawImage;
                wrapMode = ImageWrapMode.Clamp;
            }
        }

        public void CheckTransformChange()
        {
            // Debug.Log("check tansform change");
            if (ctOld.hasChanged(this))
            {
                TC.AutoGenerate();
                // Debug.Log(t.name);
                ctOld.Copy(this);
            }
        }

        public override void ChangeYPosition(float y)
        {
            #if UNITY_EDITOR
                // UnityEditor.Undo.RecordObject(t, "Edit Transform");
                UnityEditor.Undo.RecordObject(this, "Edit Transform");
            #endif
            posY += y / t.lossyScale.y;
        }

        public void CalcBounds()
        {
            // if (bounds == null) bounds = new Bounds();
            bounds.center = t.position;
            bounds.size = Vector3.Scale(new Vector3(1000, 100, 1000), t.lossyScale);
        }

        public bool OutOfBounds()
        {
            // Debug.Log(bounds + " " + Area2D.current.bounds);
            if (bounds.Intersects(TC_Area2D.instance.bounds)) return false;
            else
            {
                TC_Reporter.Log(name + " Out of bounds!");
                return true;
            }
        }

        public Enum GetInputPopup()
        {
            if (inputKind == InputKind.Terrain)
            {
                if (outputId == TC.heightOutput)
                {
                    InputTerrainHeight inputTerrainHeight = InputTerrainHeight.Collision;
                    return inputTerrainHeight;
                }
                else return inputTerrain;
            }
            else if (inputKind == InputKind.Noise) return inputNoise;
            else if (inputKind == InputKind.Shape) return inputShape;
            else if (inputKind == InputKind.File) return inputFile;
            else if (inputKind == InputKind.Current) return inputCurrent;
            else if (inputKind == InputKind.Portal) return inputPortal;

            return null;
        }

        public void SetInputPopup(Enum popup)
        {
            if (inputKind == InputKind.Terrain) inputTerrain = (InputTerrain)popup;
            else if (inputKind == InputKind.Noise) inputNoise = (InputNoise)popup;
            else if (inputKind == InputKind.Shape) inputShape = (InputShape)popup;
            else if (inputKind == InputKind.File) inputFile = (InputFile)popup;
            else if (inputKind == InputKind.Current) inputCurrent = (InputCurrent)popup;
            // else if (inputKind == InputKind.Portal) inputPortal = (InputPortal)popup;
        }
        
        public void Init()
        {
            // Debug.Log("Init");

            if (inputKind == InputKind.Terrain)
            {
                if (inputTerrain == InputTerrain.Normal) { active = false; }
            }
            if (inputKind == InputKind.Noise)
            {
                if (noise == null) noise = new Noise();
                if (inputNoise == InputNoise.IQ || inputNoise == InputNoise.Swiss || inputNoise == InputNoise.Jordan)
                {
                    if (noise.mode == NoiseMode.TextureLookup) noise.mode = NoiseMode.Normal;
                }
            }
            else if (inputKind == InputKind.Shape)
            {
                if (shapes == null) shapes = new Shapes();
            }
            else if (inputKind == InputKind.File)
            {
                if (inputFile == InputFile.RawImage)
                {
                    if (rawImage != null && rawImage.tex != null && stampTex != null) { active = visible; return; }

                    if (rawImage != null)
                    {
                        if (rawImage.tex == null) rawImage.LoadRawImage(rawImage.path);
                        if (stampTex != null && rawImage.tex != null) { active = visible; return; }
                    }

                    if (stampTex == null)
                    {
                        #if UNITY_EDITOR
                        if (pathTexStamp != "")
                        {
                            stampTex = UnityEditor.AssetDatabase.LoadAssetAtPath(pathTexStamp, typeof(Texture2D)) as Texture2D;
                            if (stampTex == null) { active = false; return; }
                        }
                        #endif
                        if (pathTexStamp == "") { active = false; return; }
                    }

                    if (rawImage == null) DropTextureEditor(stampTex);

                    if (rawImage == null)
                    {
                        active = false; stampTex = null;
                    }
                    else active = visible;
                }
                else if (inputFile == InputFile.Image)
                {
                    if (stampTex == null) active = false;
                }
            }
            
            else if (inputKind == InputKind.Portal)
            {
                if (portalNode != null)
                {
                    if (portalNode.isPortalCount > 0) active = visible; else active = false;
                }
                else active = false;
            }
        }

        public void UpdateVersion()
        {
            if (versionNumber == 0)
            {
                wrapMode = clamp ? ImageWrapMode.Clamp : ImageWrapMode.Repeat;
                size.y = 1024;

                if (inputKind == InputKind.Terrain)
                {
                    if (inputTerrain == InputTerrain.Collision) inputTerrain = InputTerrain.Convexity;
                    else if (inputTerrain == InputTerrain.Splatmap) inputTerrain = InputTerrain.Collision;
                    else if (inputTerrain == InputTerrain.Convexity) inputTerrain = InputTerrain.Splatmap;
                }

                if (inputKind == InputKind.File && inputFile == InputFile.RawImage)
                {
                    t.localScale = new Vector3(t.localScale.x, t.localScale.y, -t.localScale.z);
                }

                if (inputKind == InputKind.Noise)
                {
                    if (inputNoise == InputNoise.Billow) inputNoise = InputNoise.Ridged;
                    else if (inputNoise == InputNoise.Ridged) inputNoise = InputNoise.Billow;
                    else if (inputNoise == InputNoise.IQ) inputNoise = InputNoise.Random;
                }
                
                SetVersionNumber();
            }
        }

        public bool DropTextureEditor(Texture tex)
        {
            #if UNITY_EDITOR
            if (tex != null)
            {
                pathTexStamp = UnityEditor.AssetDatabase.GetAssetPath(tex);
                string path = pathTexStamp;
                int index = path.LastIndexOf("/");
                path = path.Insert(index, "/RawFiles");

                index = path.IndexOf("/Resources/");
                isStampInResourcesFolder = (index != -1);
                
                if (isStampInResourcesFolder)
                {
                    path = path.Substring(index + 11);
                    path = path.Remove(path.Length - 4);
                    resourcesFolder = path;
                    // Debug.Log(path);
                }
                else
                {
                    path = path.Remove(path.Length - 3) + "raw";
                    
                    if (!TC.FileExistsPath(path))
                    {
                        path = path.Remove(path.Length - 3) + "r16";
                    }

                    if (!TC.FileExistsPath(path))
                    {
                        // TC.AddMessage("Cannot find the file " + path.Remove(path.Length - 3, 3) + "\n\nThe file extension needs to be .raw or .r16");
                        if (rawImage != null) rawImage.UnregisterReference();
                        inputFile = InputFile.Image;
                        stampTex = tex;
                        TC.AutoGenerate();
                        return false;
                    }
                }
                
                TC_RawImage oldRawImage = rawImage;
                if (oldRawImage) oldRawImage.UnregisterReference();

                // Debug.Log(path);

                rawImage = TC_Settings.instance.AddRawFile(path, isStampInResourcesFolder);
            }
            #else
                if (isStampInResourcesFolder)
                {
                    rawImage = TC_Settings.instance.AddRawFile(resourcesFolder, isStampInResourcesFolder);
                }
            #endif
                
            if (rawImage != null)
            {
                stampTex = tex;
                TC.RefreshOutputReferences(outputId, true);

                // TC_Reporter.Log(path);
                TC_Reporter.Log("Node index " + rawImage.name);
                return true;
            }
            else
            {
                TC.AddMessage("This is not a stamp preview image.\n\nThe raw heightmap file needs to be placed in a 'RawFiles' folder, then TC2 will automatically make a preview image one folder before it.\nThis image needs to be used for dropping on the node.", 0, 4);
            }

            return false;
        }

    }

    [Serializable]
    public class Noise
    {
        public NoiseMode mode;
        public CellNoiseMode cellMode;
        public float frequency = 100f;
        public float lacunarity = 2.0f;
        public int octaves = 6;
        public float persistence = 0.5f;
        public float seed = 0;
        
        public float amplitude = 7f;
        public float warp0 = 0.5f;
        public float warp = 0.25f;
        public float damp0 = 0.8f;
        public float damp = 1.0f;
        public float dampScale = 1.0f;

        public int cellType = 1;
        public int distanceFunction = 1;
    }

    [Serializable]
    public class ImageSettings
    {
        const int red = 0, green = 1, blue = 2, alpha = 3;

        public ColorSelectMode colSelectMode = ColorSelectMode.ColorRange;
        public ColChannel[] colChannels;
        public Int2 tiles = new Int2(1, 1);


        public ImageSettings()
        {
            colChannels = new ColChannel[4];
            for (int i = 0; i < 4; i++) colChannels[i] = new ColChannel();
            colChannels[3].active = false;
        }
        
        [Serializable]
        public class ColChannel
        {
            public bool active = true;
            public Vector2 range = new Vector2(0, 255);
        }
    }
}