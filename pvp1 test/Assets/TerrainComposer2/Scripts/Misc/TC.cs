using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace TerrainComposer2
{
	static public class TC
	{
		static private int refreshOutputReferences;
		static public bool refreshPreviewImages;
		static public bool repaintNodeWindow;
		static public List<MessageCommand> messages = new List<MessageCommand>();
		static public float autoGenerateCallTimeStart;
		public const int splatLimit = 16;
		public const int grassLimit = 16;

		public const int heightOutput = 0;
		public const int splatOutput = 1;
		public const int colorOutput = 2;
		public const int treeOutput = 3;
		public const int grassOutput = 4;
		public const int objectOutput = 5;
		public const int allOutput = 6;

		public const int nodeLabelLength = 19;

		static readonly public string[] outputNames = new string[6] { "Height", "Splat", "Color", "Tree", "Grass", "Object" };
		static readonly public string[] colChannelNames = new string[4] { "Red", "Green", "Blue", "Alpha" };
		static readonly public string[] colChannelNamesLowerCase = new string[4] { "red", "green", "blue", "alpha" };
		static readonly public Color[] colChannel = new Color[4] { new Color(1, 0.60f, 0.60f, 1), new Color(0.60f, 1, 0.60f, 1), new Color(0.60f, 0.60f, 1, 1), new Color(1, 1, 1, 1) };

		static public string installPath;
		static public string fullInstallPath;

		static public Type FindRTP()
		{
			Type t = Type.GetType("ReliefTerrain");

			TC_Settings.instance.isRTPDetected = t != null ? true : false;

			return t;
		}

		static public float GetVersionNumber()
		{
			return 2.81f;
		}
		
		static public int OutputNameToOutputID(string outputName)
		{
			for (int i = 0; i < outputNames.Length; i++) if (outputName == outputNames[i]) return i;
			return -1;
		}

        static public int GetTerrainSplatTextureLength(Terrain terrain)
        {
            #if UNITY_5 || UNITY_2017 || UNITY_2018_1 || UNITY_2018_2
                return terrain.terrainData.splatPrototypes.Length;
            #else
                return terrain.terrainData.terrainLayers.Length;
            #endif
        }

        static public Texture2D[] GetTerrainSplatTextures(Terrain terrain)
        {
            #if UNITY_5 || UNITY_2017 || UNITY_2018_1 || UNITY_2018_2
                SplatPrototype[] splatPrototypes = terrain.terrainData.splatPrototypes;

                Texture2D[] textures = new Texture2D[splatPrototypes.Length];

                for (int i = 0; i < textures.Length; i++) textures[i] = splatPrototypes[i].texture;
                return textures; 
            #else 
                TerrainLayer[] terrainLayers = terrain.terrainData.terrainLayers;

                Texture2D[] textures = new Texture2D[terrainLayers.Length];
                for (int i = 0; i < textures.Length; i++) textures[i] = terrainLayers[i].diffuseTexture;

                return textures;
            #endif
        }

        static public int GetTerrainSplatTexture(Terrain terrain, int index, out Texture2D texture)
        {
            int length; 

            #if UNITY_5 || UNITY_2017 || UNITY_2018_1 || UNITY_2018_2
                SplatPrototype[] splatPrototypes = terrain.terrainData.splatPrototypes;
                length = splatPrototypes.Length;    

                if (index < splatPrototypes.Length && splatPrototypes[index] != null) texture = splatPrototypes[index].texture;
                else texture = null;

                return length;
            #else 
                TerrainLayer[] terrainLayers = terrain.terrainData.terrainLayers;
                length = terrainLayers.Length;    

                if (index < terrainLayers.Length && terrainLayers[index] != null) texture = terrainLayers[index].diffuseTexture;
                else texture = null;

                return length;
            #endif
        }

		static public void AutoGenerate(bool waitForNextFrame = true)
		{
			AutoGenerate(new Rect (0,0,1,1), waitForNextFrame);
		}

		static public void AutoGenerate(Rect generateRect, bool waitForNextFrame = true)
		{
			// Debug.Log("Auto Generate");

			if (TC_Generate.instance != null) 
			{
				TC_Generate.instance.cmdGenerate = true;
				TC_Generate.instance.autoGenerateRect = Mathw.ClampRect(generateRect, new Rect (0,0,1,1));
			}
		}

		static public void RefreshOutputReferences(int outputId)
		{
			// Debug.Log("Call refresh " + outputId);
			refreshOutputReferences = outputId;
		}

		static public int GetRefreshOutputReferences() { return refreshOutputReferences; }

		static public void RefreshOutputReferences(int outputId, bool autoGenerate)
		{
			// Debug.Log("Call refresh " + outputId);
			refreshOutputReferences = outputId;
			
			if (autoGenerate) AutoGenerate();
		}

		static public void Swap<T>(ref T source, ref T dest)
		{
			T temp = source;
			source = dest;
			dest = temp;
		}

		static public void Swap<T>(List<T> source, int indexS, List<T> dest, int indexD)
		{
			if (indexD < 0 || indexD >= dest.Count) return;

			T temp = source[indexS];
			source[indexS] = dest[indexD];
			dest[indexD] = temp;
		}

		static public void Swap<T>(ref T[] source, ref T[] dest)
		{
			for (int i = 0; i < source.Length; i++) Swap(ref source[i], ref dest[i]);
		}

		static public void InitArray<T>(ref T[] array, int resolution)
		{
			if (array == null) array = new T[resolution];
			else if (array.Length != resolution) array = new T[resolution];
		}

		static public void InitArray<T>(ref T[,] array, int resolutionX, int resolutionY)
		{
			if (array == null) array = new T[resolutionX, resolutionY];
			else if (array.Length != resolutionX * resolutionY) array = new T[resolutionX, resolutionY];
		}

		static public void DestroyChildrenTransform(Transform t)
		{
			int childCount = t.childCount;

			for (int i = 0; i < childCount; i++)
			{
#if UNITY_EDITOR
					UnityEngine.GameObject.DestroyImmediate(t.GetChild(childCount - i - 1).gameObject);
#else
					UnityEngine.GameObject.Destroy(t.GetChild(childCount - i - 1).gameObject);
#endif
			}
		}

		static public void MoveToDustbinChildren(Transform t, int index)
		{
			int childCount = t.childCount;

			if (childCount >= index)
			{
				int length = childCount - index;

				for (int i = 0; i < length; i++) MoveToDustbin(t.GetChild(t.childCount - 1));
			}
		}

		static public void SetTextureReadWrite(Texture2D tex)
		{
			if (tex == null) return;

#if UNITY_EDITOR
			string path = UnityEditor.AssetDatabase.GetAssetPath(tex);
			UnityEditor.TextureImporter textureImporter = (UnityEditor.TextureImporter)UnityEditor.AssetImporter.GetAtPath(path);

			if (textureImporter != null)
			{
				if (!textureImporter.isReadable)
				{
					textureImporter.isReadable = true;
					
					UnityEditor.AssetDatabase.ImportAsset(path, UnityEditor.ImportAssetOptions.ForceUpdate);
				}
			}
#endif
		}

		static public string GetFileName(string path)
		{
			int index = path.LastIndexOf("/");
			if (index != -1)
			{
				string file = path.Substring(index + 1);
				index = file.LastIndexOf(".");

				if (index != -1) return file.Substring(0, index);
				return "";
			}
			return "";
		}

		static public string GetPath(string path)
		{
			int index = path.LastIndexOf("/");
			if (index != -1) return path.Substring(0, index);
			return "";
		}

		static public string GetAssetDatabasePath(string path)
		{
			return path.Replace(Application.dataPath, "Assets");
		}

		static public bool FileExists(string fullPath)
		{
			if (fullPath == null) { Debug.Log("Path doesn't exists."); return false; }
			System.IO.FileInfo file_info = new System.IO.FileInfo(fullPath);
			if (file_info.Exists) return true; else return false;
		}

		static public bool FileExistsPath(string path)
		{
			if (path == null) { Debug.Log("Path doesn't exists."); return false; }
			path = GetApplicationPath() + path;
			// Debug.Log(path);
			System.IO.FileInfo file_info = new System.IO.FileInfo(path);
			if (file_info.Exists) return true; else return false;
		}

		static public string GetApplicationPath()
        {
			string path = Application.dataPath;
			return path.Substring(0, path.Length - 6);
        }

		static public long GetFileLength(string fullPath)
		{
#if !UNITY_WEBPLAYER
			System.IO.FileInfo fileInfo = new System.IO.FileInfo(fullPath);
			return fileInfo.Length;
#else
			return 0;
#endif
		}

		static public void GetInstallPath()
		{
#if UNITY_EDITOR
			if (TC_Settings.instance != null)
			{
				installPath = UnityEditor.AssetDatabase.GetAssetPath(UnityEditor.MonoScript.FromMonoBehaviour(TC_Settings.instance));
				installPath = installPath.Replace("/Scripts/Settings/TC_Settings.cs", "");
			}
#endif
		}

		static public bool LoadGlobalSettings()
		{
#if UNITY_EDITOR

			string file = GetApplicationPath() + installPath + "/Defaults/";
			
			if (!FileExists(file + "TC_GlobalSettings.asset"))
			{
				try
				{
					UnityEditor.FileUtil.CopyFileOrDirectory(file + "TC_GlobalSettingsDefault.asset", file + "TC_GlobalSettings.asset");
				}
				catch (Exception e)
                {
					Debug.LogException(e);
					Debug.LogError("Make sure the TerrainComposer2 folder has write access and that 'Read Only' is disabled.");
                }
				UnityEditor.AssetDatabase.Refresh();
			}

			GameObject go = GameObject.Find("TerrainComposer2");

			Transform settingsT = go.transform.Find("Settings");
			if (settingsT != null)
			{
				TC_Settings settings = settingsT.GetComponent<TC_Settings>();

				if (settings != null)
				{
					settings.global = UnityEditor.AssetDatabase.LoadAssetAtPath(installPath + "/Defaults/TC_GlobalSettings.asset", typeof(TC_GlobalSettings)) as TC_GlobalSettings;
					if (settings.global == null) return false;
				}
				else return false;
			}
			else return false;
#endif
			return true;
		}

		static public void MoveToDustbin(Transform t)
		{
			TC_Settings settings = TC_Settings.instance;

			if (settings.dustbinT == null) settings.CreateDustbin();

			t.parent = settings.dustbinT;

			AddMessage(t.name + " is not compatible with the hierarchy of TerrainComposer\n It is moved to the 'Dustbin' GameObject.", 0);
			AddMessage("If you pressed the delete key you can undo it with control-z", 3);
		}

		static public void AddMessage(string message, float delay = 0, float duration = 2)
		{
			for (int i = 0; i < messages.Count; i++) if (messages[i].message.Contains(message)) return;
			messages.Add(new MessageCommand(message, delay, duration));
		}

		static public bool VerifyPortal(TC_ItemBehaviour item)
		{
			if (item == null) return false;
			if (item.outputId == heightOutput) return true;
			if (item is TC_Node || item is TC_NodeGroup) return true;
			
			AddMessage("This node cannot be used as a portal. All nodes in Height output can be used. And for the rest of the outputs only Nodes and NodeGroups can be used.");
			return false;
		}

		public class MessageCommand
		{
			public string message;
			public float delay;
			public float duration;
			public float startTime;
			
			public MessageCommand(string message, float delay, float duration)
			{
				this.message = message;
				this.delay = delay;
				this.duration = duration;
				startTime = Time.realtimeSinceStartup;
			}
		}
	}
}