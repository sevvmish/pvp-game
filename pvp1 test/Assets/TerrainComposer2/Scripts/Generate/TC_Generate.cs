using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.IO;


namespace TerrainComposer2
{
	[ExecuteInEditMode]
	public class TC_Generate : MonoBehaviour
	{
		public static TC_Generate instance;
		public float globalScale = 1;
		public TC_Area2D area2D;
		public bool assignTerrainHeightmap;
		public bool hideHierarchy;
		public bool generate;
		public bool generateSplat;
		public bool generateSplatSingle;
		public bool generateTree;
		public bool generateObject;
		public bool generateGrass;
		public bool generateColor;
		public bool resetTrees;
		public bool generateSingle;
		public int threadActive = 0;
		public bool isMesh;
		public bool resetObjects;

		public bool autoGenerate;
		public bool scanHeight;
		public bool cmdGenerate;
		public Rect autoGenerateRect = new Rect (0,0,1,1);
		public bool generateNextFrame;
		public int generateDone;
		public int generateDoneOld;
		
		public bool isGeneratingHeight;

		public int jobs = 0;
		public bool autoGenerateOld;
		float[] heightsReadback;
		float[,] heights;
		int[,] grass;
		List<TreeInstance> trees;
		int restoreAutoGenerateFrame = 0;
		bool restoreAutoGenerate;

		public List<GenerateStackEntry> stackEntry = new List<GenerateStackEntry>();

		public Transform objectParent;

		[System.NonSerialized] TC_Terrain firstTreeTerrain;
		[System.NonSerialized] TC_Terrain firstObjectTerrain;

		// Octree octree;

		// static public EditorCoroutine co;

		void Awake()
		{
			// octree = new Octree();

			autoGenerateOld = autoGenerate;
			// Debug.Log("Awake " + autoGenerate);
			autoGenerate = false; 
			restoreAutoGenerate = true;
		}
		
		void OnEnable()
		{
			if (instance == null) instance = this;
			isGeneratingHeight = false;

			#if UNITY_EDITOR
				UnityEditor.EditorApplication.update += MyUpdate;
				#if UNITY_5_1 || UNITY_5_2 || UNITY_5_3 || UNITY_5_4 || UNITY_5_5 || UNITY_5_6 || UNITY_2017_1
				UnityEditor.EditorApplication.playmodeStateChanged += BeforePlayMode;                
				#else
				UnityEditor.EditorApplication.playModeStateChanged += BeforePlayMode;
				#endif
				if (!UnityEditor.EditorApplication.isPlaying)
				{
					restoreAutoGenerate = true;
				}
			#endif
		}

		#if UNITY_EDITOR
		#if UNITY_5_1 || UNITY_5_2 || UNITY_5_3 || UNITY_5_4 || UNITY_5_5 || UNITY_5_6 || UNITY_2017_1
		void BeforePlayMode()
		#else
		void BeforePlayMode(UnityEditor.PlayModeStateChange state) 
		#endif
		{
			
			TC_Settings settings = TC_Settings.instance;

			if (!UnityEditor.EditorApplication.isPlaying && UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
			{
				// autoGenerateOld = autoGenerate;
				// autoGenerate = false;
				// UnityEditor.EditorUtility.SetDirty(this);
				// Debug.Log("Start");
				
				if (!area2D.terrainAreas[0]) return;
				
				if (settings.isRTPDetected && area2D.terrainAreas[0].IsRTPAddedToTerrains())
				{
					if (settings.autoColormapRTP) ExportColormap(settings.exportPath, true);
					if (settings.autoColormapRTP) ExportNormalmap(settings.exportPath, true);
				}
			}
		}
		#endif

		void OnDisable()
		{
			autoGenerateOld = autoGenerate;
			autoGenerate = false;  
			
			#if UNITY_EDITOR    
			UnityEditor.EditorApplication.update -= MyUpdate;
			#if UNITY_5_1 || UNITY_5_2 || UNITY_5_3 || UNITY_5_4 || UNITY_5_5 || UNITY_5_6 || UNITY_2017_1
			UnityEditor.EditorApplication.playmodeStateChanged -= BeforePlayMode;
			#else
			UnityEditor.EditorApplication.playModeStateChanged -= BeforePlayMode;
			#endif
			#endif
		}

		void OnDestroy()
		{
			if (instance == this) instance = null;
			#if UNITY_EDITOR
			UnityEditor.EditorApplication.update -= MyUpdate;
			#if UNITY_5_1 || UNITY_5_2 || UNITY_5_3 || UNITY_5_4 || UNITY_5_5 || UNITY_5_6 || UNITY_2017_1
			UnityEditor.EditorApplication.playmodeStateChanged -= BeforePlayMode;
			#else
			UnityEditor.EditorApplication.playModeStateChanged -= BeforePlayMode;
			#endif
			#endif
		}

		int frame = 0;
		bool updateCamCapture;

		public void Update()
		{
			#if UNITY_EDITOR
			if (Application.isPlaying) MyUpdate();
			#else
				MyUpdate();
			#endif 
		}
		
		public void MyUpdate()
		{
			TC_Settings settings = TC_Settings.instance;

			if (settings != null)
			{
				if (settings.version == 0)
				{
					settings.version = TC.GetVersionNumber();
					TC.RefreshOutputReferences(7);
					TC.AddMessage("TerrainComposer2 is updated to " + TC.GetVersionNumber().ToString());
				}
			}

			area2D = TC_Area2D.instance;

			if (area2D == null) return;
			if (area2D.terrainLayer == null) return;
			if (area2D.terrainLayer.layerGroups[0] == null) TC.RefreshOutputReferences(TC.allOutput);

			RefreshOutputReferences(TC.GetRefreshOutputReferences(), TC.refreshPreviewImages);

			if (cmdGenerate)
			{
				cmdGenerate = false;

				if (autoGenerate)
				{
					TC_Reporter.Log("Generate from auto", 2);
					Generate(false, autoGenerateRect);
				}
				else TC.autoGenerateCallTimeStart = Time.realtimeSinceStartup;

				autoGenerateRect = new Rect (0,0,1,1);
			}

			if (restoreAutoGenerate)
			{
				if (restoreAutoGenerateFrame > 1)
				{
					restoreAutoGenerate = false;
					restoreAutoGenerateFrame = 0;
					autoGenerate = autoGenerateOld;
				}
				++restoreAutoGenerateFrame;
			}

			generate = false;

			// if (frame >= 0)
			//{
				RunGenerateStack();
				// frame = 0;
			// }
			++frame;
		}

		public void RefreshOutputReferences(int outputId, bool refreshPreviewImages)
		{
			if (outputId >= 0)
			{
				TC.refreshPreviewImages = refreshPreviewImages;
				// Debug.Log("GetItems " + outputId);
				
				if (outputId >= 6) area2D.terrainLayer.GetItems(false, true, outputId == 7); else area2D.terrainLayer.GetItem(outputId, true, false);
				TC.RefreshOutputReferences(-1);
				TC.refreshPreviewImages = false;
				TC.repaintNodeWindow = true;
			}
		}

		public void RunGenerateStack()
		{
			// if (stack.Count > 0) Debug.Log(stack.Count);

			if (stackEntry.Count > 0)
			{
				if (stackEntry[0].stack.Count == 0) stackEntry.RemoveAt(0);
			}

			if (stackEntry.Count > 0 && !generate)
			{
				// Debug.Log(stack.Count);
				List<GenerateStack> stack = stackEntry[0].stack;
				GenerateStack curStack = stack[0];

				int outputId = curStack.outputId;
				//TODO Debug.Log ("TC_Generate.RunGenerateStack: Generating ID " + outputId + " in Stack of length " + stackEntry.Count + "/" + stack.Count + "!");
				TCUnityTerrain tcTerrain = curStack.tcTerrain;
				assignTerrainHeightmap = curStack.assignTerrainHeightmap;
				stack.RemoveAt(0);
				// Debug.Log(stack[0].tcTerrain.terrain.name);
				GenerateTerrain(tcTerrain, outputId);
				Compute(tcTerrain, outputId, curStack.generateRect);                
			}

			if (stackEntry.Count == 0 && disableHeightOuput)
			{
				disableHeightOuput = false;
				area2D.terrainLayer.layerGroups[TC.heightOutput].visible = false;
			}
		}

		bool disableHeightOuput;

		public void Compute(TCUnityTerrain tcTerrain, int outputId, Rect generateRect)
		{
			//TODO Debug.Log ("TC_Generate.Compute: Computing ID " + outputId + "!");
			// Debug.Log(frame);
			
			if (outputId == TC.heightOutput) ComputeHeight(generateRect); 
			else if (outputId == TC.splatOutput) ComputeSplat(generateRect);
			else if (outputId == TC.colorOutput) ComputeColor();
			else if (outputId == TC.treeOutput) ComputeTree();
			else if (outputId == TC.grassOutput) ComputeGrass();
			else if (outputId == TC.objectOutput) ComputeObject();

			tcTerrain.generateStatus &= (byte)(255 - Mathw.bit8[outputId]);

			if (tcTerrain.generateStatus == 0)
			{
				if (tcTerrain.tasks > 0)
				{
					--tcTerrain.tasks;
				}
				// tcTerrain.terrain.Flush();
				// Debug.Log("Flush terrain");

				if (!tcTerrain.terrain.gameObject.activeSelf)
				{
					// tcTerrain.terrain.enabled = true;
					tcTerrain.terrain.gameObject.SetActive(true);
				}
			}
			
			TC_Compute.instance.camCapture.DisposeRTCapture();
		}

		public bool CheckForTerrain(bool selectTerrainArea = true)
		{
			if (area2D.terrainAreas == null) area2D.terrainAreas = new TC_TerrainArea[1];
			else if (area2D.terrainAreas.Length == 0) area2D.terrainAreas = new TC_TerrainArea[1];

			if (area2D.terrainAreas[0] == null)
			{
				GameObject go = GameObject.Find("Terrain Area");
				if (go != null)
				{
					TC_TerrainArea terrainArea = go.GetComponent<TC_TerrainArea>();
					if (terrainArea != null) area2D.terrainAreas[0] = terrainArea;
					else
					{
						TC.AddMessage("The Terrain Area GameObject is missing the 'TC_TerrainArea' script.");
#if UNITY_EDITOR
						if (UnityEditor.EditorUtility.DisplayDialog("The Terrain Area GameObject is missing the 'TC_TerrainArea' script.", "Do you want to TC2 to add the 'TC_TerrainArea' script now?", "Yes", "Cancel"))
						{
							area2D.terrainAreas[0] = go.AddComponent<TC_TerrainArea>();
						}
						else return false;
#else
							return false;
#endif
					}
				}
				else { TC.AddMessage("No Terrain Area is created."); return false; }
			}
			bool hasTerrain = true;
			if (area2D.terrainAreas[0].terrains.Count == 0)
			{
				area2D.terrainAreas[0].terrains.Add(new TCUnityTerrain());
				hasTerrain = false;
			}

			for (int i = 0; i < area2D.terrainAreas[0].terrains.Count; i++)
			{
				if (!area2D.terrainAreas[0].terrains[i].CheckValidUnityTerrain())
				{
					if (selectTerrainArea) TC.AddMessage("Terrain missing on X" + area2D.terrainAreas[0].terrains[i].tileX + "_Y"+area2D.terrainAreas[0].terrains[i].tileZ + "\n\nTC2 has automatically selected the Terrain Area GameObject.");
					hasTerrain = false;
				}
			}

			if (!hasTerrain)
			{
				TC.AddMessage("Please create a terrain first.");
#if UNITY_EDITOR
					if (selectTerrainArea) UnityEditor.Selection.activeGameObject = area2D.terrainAreas[0].gameObject;
#endif
				return false;
			}
			return hasTerrain;
		}

		public void Generate(bool instantGenerate, int outputId = TC.allOutput)
		{
			Generate (instantGenerate, new Rect (0,0,1,1), outputId);
		}

		public void Generate(bool instantGenerate, Rect generateRect, int outputId = TC.allOutput)
		{
			// Debug.Log("Generate");
			if (!CheckForTerrain()) return;

			area2D = TC_Area2D.instance;

			if (area2D == null) return;
			if (area2D.terrainLayer == null) return;
			if (area2D.terrainLayer.layerGroups[0] == null) area2D.terrainLayer.GetItems(true);

			TC_Settings settings = TC_Settings.instance;
			if (settings == null)
			{
				TC.AddMessage("Settings GameObject not found."); return;
			}

			generateRect = Mathw.ClampRect (generateRect, new Rect (0,0,1,1));

			if (area2D.terrainAreas[0].terrains[0].texHeight == null)
			{
				// TC.AddMessage("The 'Height' output is not generated. Enable 'Height' output for TC2 to regenerate the height textures.", 0, 4);
				if (!area2D.terrainLayer.layerGroups[TC.heightOutput].visible)
				{
                    // Debug.Log("Not Active");
                    // area2D.terrainLayer.layerGroups[TC.heightOutput].visible = true;
                    // area2D.terrainLayer.layerGroups[TC.heightOutput].GetItems(true, false, false);
                    // disableHeightOuput = true;
				}
			}

			isMesh = false;
			bool firstTerrain = true;

			//TODO Debug.Log ("TC_Generate.Generate: Generating ID " + outputId + " " + (instantGenerate? "instantly" : "delayed") + " in rect " + generateRect.ToString () + "!");
			TC_TerrainArea terrainArea = area2D.terrainAreas[0];

			for (int i = 0; i < terrainArea.terrains.Count; i++)
			{
				TCUnityTerrain tcTerrain = terrainArea.terrains[i];

                if (tcTerrain.texHeight == null || (scanHeight && !area2D.terrainLayer.layerGroups[TC.heightOutput].active))
                {
					area2D.SetCurrentArea(tcTerrain);
					
					RenderTexture rtHeight = tcTerrain.rtHeight;
                    TC_Compute.instance.RunTerrainTexFromTerrainData(tcTerrain.terrain.terrainData, ref rtHeight);
                    RenderTextureHeightToTexHeight(rtHeight, tcTerrain, tcTerrain.terrain);
                }

				Vector2 terrainLocalPos = new Vector2 ((float)tcTerrain.tileX/terrainArea.tiles.x, (float)tcTerrain.tileZ/terrainArea.tiles.y);
				Vector2 terrainLocalSize = new Vector2 (1f/terrainArea.tiles.x, 1f/terrainArea.tiles.y);
				Rect terrainRect = new Rect (terrainLocalPos, terrainLocalSize);
				//Debug.Log ("Terrain tile " + tcTerrain.tileX + "/" + tcTerrain.tileZ + " in " + terrainArea.tiles.x + "/" + terrainArea.tiles.y + " tiles has rect " + terrainRect);
				Rect terrainGenRect;
				if (tcTerrain.active && Mathw.OverlapRect(generateRect, terrainRect, out terrainGenRect))
				{
					Rect relTerrainGenRect = new Rect ((terrainGenRect.x-terrainLocalPos.x)*terrainArea.tiles.x, (terrainGenRect.y-terrainLocalPos.y)*terrainArea.tiles.y, 
														terrainGenRect.width*terrainArea.tiles.x, terrainGenRect.height*terrainArea.tiles.y);
					relTerrainGenRect = Mathw.ClampRect (relTerrainGenRect, new Rect (0,0,1,1));
					//TODO Debug.Log ("Terrain tile " + tcTerrain.tileX + "/" + tcTerrain.tileZ + " generates global rect " + terrainGenRect.ToString () + " and relative rect " + relTerrainGenRect.ToString ());
					if (firstTerrain)
					{
						if (area2D.terrainLayer.layerGroups[TC.treeOutput].active && (outputId == TC.allOutput || outputId == TC.treeOutput)) firstTreeTerrain = tcTerrain;
						if (area2D.terrainLayer.layerGroups[TC.objectOutput].active && (outputId == TC.allOutput || outputId == TC.objectOutput)) firstObjectTerrain = tcTerrain;
						firstTerrain = false;
					}

					TC_Compute.instance.camCapture.collisionMask = 0;

					if (outputId == TC.allOutput) Generate(tcTerrain, instantGenerate, relTerrainGenRect);
					else
					{
						if (outputId == TC.heightOutput) GenerateHeight(tcTerrain, instantGenerate, relTerrainGenRect, false); else GenerateOutput(tcTerrain, outputId, instantGenerate, relTerrainGenRect);
					}
				}
			}
		}            

		public void GenerateMesh()
		{
			isMesh = true;
		}

		public void Generate(TCUnityTerrain tcTerrain, bool instantGenerate, int outputId = TC.allOutput)
		{
			Generate(tcTerrain, instantGenerate, new Rect(0, 0, 1, 1));
		}

		public void Generate(TCUnityTerrain tcTerrain, bool instantGenerate, Rect generateRect)
		{
			// Debug.Log(instantGenerate);
			// if (disableTerrain) tcTerrain.terrain.enabled = false;

			//TODO Debug.Log ("TC_Generate.Generate/Terrain: Generating terrain " + tcTerrain.index + "!");
			
			if (area2D.terrainLayer.layerGroups[TC.heightOutput].active || area2D.terrainLayer.layerGroups[TC.objectOutput].active) GenerateHeight(tcTerrain, instantGenerate, generateRect, false);
			
			for (int i = 1; i <= 4; i++)
			{
				if (area2D.terrainLayer.layerGroups[i].active) GenerateOutput(tcTerrain, i, instantGenerate, generateRect);
			}
			// TC.repaintNodeWindow = true;
		}

		public void GenerateOutput(TCUnityTerrain tcTerrain, int outputId, bool instantGenerate, Rect generateRect)
		{
			if (area2D.terrainLayer.layerGroups[outputId] != null)
			{
				// Debug.Log("Generate "+generate+ " +"+instantGenerate);
				tcTerrain.generateStatus |= (byte)Mathw.bit8[outputId];

				if (generate && !instantGenerate)
				{
					//TODO Debug.Log ("TC_Generate.GenerateOutput/Terrain: Enqueueing terrain " + tcTerrain.index + " output " + outputId + "!");

					bool addToStack = true;
					
					if (stackEntry.Count == 0) stackEntry.Add(new GenerateStackEntry(frame));
					else
					{
						if (stackEntry[0].frame != frame && stackEntry.Count == 1)
						{
							stackEntry.Add(new GenerateStackEntry(frame));
						}
					}

					if (stackEntry.Count == 2)
					{
						List<GenerateStack> stack = stackEntry[1].stack;

						for (int i = 0; i < stack.Count; i++)
						{
							GenerateStack genEntry = stack[i];
							if (genEntry.tcTerrain == tcTerrain && genEntry.outputId == outputId && genEntry.assignTerrainHeightmap == assignTerrainHeightmap) 
							{ // Ensure generate rect includes the rectangle
								Mathw.EncapsulteRect (ref genEntry.generateRect, generateRect);
								addToStack = false; 
								break; 
							}
						}
					}
					if (addToStack)
					{
						List<GenerateStack> stack = stackEntry[stackEntry.Count - 1].stack;
						stack.Add(new GenerateStack(outputId, tcTerrain, assignTerrainHeightmap, generateRect));
					}
				}
				else
				{
					//TODO Debug.Log ("TC_Generate.GenerateOutput/Terrain: Generating terrain " + tcTerrain.index + " output " + outputId + "!");
					// Debug.Log("Generate Output");
					++tcTerrain.tasks;
					GenerateTerrain(tcTerrain, outputId);
					Compute(tcTerrain, outputId, generateRect);
				}
			}
		}

		public void GenerateHeight(TCUnityTerrain tcTerrain, bool instantGenerate, Rect generateRect, bool disableTerrain)
		{
			//TODO Debug.Log ("TC_Generate.GenerateHeight/Terrain: Generating terrain " + tcTerrain.index + " height!");

			// Debug.Log(instantGenerate);
			// if (disableTerrain) tcTerrain.terrain.enabled = false;
			TC_Compute.instance.camCapture.collisionMask = 0;

			assignTerrainHeightmap = true;

			if (area2D.terrainLayer.layerGroups[TC.heightOutput].active && area2D.terrainLayer.layerGroups[TC.objectOutput].active)
			{
				if (area2D.terrainLayer.layerGroups[TC.heightOutput].ContainsCollisionNode()) assignTerrainHeightmap = false;
			}

			if (area2D.terrainLayer.layerGroups[TC.heightOutput].active) GenerateOutput(tcTerrain, TC.heightOutput, instantGenerate, generateRect);
			if (area2D.terrainLayer.layerGroups[TC.objectOutput].active) GenerateOutput(tcTerrain, TC.objectOutput, instantGenerate, generateRect);

			if (!assignTerrainHeightmap)
			{
				// Debug.Log("Second pass");
				assignTerrainHeightmap = true;
				GenerateOutput(tcTerrain, TC.heightOutput, instantGenerate, generateRect);
			}

			if (area2D.terrainLayer.layerGroups[TC.objectOutput].active) GenerateOutput(tcTerrain, TC.objectOutput, instantGenerate, generateRect);
			// TC.repaintNodeWindow = true;
		}

		public bool GenerateStart()
		{
			TC_Area2D.instance = area2D;
			// area2D.currentTerrainArea = area2D.terrainAreas[0];
			// Debug.Log("Generate Start");

			if (area2D.terrainAreas[0] == null) return false;
			
			if (area2D.previewArea != null)
			{
				if (area2D.previewArea.manual) area2D.SetManualTotalArea();
				else if (!area2D.CalcTotalArea()) return false;
			}
			else if (!area2D.CalcTotalArea()) return false;

			area2D.terrainsDone = 0;
			area2D.terrainsToDo = 0;
			generate = true;
			for (int i = 0; i < area2D.terrainAreas.Length; i++) area2D.terrainsToDo += area2D.terrainAreas[i].terrains.Count;

			return true;
		}

		public void GenerateStop()
		{
			++generateDone;
		}

		public void ComputeTerrainAreas()
		{
			if (!GenerateStart()) return;

			for (int j = 0; j < area2D.terrainAreas.Length; j++)
			{
				area2D.currentTerrainArea = area2D.terrainAreas[j];
				for (int i = 0; i < area2D.currentTerrainArea.terrains.Count; i++)
				{
					// if (ComputeTerrain(, area2D.currentTerrainArea.terrains[i], false)) ++area2D.terrainsDone;
				}
			}

			// isGeneratingHeight = false;

			GenerateStop();
		}

		public void ComputeMeshTerrainAreas()
		{
			float t = Time.realtimeSinceStartup;
			if (!GenerateStart()) return;

			for (int j = 0; j < area2D.meshTerrainAreas.Length; j++)
			{
				area2D.currentMeshTerrainArea = area2D.meshTerrainAreas[j];
				for (int i = 0; i < area2D.currentMeshTerrainArea.terrains.Count; i++)
				{
					area2D.currentMeshTerrainArea.terrains[i].SetNodesActive(true);
					// area2D.terrainLayer.GetItem(TC.heightOutput);
					if (ComputeMeshTerrain(TC.heightOutput, area2D.currentMeshTerrainArea.terrains[i], false)) ++area2D.terrainsDone;
					area2D.currentMeshTerrainArea.terrains[i].SetNodesActive(false);
					// Debug.Log("MeshTerrain " + i);
				}
			}

			// isGeneratingHeight = false;

			GenerateStop();
			float f = 1 / (Time.realtimeSinceStartup - t);
			Debug.Log("Mesh Frames " + f);

		}

		public bool GenerateTerrain(TCUnityTerrain tcTerrain, int outputId, bool doGenerateStart = true)
		{
			if (tcTerrain == null) return false;

			if (doGenerateStart)
			{
				if (!GenerateStart()) return false;
			}

			// TC_TerrainLayer.current = area2D.terrainLayer;

			if (tcTerrain.updateTerrainPos)
			{
				tcTerrain.updateTerrainPos = false;
				tcTerrain.terrain.gameObject.SetActive(false);
				if (tcTerrain.terrain.terrainData.treeInstanceCount > 0) tcTerrain.terrain.terrainData.treeInstances = new TreeInstance[0];
				tcTerrain.terrain.transform.position = tcTerrain.newPos;
			}

			area2D.SetCurrentArea(tcTerrain, outputId);

			// Debug.Log(i);
			// Debug.Log("resolution " + resolution.x);
			// ReportArea();
			//a Debug.Log("Preview Resolution " + area2D.previewResolution);
			// area2D.terrainLayer.GetItem(outputId);
			
			return true;
		}

		public void ReportArea()
		{
			Debug.Log("Resolution X " + area2D.resolution.x + " Y " + area2D.resolution.y);
			Debug.Log("IntResolution " + area2D.intResolution.ToString());
			Debug.Log("ResToTerrain X " + area2D.resolutionPM.x + " Y " + area2D.resolutionPM.y);

			Debug.Log("Bounds " + area2D.bounds);
			Debug.Log("StartPos X " + area2D.startPos.x + " Y " + area2D.startPos.y);
			Debug.Log("TerrainSize X " + area2D.terrainSize.x + " Y " + area2D.terrainSize.y);

			Debug.Log("Preview Resolution " + area2D.previewResolution);
			Debug.Log("ResToPreview X " + area2D.resToPreview.x + " Y " + area2D.resToPreview.y);

			Debug.Log("-------------------------------------------------------");
		}

		public bool ComputeMeshTerrain(int outputId, MeshTerrain tcMeshTerrain, bool doGenerateStart = true)
		{
			if (doGenerateStart)
			{
				if (!GenerateStart()) return false;
			}

			area2D.currentMeshTerrain = tcMeshTerrain;

			// if (!area2D.currentTCTerrain.active) return false;

			Int2 resolution = new Int2();
			Int2 resolution2 = new Int2();

			if (outputId == TC.heightOutput) { resolution.x = resolution.y = area2D.terrainLayer.meshResolution + 2; resolution2 = new Int2(resolution.x - 2, resolution.y - 2); }
			else if (outputId == TC.splatOutput) { resolution.x = resolution.y = area2D.terrainLayer.meshResolution; resolution2 = resolution; }
			// else if (computeGenerate.GetType() == typeof(ComputeGenerateTrees)) { resolution.x = resolution.y = area2D.layerLevel.meshResolution; resolution2 = resolution; }
			// else if (computeGenerate.GetType() == typeof(ComputeGenerateGrass)) { resolution.x = resolution.y = terrain.terrainData.detailResolution; resolution2 = resolution; }
			// else if (computeGenerate.GetType() == typeof(ComputeGenerateObjects)) { resolution.x = resolution.y = area2D.layerLevel.objectResolution; resolution2 = resolution; }
			else if (outputId == TC.colorOutput) { resolution.x = resolution.y = area2D.terrainLayer.meshResolution; resolution2 = resolution; }

			area2D.resolution = new Vector2(resolution.x, resolution.y);
			area2D.intResolution = resolution;
			TC_Reporter.Log("Resolution" + resolution.ToString());
			MeshTerrain meshTerrain = area2D.currentMeshTerrain;
			Vector2 size = new Vector2(meshTerrain.t.lossyScale.x * 10, meshTerrain.t.lossyScale.z * 10);
			area2D.resolutionPM = new Vector2(size.x / (resolution2.x), size.y / (resolution2.y));
			// Debug.Log("size " + size);
			// Debug.Log("con" + area2D.resToTerrain);
			area2D.area = new Rect(meshTerrain.t.position.x - (size.x / 2), meshTerrain.t.position.z - (size.y / 2), resolution.x, resolution.y);
			area2D.terrainSize = new Vector3(size.x, 4800, size.y);
			area2D.bounds = new Bounds(new Vector3(meshTerrain.t.position.x, 0, meshTerrain.t.position.z), area2D.terrainSize);
			area2D.startPos = new Vector3(area2D.area.xMin, meshTerrain.t.position.y, area2D.area.yMin);

			// Debug.Log(i);
			return true;
		}

		public void ExportHeightmap(string path)
		{
			byte[] bytes = null;
			Color32[] colors;
			
			int i = 0;
			int offset = TC_Area2D.instance.resExpandBorder;
			
			for (int y = 0; y < area2D.terrainAreas[0].tiles.y; ++y)
			{
				for (int x = 0; x < area2D.terrainAreas[0].tiles.x; ++x)
				{
					TCUnityTerrain terrain = area2D.terrainAreas[0].terrains[i];
					Texture2D texHeight = terrain.texHeight;

					if (texHeight == null) continue;

					string filePath = path + "/" + TC_Settings.instance.heightmapFilename + "_x" + x + "_y" + y + ".raw";

					FileStream fs = new FileStream(filePath, FileMode.Create);
					
					colors = terrain.texHeight.GetPixels32();
					
					Int2 resolution = new Int2(texHeight.width - (offset * 2), texHeight.height - (offset * 2));

					int size = resolution.x * resolution.y;

					if (bytes == null) bytes = new byte[size * 2];
					else if (bytes.Length != size * 2) bytes = new byte[size * 2];
					
					for (int yy = 0; yy < resolution.y; yy++)
					{
						for (int xx = 0; xx < resolution.x; xx++)
						{
							int index = ((yy * resolution.x) + xx) * 2;
							int index2 = ((texHeight.height - yy - 1 - offset) * texHeight.width) + xx + offset;
							bytes[index] = colors[index2].g;
							bytes[index + 1] = colors[index2].r;
						}
					}

					fs.Write(bytes, 0, bytes.Length);
					fs.Close();

					i++;
				}
			}

#if UNITY_EDITOR
			UnityEditor.AssetDatabase.Refresh();
#endif
		}

		public void ExportHeightmapCombined(string path)
		{
			TC_Settings settings = TC_Settings.instance;

			byte[] bytes = null;
			
			int i = 0;
			// int offset = TC_Area2D.current.resExpandBorder;

			string filePath = path + "/" + TC_Settings.instance.heightmapFilename + ".raw";

			FileStream fs = new FileStream(filePath, FileMode.Create);

			Int2 tiles;
			
			if (settings.importSource == TC_Settings.ImportSource.TC2_TerrainArea) tiles = area2D.terrainAreas[0].tiles;
			else tiles = settings.importTiles;

			for (int y = 0; y < tiles.y; ++y)
			{
				for (int x = 0; x < tiles.x; ++x)
				{
					TerrainData terrainData;
					if (settings.importSource == TC_Settings.ImportSource.TC2_TerrainArea) terrainData = area2D.terrainAreas[0].terrains[i].terrain.terrainData;
					else terrainData = settings.importTerrains[i];

					if (terrainData == null) { TC.AddMessage("TerrainData X" + x + "_Y" + y + " is null"); i++; continue; }
					//Texture2D texHeight = terrain.texHeight;
					//if (texHeight == null) continue;
					//colors = terrain.texHeight.GetPixels32();
					//Int2 resolution = new Int2(texHeight.width - (offset * 2), texHeight.height - (offset * 2));
					int resolution = terrainData.heightmapResolution;

					float[,] heights = terrainData.GetHeights(0, 0, resolution, resolution);
					
					// int size = resolution.x * resolution.y;

					if (bytes == null) bytes = new byte[resolution * 2];
					else if (bytes.Length != resolution * 2) bytes = new byte[resolution * 2];

					fs.Seek((x * resolution * 2) + ((tiles.y - y - 1) * resolution * (resolution * 2 * tiles.x)), SeekOrigin.Begin);

					for (int yy = 0; yy < resolution; yy++)
					{
						int index = 0;
						for (int xx = 0; xx < resolution; xx++)
						{
							int height = (int)(heights[resolution - yy - 1, xx] * 65536);
							byte hiByte = (byte)(height >> 8);
							bytes[index++] = (byte)(height - (hiByte << 8)); 
							bytes[index++] = hiByte;
						}
						
						fs.Write(bytes, 0, bytes.Length); 
						fs.Seek(resolution * 2 * (tiles.x - 1), SeekOrigin.Current);
					}
					
					i++;
				}
			}

			fs.Close();

#if UNITY_EDITOR
			UnityEditor.AssetDatabase.Refresh();
#endif
		}

		public void CreateLayerFromExportedHeightmap(string path)
		{
#if UNITY_EDITOR
			if (!path.Contains("RawFiles")) { TC.AddMessage("You need to save the heightmap in a RawFiles folder for TC2 to make a stamp out of it."); return; }

			string filePath = path.Replace(Application.dataPath, "Assets");
			filePath = filePath.Replace("RawFiles" , "") + TC_Settings.instance.heightmapFilename + ".Jpg";

			// Debug.Log(filePath);
			
			Texture tex = UnityEditor.AssetDatabase.LoadAssetAtPath(filePath, typeof(Texture)) as Texture;
			
			if (tex == null) { TC.AddMessage("'" + filePath + "' not found."); return; }
			
			TC_LayerGroup heightLayerGroup = area2D.terrainLayer.layerGroups[TC.heightOutput];

			TC_Layer layer = heightLayerGroup.groupResult.Add<TC_Layer>("", false) as TC_Layer;
			layer.GetItems(false, false, false);
			TC_Node node = layer.selectNodeGroup.itemList[0] as TC_Node;

			node.DropTextureEditor(tex);

#endif
		}

		public void ExportNormalmap(string path, bool assignRTP)
		{
			Color32[] colors, normalColors = null;

			int i = 0;
			int offset = TC_Area2D.instance.resExpandBorder;
			float nx, ny, nz;
			float normalmapStrength = TC_Settings.instance.normalmapStrength;
			
			for (int y = 0; y < area2D.terrainAreas[0].tiles.y; ++y)
			{
				for (int x = 0; x < area2D.terrainAreas[0].tiles.x; ++x)
				{
					TCUnityTerrain tcTerrain = area2D.terrainAreas[0].terrains[i];
					Texture2D texHeight = tcTerrain.texHeight;

					if (texHeight == null) continue;

					colors = tcTerrain.texHeight.GetPixels32();

					Int2 resolution = new Int2(texHeight.width - (offset * 2), texHeight.height - (offset * 2));
					int size = resolution.x * resolution.y;

					TC_Compute.InitTexture(ref tcTerrain.texNormalmap, TC_Settings.instance.normalmapFilename, resolution.x, true, TextureFormat.RGB24);

					if (normalColors == null) normalColors = new Color32[size];
					else if (normalColors.Length != size) normalColors = new Color32[size];
					
					for (int yy = 0; yy < resolution.y; yy++)
					{
						for (int xx = 0; xx < resolution.x; xx++)
						{
							int index = ((yy * resolution.x) + xx);
							int index2 = ((yy + offset) * texHeight.width) + xx + offset;
							nx = ((((float)colors[index2].b / 255.0f) * 2) - 1) * normalmapStrength;
							nz = ((((float)colors[index2].a / 255.0f) * 2) - 1) * normalmapStrength;
							ny = Mathf.Sqrt(1 - nx * nx - nz * nz);
							

							normalColors[index].r = colors[index2].b;
							normalColors[index].g = colors[index2].a;
							normalColors[index].b = (byte)(ny * 255f);
						}
					}

					tcTerrain.texNormalmap.SetPixels32(normalColors);
					tcTerrain.texNormalmap.Apply();

#if UNITY_EDITOR

					string filePath = ExportImage(path + "/" + TC_Settings.instance.normalmapFilename + "_x" + x + "_y" + y, tcTerrain.texNormalmap);

					filePath = filePath.Replace(Application.dataPath, "Assets");

					UnityEditor.AssetDatabase.Refresh();

					UnityEditor.TextureImporter textureImporter = (UnityEditor.TextureImporter)UnityEditor.AssetImporter.GetAtPath(filePath);

					if (textureImporter)
					{
#if UNITY_5_1 || UNITY_5_2 || UNITY_5_3 || UNITY_5_4
							textureImporter.normalmap = true;
#else
							textureImporter.textureType = UnityEditor.TextureImporterType.NormalMap;
#endif

						textureImporter.SaveAndReimport();
						UnityEditor.AssetDatabase.ImportAsset(filePath, UnityEditor.ImportAssetOptions.ForceUpdate);
						// Debug.Log("Normalmap");
					}
					
					if (TC_Settings.instance.isRTPDetected && TC_Settings.instance.autoNormalmapRTP && assignRTP)
					{
						// Debug.Log(filePath);

						Texture2D tex = (Texture2D)UnityEditor.AssetDatabase.LoadAssetAtPath(filePath, typeof(Texture2D));
						if (tex != null)
						{
							tcTerrain.AssignTextureRTP("NormalGlobal", tex);
						}
					}
#endif

					i++;
				}
			}

#if UNITY_EDITOR
			UnityEditor.AssetDatabase.Refresh();
#endif
		}

		public void ExportColormap(string path, bool assignRTP)
		{
#if UNITY_EDITOR
			int i = 0;
			for (int y = 0; y < area2D.terrainAreas[0].tiles.y; ++y)
			{
				for (int x = 0; x < area2D.terrainAreas[0].tiles.x; ++x)
				{
					TCUnityTerrain tcTerrain = area2D.terrainAreas[0].terrains[i];

					if (tcTerrain.texColormap == null) continue;
					
					string filePath = ExportImage(path + "/" + TC_Settings.instance.colormapFilename + "_x" + x + "_y" + y, tcTerrain.texColormap);
					if (TC_Settings.instance.isRTPDetected && TC_Settings.instance.autoColormapRTP && assignRTP)
					{
						filePath = filePath.Replace(Application.dataPath, "Assets");
						// Debug.Log(filePath);

						Texture2D tex = (Texture2D)UnityEditor.AssetDatabase.LoadAssetAtPath(filePath, typeof(Texture2D));
						if (tex != null)
						{
							tcTerrain.AssignTextureRTP("ColorGlobal", tex);
						}
					}
			
					++i;
				}

			}

			UnityEditor.AssetDatabase.Refresh();
#endif
		}

		public string ExportImage(string filePath, Texture2D tex)
		{
			byte[] bytes;
			string extension;
			
			if (TC_Settings.instance.imageExportFormat == TC_Settings.ImageExportFormat.PNG)
			{
				bytes = tex.EncodeToPNG(); extension = "png";
			}
			else
			{
				bytes = tex.EncodeToJPG(); extension = "jpg";
			}

			filePath = filePath + "." + extension;

#if !UNITY_WEBPLAYER
			File.WriteAllBytes(filePath, bytes);
#endif

			return filePath;
		}

		public void ExportSplatmap(string path)
		{
			int i = 0;
			for (int y = 0; y < area2D.terrainAreas[0].tiles.y; ++y)
			{
				for (int x = 0; x < area2D.terrainAreas[0].tiles.x; ++x)
				{
					TCUnityTerrain tcTerrain = area2D.terrainAreas[0].terrains[i];

					Terrain terrain = tcTerrain.terrain;

					if (terrain == null) continue;
					if (terrain.terrainData == null) continue;

					Texture2D[] texSplatmaps = terrain.terrainData.alphamapTextures;

					for (int j = 0; j < texSplatmaps.Length; j++)
					{

						if (texSplatmaps[j] == null) continue;

						ExportImage(path + "/" + TC_Settings.instance.splatmapFilename + j + "_x" + x + "_y" + y, texSplatmaps[j]);

					}
					++i;
				}

			}

#if UNITY_EDITOR
			UnityEditor.AssetDatabase.Refresh();
#endif
		}

        void RenderTextureHeightToTexHeight(RenderTexture rtHeight, TC_Terrain tcTerrain, Terrain terrain)
        {
            TC_Compute.InitTexture(ref tcTerrain.texHeight, "HeightTexture " + terrain.name, rtHeight.width, true);

            RenderTexture rtActiveOld = RenderTexture.active;
            RenderTexture.active = rtHeight;
            tcTerrain.texHeight.ReadPixels(new Rect(0, 0, rtHeight.width, rtHeight.height), 0, 0);
            tcTerrain.texHeight.Apply();
            RenderTexture.active = rtActiveOld;
        }

        public void ComputeHeight(Rect generateRect)
		{
			// Debug.Log("ComputeHeight");
			TC_LayerGroup heightLayerGroup = area2D.terrainLayer.layerGroups[TC.heightOutput];
			// Debug.Log(area2D.currentTCUnityTerrain.terrain.name + " " + assignTerrainHeightmap);
			// Debug.Log("ComputeHeight");

			if (heightLayerGroup == null) return;
			if (!heightLayerGroup.active) return;

			if (!assignTerrainHeightmap)
			{
				area2D.currentTCUnityTerrain.ResetObjects();
				area2D.terrainLayer.ResetObjects();
			}

			TC_Compute compute = TC_Compute.instance;
			// isGeneratingHeight = true;
			int resolution = area2D.intResolution.x;

			ComputeBuffer buffer = null;

			// TC_Reporter.BenchmarkStart();
			heightLayerGroup.ComputeSingle(ref buffer, 0, true);
			// TC_Reporter.BenchmarkStop("Height compute");
			if (buffer == null) { TC_Reporter.Log("final buffer is null"); return; }

			// if (tcGenerate.isMesh) Debug.Log("Frames generate " + area2D.currentMeshTerrain.t.name + " " + f.ToString("F2"));
			// else 
			// Debug.Log("Frames generate " + area2D.currentTerrain.name + " " + f.ToString("F2"));
			// Debug.Log("Is mesh " + isMesh);

			if (!isMesh)
			{
				// Debug.Log(area2D.currentTCUnityTerrain.terrain.transform.name);
				// Debug.Log(((TCUnityTerrain)area2D.currentTCTerrain).terrain.name);

				compute.RunTerrainTex(buffer, ref area2D.currentTCTerrain.rtHeight, resolution);
				
				// else Debug.Log("Pass");
				
				RenderTexture rtHeight = area2D.currentTCTerrain.rtHeight;

                RenderTextureHeightToTexHeight(rtHeight, area2D.currentTCTerrain, area2D.currentTerrain);

				// Debug.Log(area2D.currentTCTerrain.texHeight.mipmapCount);
				
				//if (TC_Settings.instance.isRTPDetected && TC_Settings.instance.autoNormalmapRTP)
				//{
				//    ExportNormalmap(TC_Settings.instance.exportPath);
				//    area2D.currentTCUnityTerrain.AssignTextureRTP("NormalGlobal");
				//}

				//if (!assignTerrainHeightmap)
				//{
				//    // Debug.Log("!!!!!!!!!!!!!!");
				//    compute.DisposeBuffer(ref buffer);
				//    return;
				//}

				int offset = area2D.resExpandBorder;

				int heightResolution = resolution - (offset * 2);

				generateRect = Mathw.ClampRect (generateRect, new Rect (0,0,1,1));
				//TODO Debug.Log ("TC_Generate.ComputeHeight: Computing Height of terrain " + area2D.currentTCUnityTerrain.tileX + "/" + area2D.currentTCUnityTerrain.tileZ + " at " + generateRect.ToString () + "!");
				Rect pixelRect = new Rect (Mathf.Floor (heightResolution*generateRect.x), Mathf.Floor (heightResolution*generateRect.y),
					Mathf.Ceil (heightResolution*generateRect.width), Mathf.Ceil (heightResolution*generateRect.height));

				TC.InitArray(ref heights, (int)pixelRect.height, (int)pixelRect.width);
				TC.InitArray(ref heightsReadback, resolution * resolution);

                // Debug.LogError("Resolution " + resolution + " buffer " + buffer.count);

				// TODO: Can read directly into heights with rearranging the array
				buffer.GetData(heightsReadback);
				compute.DisposeBuffer(ref buffer);
				
				// Debug.Log(resolution);
				// Debug.Log(heightResolution);
				// Debug.Log(offset);

				//for (int x = 0; x < 32; x++)
				//{
				//    Int2 pixel = new Int2(x, 0);
				//    Color color = area2D.currentTCTerrain.texHeight.GetPixel(pixel.x, pixel.y + 1);

				//    float red = (color.r * 255) * 256;
				//    float green = (color.g * 255);

				//    float height = (red + green);

				//    float height2 = (heightsReadback[x] * 65535);

				//    Debug.Log(height2 + " - " + height + " = " + (height2 - height));
				//}

				int yLength = (int)pixelRect.height;
				int xLength = (int)pixelRect.width;
				int pixelRectX = (int)pixelRect.x;
				int pixelRectY = (int)pixelRect.y;

				for (int y = 0; y < yLength; y++)
				{
					for (int x = 0; x < xLength; x++)
					{
						//Debug.Log ("Sampling pixel " + x + "/" + y + " from " + (x + offset + ((y + offset) * resolution)));
						heights[y, x] = heightsReadback[x + offset + pixelRectX + ((y + offset + pixelRectY) * resolution)];
					}
				}

				if (area2D.currentTCUnityTerrain.tileX == 0 && area2D.currentTCUnityTerrain.tileZ == 0)//  && !exportHeightmap)
				{
					area2D.currentTerrainArea.ResetNeighbors();
				}

				area2D.currentTerrain.terrainData.SetHeights(pixelRectX, pixelRectY, heights);

				if (area2D.currentTCUnityTerrain.tileX == area2D.currentTerrainArea.tiles.x - 1 && area2D.currentTCUnityTerrain.tileZ == area2D.currentTerrainArea.tiles.y - 1)
				{
					area2D.currentTerrainArea.SetNeighbors();
				}
				
				// area2D.currentTerrain.materialTemplate.SetTexture("_NormalMapGlobal", area2D.currentTCUnityTerrain.renderTex);
				// Debug.Log("Set heights");
			}
			else
			{
				// Debug.Log(area2D.currentMeshTerrain.t.name);
				// compute.RunTerrainTex(buffer, ref area2D.currentTCTerrain.rtHeight, true);
				// compute.DisposeBuffer(ref buffer);
				
				// Debug.Log(area2D.currentMeshTerrain.t.name);
				// Debug.Log("Assign RTP material");
			}
		}

		//void OnDrawGizmos()
		//{
		//    // Debug.Log("help");
		//    if (octree == null) return;
		//    if (octree.cell != null) octree.cell.Draw(false);
		//}

		public void ComputeColor()
		{
			if (area2D.terrainLayer.layerGroups[TC.colorOutput] == null) return;

			TC_Compute compute = TC_Compute.instance;

			compute.SetPreviewColors(compute.colors);

			ComputeBuffer maskBuffer = null;

			TC_Compute.InitRenderTextures(ref compute.rtsColor, "rtsColor", 1);

			TC_Compute.InitRenderTexture(ref compute.rtResult, "rtResult");
			area2D.terrainLayer.layerGroups[TC.colorOutput].ComputeMulti(ref compute.rtsColor, ref maskBuffer, 0);

			area2D.currentTerrainArea.rtColormap = compute.rtsColor[0];

			if (maskBuffer != null) compute.DisposeBuffer(ref maskBuffer);
			
			if (!isMesh)
			{
				// compute.RunColormap(ref colorRTexture, ref area2D.currentTCUnityTerrain.colormap);

				RenderTexture rtActiveOld = RenderTexture.active;
				TC_Compute.InitTexture(ref area2D.currentTCUnityTerrain.texColormap, "texColormap", -1, true, TextureFormat.RGB24, true, false);
				Texture2D texColormap = area2D.currentTCUnityTerrain.texColormap;

				RenderTexture.active = area2D.currentTerrainArea.rtColormap;
				texColormap.ReadPixels(new Rect(0, 0, texColormap.width, texColormap.height), 0, 0);
				texColormap.Apply();

				if (TC_Settings.instance.isRTPDetected && TC_Settings.instance.autoColormapRTP)
				{
					area2D.currentTCUnityTerrain.AssignTextureRTP("ColorGlobal", area2D.currentTCUnityTerrain.texColormap);
				}

				RenderTexture.active = rtActiveOld;

//                if (area2D.currentTCUnityTerrain.terrain.materialTemplate != null) area2D.currentTCUnityTerrain.terrain.materialTemplate.SetTexture("_Colormap", area2D.currentTCUnityTerrain.texColormap);
//                else
//                {
//                    TC.AddMessage("The TC2 Colormap material is not assigned to the terrain. So it won't show.");
//                    TC.AddMessage("This will be added in the next beta.");
//                    // TC.AddMessage("Please go to the Setting tab in the inspector on Terrain Area GameObject and assign the custom material 'TC2_TerrainMaterial'.");
//                    // TC.AddMessage("The 'TC2_TerrainMaterial' is in the folder TerrainComposer2 -> Shaders -> Terrain.");
//#if UNITY_EDITOR
//                    // UnityEditor.Selection.activeTransform = area2D.terrainAreas[0].transform;
//#endif
//                }

				// Material rtpMat = area2D.currentTerrain.materialTemplate;
				// rtpMat.SetTexture("_ColorMapGlobal", area2D.currentTCUnityTerrain.colormapTex);
				// area2D.currentTerrain.GetComponent<ReliefTerrain>().ColorGlobal = texColormap;
			}
			else
			{
				// compute.RunColormap(ref colorRTexture, ref area2D.currentMeshTerrain.colormap);
				area2D.currentMeshTerrain.rtpMat.SetTexture("_ColorMapGlobal", area2D.currentTerrainArea.rtColormap);
			}
		}

		public void ComputeSplat(Rect generateRect)
		{
			if (area2D.terrainLayer.layerGroups[TC.splatOutput] == null) return;
			int splatLength = area2D.splatLength;
			int splatmapLength = area2D.splatmapLength;

			// Debug.Log("Splat Compute " + TC_Area2D.current.currentTCUnityTerrain.terrain.name);

			if (splatLength == 0)
			{
				TC.AddMessage("No splat textures assigned to terrain '" + area2D.currentTerrain.name + "'");
				TC.AddMessage("Splat textures can be assigned on the Terrain Area GameObject -> Splat tab.", 2);
				return;
			}
			else if (splatLength > TC.splatLimit)
			{
				TC.AddMessage("TC2 supports generating maximum " + TC.splatLimit + " splat textures. There are " + splatLength + " on " + area2D.currentTerrain.name + " assigned.", 0, 4);
				return;
			}

			TC_Compute compute = TC_Compute.instance;

			ComputeBuffer maskBuffer = null;
			RenderTexture[] rtSplatmaps;

			compute.SetPreviewColors(compute.splatColors);

			if (!isMesh)
			{
				TC_Compute.InitRenderTextures(ref area2D.currentTerrainArea.rtSplatmaps, "splatmapRTextures", splatmapLength);
			}
			else
			{
				// TCCompute.InitRenderTextures(ref area2D.currentTerrainArea.splatmapRTextures, "splatmapRTextures");
				// splatmapRTextures = area2D.currentTerrainArea.splatmapRTextures;
				// area2D.terrainTex = area2D.currentTerrainArea.renderTex;
			}
			TC_Compute.InitRenderTextures(ref compute.rtsResult, "resultRTextures", splatmapLength);
			rtSplatmaps = area2D.currentTerrainArea.rtSplatmaps;

			area2D.terrainLayer.layerGroups[TC.splatOutput].ComputeMulti(ref rtSplatmaps, ref maskBuffer, 0);
			if (maskBuffer != null) compute.DisposeBuffer(ref maskBuffer);
			
			if (!isMesh)
			{
				Texture2D[] texSplatmaps = area2D.currentTerrain.terrainData.alphamapTextures;

				generateRect = Mathw.ClampRect (generateRect, new Rect (0,0,1,1));

				// The rect has to be transformed into pixel-space, accounting for different source(RT) and target (Tex) resolutions
				// First, rect has to be transformed to tex, then to RT space in order to sync destX/Y and srcRect
				// This prevents flickering and small sporadic movements of the splatmap on the terrain

				Int2 dest;
				Rect srcRect = Mathw.UniformRectToResolution (generateRect, new Int2 (rtSplatmaps[0].width, rtSplatmaps[0].height), new Int2 (texSplatmaps[0].width, texSplatmaps[0].height), out dest);
				// For some odd reason, y-position has to be inverted
				srcRect.y = rtSplatmaps[0].width-srcRect.y-srcRect.height;

				//TODO Debug.Log ("Assigning splats at " + generateRect.ToString () + " from source pixel rect " + srcRect.ToString ());

				RenderTexture rtActiveOld = RenderTexture.active;
				// Debug.Log(area2D.currentTerrain.name + " " + splatmapRTextures.Length);
				for (int i = 0; i < rtSplatmaps.Length; i++)
				{
					RenderTexture.active = rtSplatmaps[i];
					texSplatmaps[i].ReadPixels(srcRect, dest.x, dest.y);
					texSplatmaps[i].Apply();
				}
				RenderTexture.active = rtActiveOld;
			}
			else
			{
				area2D.currentMeshTerrain.rtpMat.SetTexture("_Control1", rtSplatmaps[0]);
				area2D.currentMeshTerrain.rtpMat.SetTexture("_Control2", rtSplatmaps[1]);
				area2D.currentMeshTerrain.rtpMat.SetTexture("_Control3", rtSplatmaps[1]);
				// Debug.Log("Assign rtp splat");
			}
			// Debug.Log("Frames generate " + area2D.currentTerrain.name + " " + f.ToString("F2"));
		}

		// TODO: Same as splat
		public void ComputeGrass()
		{
			if (area2D.terrainLayer.layerGroups[TC.grassOutput] == null) return;
			int grassLength = area2D.currentTerrain.terrainData.detailPrototypes.Length;
			if (grassLength == 0)
			{
				TC.AddMessage("No grass assigned to terrain '" + area2D.currentTerrain.name + "'");
				TC.AddMessage("Grass can be assigned on the Terrain Area GameObject -> Grass tab.", 2);
				return;
			}
			else if (grassLength > TC.grassLimit)
			{
				TC.AddMessage("TC2 supports generating maximum " + TC.grassLimit + " grass textures. There are " + grassLength + " on " + area2D.currentTerrain.name + " assigned.", 0, 4);
				return;
			}

			int resolution = area2D.intResolution.x;

			TC_Compute compute = TC_Compute.instance;
			compute.SetPreviewColors(compute.splatColors);
			
			ComputeBuffer maskBuffer = null;
			
			int grassmapCount = Mathf.CeilToInt(grassLength / 4.0f);
			
			// Debug.Log(grassmapCount + " " + grassLength);
			// TC_Compute.InitRenderTextures(ref area2D.currentTerrainArea.rtSplatmaps, "splatmapRTextures");

			TC_Compute.InitRenderTextures(ref compute.rtsSplatmap, "rtsSplatmap", grassmapCount);
			TC_Compute.InitRenderTextures(ref compute.rtsResult, "rtsResult", grassmapCount);
			RenderTexture[] rtGrassmaps = compute.rtsSplatmap;
			
			area2D.terrainLayer.layerGroups[TC.grassOutput].ComputeMulti(ref rtGrassmaps, ref maskBuffer, 0);
			compute.DisposeBuffer(ref maskBuffer);
			
			TC_Compute.InitTextures(ref compute.texGrassmaps, "grassTextures", grassmapCount);
			compute.InitBytesArray(grassmapCount);
			TC_Compute.BytesArray[] bytesArray = compute.bytesArray;

			RenderTexture rtActiveOld = RenderTexture.active;
			for (int i = 0; i < rtGrassmaps.Length; i++)
			{
				RenderTexture.active = rtGrassmaps[i];
				compute.texGrassmaps[i].ReadPixels(new Rect(0, 0, rtGrassmaps[i].width, rtGrassmaps[i].height), 0, 0);
				bytesArray[i].bytes = compute.texGrassmaps[i].GetRawTextureData();
			}

			RenderTexture.active = rtActiveOld;

			TC.InitArray(ref grass, resolution, resolution);
			
			int index, colorIndex, byteIndex;

			for (int i = 0; i < grassLength; ++i)
			{
				index = i / 4;
				colorIndex = (1 + (i - (index * 4))) % 4;
				for (int y = 0; y < resolution; ++y)
				{
					for (int x = 0; x < resolution; ++x)
					{
						byteIndex = (y * resolution * 4) + (x * 4) + colorIndex;
						grass[y, x] = (int)(((float)bytesArray[index].bytes[byteIndex] / 255.0f) * 16.0f);
					}
				}

				area2D.currentTerrain.terrainData.SetDetailLayer(0, 0, i, grass);
			}
		}

		public void ComputeTree()
		{
			if (area2D.terrainLayer.layerGroups[TC.treeOutput] == null) return;
			if (area2D.currentTerrain.terrainData.treePrototypes.Length == 0)
			{
				TC.AddMessage("No trees assigned to terrain '" + area2D.currentTerrain.name + "'");
				TC.AddMessage("Trees can be assigned on the Terrain Area GameObject -> Trees tab.", 2);
				return;
			}
            //if (area2D.currentTCTerrain.texHeight == null)
            //{
            //    TC.AddMessage("You need to generate a heightmap first", 0, 7);
            //    return;
            //}

            if (firstTreeTerrain == area2D.currentTCTerrain)
			{
				area2D.terrainLayer.ResetPlaced();
				firstTreeTerrain = null;

				//if (octree == null) octree = new Octree();
				//if (octree.cell == null)
				//{
				//    octree.cell = new Octree.Cell(null, 0, area2D.totalTerrainBounds);
				//    octree.cell.maxLevels = 8;// Mathf.RoundToInt(octree.cell.bounds.size.x / 32);
				//    //Debug.Log(octree.cell.maxLevels);
				//}
				//else if (octree.cell.bounds != area2D.totalTerrainBounds) octree.cell.bounds = area2D.totalTerrainBounds;

				//Debug.Log(octree.cell.bounds.size);

				//octree.cell.Reset();
			}
			
			int resolution = area2D.intResolution.x;

			TC_Compute compute = TC_Compute.instance;
			compute.SetPreviewColors(compute.splatColors);
			
			ComputeBuffer itemMapBuffer = null;

			area2D.terrainLayer.layerGroups[TC.treeOutput].ComputeSingle(ref itemMapBuffer, 0, true);
			// compute.RunItemPositionCompute(itemMapBuffer, TC.treeOutput);

			ItemMap[] itemMap = new ItemMap[resolution * resolution];

			itemMapBuffer.GetData(itemMap);

			compute.DisposeBuffer(ref itemMapBuffer);
			
			if (trees == null) trees = new List<TreeInstance>();

			Vector3 terrainSize = area2D.currentTerrain.terrainData.size;
			Vector3 terrainPos = area2D.currentTerrain.transform.position;
			Vector3 outputOffset = area2D.outputOffsetV3;

			List<TC_SelectItem> treeItems = TC_Area2D.instance.terrainLayer.treeSelectItems;
			
			for (int y = 0; y < resolution; ++y)
			{
				for (int x = 0; x < resolution; ++x)
				{
					// TODO: Move more to compute shader
					int index = (y * resolution) + x;
					float density = itemMap[index].density * itemMap[index].maskValue;

					if (density == 0) continue;

					Vector3 pos = new Vector3((float)x / resolution, 0, (float)y / resolution);
					Vector3 pos2 = pos + itemMap[index].pos;

					if (pos2.x < 0 || pos2.x > 1 || pos2.z < 0 || pos2.z > 1)
					{
						// Debug.Log(position.x + ", "+position.y+", "+position.z);
						continue;
					}

					// Debug.Log("x " + itemMap[index].pos.x + " z " + itemMap[index].pos.z);

					int id = itemMap[index].index;
					if (id > treeItems.Count - 1)
					{
						TC.AddMessage("Tree index is out of bounds, index = " + id + ". Try the 'Refresh' button.");
						return;
					}

					TC_SelectItem item = treeItems[id];
					int treeIndex = item.selectIndex;
					TC_SelectItem.Tree tree = item.tree;

					Vector2 posSeed = new Vector2(pos.x * terrainSize.x, pos.z * terrainSize.z) + new Vector2(terrainPos.x - outputOffset.x, terrainPos.z - outputOffset.z);
					posSeed = Mathw.SnapVector2(posSeed + new Vector2(area2D.resolutionPM.x / 4, area2D.resolutionPM.x / 4), area2D.resolutionPM.x / 2);

					// pos.y += tree.heightOffset / terrainSize.y;

					// Debug.Log(id + " " + treeIndex);

					TreeInstance treeInstance = new TreeInstance();
					treeInstance.color = Color.white;
					treeInstance.lightmapColor = Color.white;

					treeInstance.position = pos + itemMap[index].pos;
					treeInstance.prototypeIndex = treeIndex;

					//#if UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3
					//    Random.seed = (int)posSeed.x + ((int)posSeed.y * resolution);
					//#else
					//    Random.InitState((int)posSeed.x + ((int)posSeed.z * resolution));
					//#endif

					treeInstance.rotation = RandomPos(posSeed + new Vector2(225.5f, 350.5f)) * 360;

					Vector2 scaleRange = new Vector2(tree.scaleRange.x * item.parentItem.scaleMinMaxMulti.x, tree.scaleRange.y * item.parentItem.scaleMinMaxMulti.y);
					float scaleRangeDelta = scaleRange.y - scaleRange.x;
					if (scaleRangeDelta == 0) scaleRangeDelta = 0.001f;
					treeInstance.heightScale = (tree.scaleCurve.Evaluate(RandomPos(posSeed)) * scaleRangeDelta) + scaleRange.x;

					float scaleMulti = tree.scaleMulti * item.parentItem.scaleMulti;

					treeInstance.heightScale *= scaleMulti;
					if (item.parentItem.linkScaleToMask) treeInstance.heightScale *= Mathf.Lerp(1, itemMap[index].maskValue, item.parentItem.linkScaleToMaskAmount);
					if (treeInstance.heightScale < scaleRange.x * scaleMulti) treeInstance.heightScale = scaleRange.x * scaleMulti;
					treeInstance.widthScale = treeInstance.heightScale * ((RandomPos(posSeed + new Vector2(997.5f, 500.5f)) * tree.nonUniformScale * 2) + (1 - tree.nonUniformScale));
					trees.Add(treeInstance);
					++item.placed;

					// Octree.SpawnedObject obj = new Octree.SpawnedObject(index, new Vector3(treeInstance.position.x * terrainSize.x, treeInstance.position.y * terrainSize.y, treeInstance.position.z * terrainSize.z) + terrainPos);
					// octree.cell.AddObject(obj);
				}
			}

			area2D.currentTerrain.terrainData.treeInstances = trees.ToArray();
			float[,] height1 = area2D.currentTerrain.terrainData.GetHeights(0, 0, 1, 1);
			area2D.currentTerrain.terrainData.SetHeights(0, 0, height1);
			trees.Clear();

			area2D.terrainLayer.CalcTreePlaced();
		}

		public void ComputeObject()
		{
			// float minHeight = Mathf.Infinity, maxHeight = 0;
			if (objectParent != null) DestroyImmediate(objectParent);
			
			if (area2D.terrainLayer.layerGroups[TC.objectOutput] == null) return;
			if (area2D.terrainLayer.objectSelectItems.Count == 0)
			{
				TC.AddMessage("No objects nodes are active.");
				return;
			}
            //if (area2D.currentTCTerrain.texHeight == null)
            //{
            //    TC.AddMessage("You need to generate a heightmap first", 0, 7);
            //    return;
            //}

            if (firstObjectTerrain == area2D.currentTCTerrain) { area2D.terrainLayer.ResetPlaced(); firstObjectTerrain = null; }

			int resolution = area2D.intResolution.x;

			Transform objectsParent = CheckObjectsParent(area2D.currentTCUnityTerrain);

			if (assignTerrainHeightmap)
			{
				area2D.currentTCUnityTerrain.ResetObjects();
				area2D.terrainLayer.ResetObjects();
			}
			
			TC_Compute compute = TC_Compute.instance;
			compute.SetPreviewColors(compute.splatColors);

			ComputeBuffer itemMapBuffer = null;
			area2D.terrainLayer.layerGroups[TC.objectOutput].ComputeSingle(ref itemMapBuffer, 0, true);
			// compute.RunItemPositionCompute(itemMapBuffer, TC.objectOutput);

			ItemMap[] itemMap = new ItemMap[resolution * resolution];

			itemMapBuffer.GetData(itemMap);

			compute.DisposeBuffer(ref itemMapBuffer);
			
			// Vector2 terrainSize = area2D.area.size;
			// Vector2 terrainPos = area2D.area.position;
			Vector3 terrainSize = area2D.currentTerrain.terrainData.size;
			Vector3 terrainPos = area2D.currentTerrain.transform.position;
			Vector3 outputOffset = area2D.outputOffsetV3;

			// tcGenerate.ClearSpawnedObjects();
			// for (int i = 0; i < tcGenerate.objectItems.Length; i++) tcGenerate.objectItems[i].objectCount = 0;
			// Debug.Log(TCLayerLevel.current.objectitems.Count);
			// return;


			List<TC_SelectItem> objectItems = TC_Area2D.instance.terrainLayer.objectSelectItems;
			
			for (int y = 0; y < resolution; ++y)
			{
				for (int x = 0; x < resolution; ++x)
				{
					int index = (y * resolution) + x;
					float density = itemMap[index].density * itemMap[index].maskValue;

					if (density == 0) continue;

					Vector3 pos = new Vector3(((float)x / resolution), 0, ((float)y / resolution));
					Vector3 pos2 = pos + itemMap[index].pos;

					if (pos2.x < 0 || pos2.x > 1 || pos2.z < 0 || pos2.z > 1)
					{
					// Debug.Log(position.x + ", "+position.y+", "+position.z);
						continue;
					}

					// Debug.Log("x " + itemMap[index].pos.x + " z " + itemMap[index].pos.z);

					int id = itemMap[index].index;
					if (id > objectItems.Count - 1)
					{
						// Debug.Log("Object index is out of bounds, index = " + id);
						TC.AddMessage("Object index is out of bounds, index = " + id + ". Try the 'Refresh' button.");
						return;
					}

					TC_SelectItem item = objectItems[id];
					TC_SelectItem.SpawnObject spawnObject = item.spawnObject;

					// Debug.Log("x " + itemMap[index].pos.x + " z " + itemMap[index].pos.z);
					
					// Debug.Log(itemMap[index].pos);
					pos = new Vector3(pos.x * terrainSize.x, pos.y, pos.z * terrainSize.z) + terrainPos; // - item.t.parent.parent.position;
					
					Vector3 posSeed = Mathw.SnapVector3(pos + new Vector3(area2D.resolutionPM.x / 4, 0, area2D.resolutionPM.x / 4), area2D.resolutionPM.x / 2) - outputOffset;

					pos += new Vector3(itemMap[index].pos.x * terrainSize.x, 0, itemMap[index].pos.z * terrainSize.z);
					// Debug.Log(itemMap[index].pos.x +", "+itemMap[index].pos.z);

					if (spawnObject.includeTerrainHeight) pos.y = area2D.currentTerrain.SampleHeight(pos);
					else pos.y = 0;

					pos.y += TC_Settings.instance.generateOffset.y;
					
					// Debug.Log((posSeed.x - pos.x) + ", " + (posSeed.z - pos.z));

					#if UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3
						Random.seed = (int)posSeed.x + ((int)posSeed.z * resolution);
					#else
						Random.InitState((int)posSeed.x + ((int)posSeed.z * resolution));
					#endif

					Vector3 rotation;
					if (spawnObject.includeTerrainAngle)
					{
						Vector3 normal = area2D.currentTerrain.terrainData.GetInterpolatedNormal(pos2.x, pos2.z);
						rotation.x = normal.z * 90;
						rotation.y = 0;
						rotation.z = normal.x * -90;
					}
					else rotation = Vector3.zero;

					if (spawnObject.lookAtTarget != null)
					{
						rotation = Quaternion.LookRotation(spawnObject.lookAtTarget.position - pos).eulerAngles;
						if (!spawnObject.lookAtX) rotation.x = rotation.z = 0;
					}

					#if UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3
						Random.seed = (int)posSeed.x + ((int)posSeed.z * resolution);
					#else
						Random.InitState((int)posSeed.x + ((int)posSeed.z * resolution));
					#endif
					
					Vector3 scale;

					float scaleMulti = spawnObject.scaleMulti * item.parentItem.scaleMulti;

					if (!spawnObject.customScaleRange)
					{
						Vector2 scaleRange = new Vector2(spawnObject.scaleRange.x * item.parentItem.scaleMinMaxMulti.x, spawnObject.scaleRange.y * item.parentItem.scaleMinMaxMulti.y);

						float scaleRangeDelta = scaleRange.y - scaleRange.x;
						if (scaleRangeDelta == 0) scaleRangeDelta = 0.001f;
						scale.x = (spawnObject.scaleCurve.Evaluate(Random.value) * scaleRangeDelta) + scaleRange.x;

						scale.x *= scaleMulti;
						if (item.parentItem.linkScaleToMask) scale.x *= Mathf.Lerp(1, itemMap[index].maskValue, item.parentItem.linkScaleToMaskAmount);
						if (scale.x < scaleRange.x * scaleMulti) scale.x = scaleRange.x * scaleMulti;
						scale.y = scale.x * Random.Range(1 - spawnObject.nonUniformScale, 1 + spawnObject.nonUniformScale);
						scale.z = scale.x * Random.Range(1 - spawnObject.nonUniformScale, 1 + spawnObject.nonUniformScale);
					}
					else
					{
						Vector2 scaleRangeX = new Vector2(spawnObject.scaleRangeX.x * item.parentItem.scaleMinMaxMulti.x, spawnObject.scaleRangeX.y * item.parentItem.scaleMinMaxMulti.y);
						Vector2 scaleRangeY = new Vector2(spawnObject.scaleRangeY.x * item.parentItem.scaleMinMaxMulti.x, spawnObject.scaleRangeY.y * item.parentItem.scaleMinMaxMulti.y);
						Vector2 scaleRangeZ = new Vector2(spawnObject.scaleRangeZ.x * item.parentItem.scaleMinMaxMulti.x, spawnObject.scaleRangeZ.y * item.parentItem.scaleMinMaxMulti.y);

						float scaleRangeDeltaX = scaleRangeX.y - scaleRangeX.x;
						if (scaleRangeDeltaX == 0) scaleRangeDeltaX = 0.001f;
						float scaleRangeDeltaY = scaleRangeY.y - scaleRangeY.x;
						if (scaleRangeDeltaY == 0) scaleRangeDeltaY = 0.001f;
						float scaleRangeDeltaZ = scaleRangeZ.y - scaleRangeZ.x;
						if (scaleRangeDeltaZ == 0) scaleRangeDeltaZ = 0.001f;

						scale.x = (spawnObject.scaleCurve.Evaluate(Random.value) * scaleRangeDeltaX) + scaleRangeX.x;
						scale.y = (spawnObject.scaleCurve.Evaluate(Random.value) * scaleRangeDeltaY) + scaleRangeY.x;
						scale.z = (spawnObject.scaleCurve.Evaluate(Random.value) * scaleRangeDeltaZ) + scaleRangeZ.x;

						scale *= scaleMulti;
						if (item.parentItem.linkScaleToMask) scale *= Mathf.Lerp(1, itemMap[index].maskValue, item.parentItem.linkScaleToMaskAmount);
						if (scale.x < scaleRangeX.x * scaleMulti) scale.x = scaleRangeX.x * scaleMulti;
						if (scale.y < scaleRangeY.x * scaleMulti) scale.y = scaleRangeY.x * scaleMulti;
						if (scale.z < scaleRangeZ.x * scaleMulti) scale.z = scaleRangeZ.x * scaleMulti;
					}

					pos.y += spawnObject.heightOffset;
					if (spawnObject.includeScale) pos.y += Random.Range(spawnObject.heightRange.x, spawnObject.heightRange.y) * scale.y;
					else pos.y += Random.Range(spawnObject.heightRange.x, spawnObject.heightRange.y);

					GameObject go;
					Transform t;

					#if !UNITY_EDITOR
						go = (GameObject)Instantiate(spawnObject.go, pos, Quaternion.Euler(rotation));
						t = go.transform;
					#else
						// TODO rotation can be returned as Quaternion
						if (!spawnObject.linkToPrefab) {
							go = (GameObject)Instantiate(spawnObject.go, pos, Quaternion.Euler(rotation));
							t = go.transform;
						}
						else
						{
							go = (GameObject)UnityEditor.PrefabUtility.InstantiatePrefab(spawnObject.go);
							t = go.transform;
							t.position = pos;
							t.rotation = Quaternion.Euler(rotation);
						}
					#endif

					t.Rotate(new Vector3(Random.Range(spawnObject.rotRangeX.x, spawnObject.rotRangeX.y), Random.Range(spawnObject.rotRangeY.x, spawnObject.rotRangeY.y), Random.Range(spawnObject.rotRangeZ.x, spawnObject.rotRangeZ.y)), Space.Self);
					
					if (spawnObject.isSnapRot)
					{
						if (spawnObject.isSnapRotX) rotation.x = ((int)(rotation.x / spawnObject.snapRotX)) * spawnObject.snapRotX;
						if (spawnObject.isSnapRotY) rotation.y = ((int)(rotation.y / spawnObject.snapRotY)) * spawnObject.snapRotY;
						if (spawnObject.isSnapRotZ) rotation.z = ((int)(rotation.z / spawnObject.snapRotZ)) * spawnObject.snapRotZ;
					}

					//if (tcObject.spawnList.Count <= tcGenerate.objectItems[objectIndex].objectCount)
					//{
					
					go.name = spawnObject.go.name;

					if (spawnObject.parentMode == TC_SelectItem.SpawnObject.ParentMode.Terrain) t.parent = objectsParent;
					else if (spawnObject.parentMode == TC_SelectItem.SpawnObject.ParentMode.Existing) t.parent = spawnObject.parentT;
					else if (spawnObject.parentMode == TC_SelectItem.SpawnObject.ParentMode.Create)
					{
						if (spawnObject.newParentT == null)
						{
							GameObject parentGO = new GameObject(spawnObject.parentName);
							if (spawnObject.parentToTerrain) parentGO.transform.parent = objectsParent;
							spawnObject.newParentT = parentGO.transform;
						}
						t.parent = spawnObject.newParentT;
					}
						
					t.localScale = Vector3.Scale(spawnObject.go.transform.localScale, scale);
					++item.placed;

					// tcObject.spawnList.Add(go.transform);
					//}
					//else
					//{
					//    tcObject.spawnList[tcGenerate.objectItems[objectIndex].objectCount].position = position;
					//    tcObject.spawnList[tcGenerate.objectItems[objectIndex].objectCount].rotation = Quaternion.Euler(rotation);
					//    tcObject.spawnList[tcGenerate.objectItems[objectIndex].objectCount].localScale = scale;
					//    tcObject.spawnList[tcGenerate.objectItems[objectIndex].objectCount].gameObject.SetActive(true);
					//}

					// ++tcGenerate.objectItems[objectIndex].objectCount;
					//}
					// if (pos.y > maxHeight) maxHeight = pos.y;
					// else if (pos.y < minHeight) minHeight = pos.y;

				}
			}

			// Debug.Log("Min height " + minHeight + " Max height " + maxHeight);

			area2D.terrainLayer.CalcObjectPlaced();

			//for (int j = 0; j < tcGenerate.objectItems.Length; ++j)
			//{
			//    for (int i = tcGenerate.objectItems[j].objectCount; i < tcObjects[j].spawnList.Count; i++) tcObjects[j].spawnList[i].gameObject.SetActive(false);
			//}
		}

		public float RandomPos(Vector2 pos)
		{
			return Mathw.Frac(Mathf.Sin(Vector2.Dot(pos, new Vector2(12.9898f, 78.233f))) * 43758.5453123f);
		}

		public Transform CheckObjectsParent(TCUnityTerrain tcUnityTerrain)
		{
			if (tcUnityTerrain.objectsParent == null)
			{
				tcUnityTerrain.objectsParent = new GameObject("TerrainComposer Objects").transform;
				tcUnityTerrain.objectsParent.parent = tcUnityTerrain.terrain.transform;
			}

			return tcUnityTerrain.objectsParent;
        }
        
        #pragma warning disable 0649
        struct ItemMap
		{
			public int index;
			public float density;
			public float maskValue;
			public Vector3 pos;
		};
        #pragma warning restore 0649

        [System.Serializable]
		public class GenerateStackEntry
		{
			public List<GenerateStack> stack = new List<GenerateStack>();
			public int frame;

			public GenerateStackEntry(int frame)
			{
				this.frame = frame;
			}
		}
		
		[System.Serializable]
		public class GenerateStack
		{
			public TCUnityTerrain tcTerrain;
			public int outputId;
			public bool assignTerrainHeightmap;
			
			public Rect generateRect;

			public GenerateStack(int outputId, TCUnityTerrain tcTerrain, bool assignTerrainHeightmap)
			{
				this.tcTerrain = tcTerrain;
				this.outputId = outputId;
				this.assignTerrainHeightmap = assignTerrainHeightmap;
			}

			public GenerateStack(int outputId, TCUnityTerrain tcTerrain, bool assignTerrainHeightmap, Rect generateRect)
			{
				this.tcTerrain = tcTerrain;
				this.outputId = outputId;
				this.assignTerrainHeightmap = assignTerrainHeightmap;
				this.generateRect = generateRect;
			}
		}
	}
}