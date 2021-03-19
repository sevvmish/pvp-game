using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Reflection;
#if !UNITY_5_1 && !UNITY_5_2
using UnityEditor.SceneManagement;
#endif

namespace TerrainComposer2
{
    [CustomEditor(typeof(TC_TerrainArea), true)]
    public class TC_TerrainAreaEditor : Editor
    {
        Event eventCurrent;

        TC_TerrainArea terrainArea;
        TC_Settings settings;
        TC_GlobalSettings g;
        bool guiChanged;

        void OnEnable()
        {
            if (target == null) return;

            terrainArea = (TC_TerrainArea)target;
            settings = TC_Settings.instance;
            if (settings != null) g = settings.global;
            if (terrainArea != null) terrainArea.GetAll(); 
            TC.GetInstallPath();
        }

        void OnDisable()
        {
            if (target == null || settings == null || TC_Generate.instance == null) return;
            
            if (TC_Generate.instance.CheckForTerrain(false)) 
            {
                if (settings.masterTerrain != null) Apply();
            }
        }

        public override void OnInspectorGUI()
        {
            if (g == null)
            {
                EditorGUILayout.LabelField("Global Settings are not found.");
                return;
            }
            terrainArea = (TC_TerrainArea)target;

            terrainArea.transform.localScale = Vector3.one;

            eventCurrent = Event.current;

            if (button_splatmap == null) LoadButtonTextures();

            GUI.changed = false;

            GUILayout.Space(5);
            DrawCreateTerrain();
            GUILayout.Space(5);
            if (GUILayout.Button("Scan for Terrains", GUILayout.Width(150)))
            {
                terrainArea.GetTerrainsFromChildren();
                terrainArea.GetAll();
                return;
            }

            #if !UNITY_5 && !UNITY_2017 && !UNITY_2018_1 && !UNITY_2018_2
            GUILayout.Space(10);
                DrawPerformance();
            #endif
            GUILayout.Space(10);
                DrawTerrainAreaTiles();
            GUILayout.Space(10);

            DrawRTP();
            
            DrawTerrain(terrainArea.terrainSelect, -15);

            if (GUI.changed)
            {
                // Debug.Log("Set Dirty");
                EditorUtility.SetDirty(terrainArea);
                #if !UNITY_5_1 && !UNITY_5_2
                    if (!Application.isPlaying) EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                #else
                    EditorApplication.MarkSceneDirty();
                #endif
            }

            if (TC_Settings.instance == null) return; 
            if (TC_Settings.instance.drawDefaultInspector) base.OnInspectorGUI();
        }

        string tooltipText = ""; // !

        TCUnityTerrain currentTerrain; // !
        Rect rtp_rect1; // !

        Texture button_grass, button_reset, button_resolution, button_settings, button_size, button_splatmap, button_tree, button_folder; // !

        public void create_preview_window(Texture2D texture, string text) { } // !
        public void create_select_window(int mode) { } // !


        public void DrawRTP()
        {
            Type t = TC.FindRTP();

            if (t == null) return;

            TD.DrawSpacer();

            TD.DrawLabelWidthUnderline("RTP Detected", 14);

            bool addRTPButton = !terrainArea.IsRTPAddedToTerrains();

            if (addRTPButton)
            {
                if (GUILayout.Button("Enable RTP",GUILayout.Width(100)))
                {
                    terrainArea.AddRTPTOTerrains();
                }
            }
            else
            {
                GUI.changed = false;
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel("Assign colormap");
                settings.autoColormapRTP = EditorGUILayout.Toggle(settings.autoColormapRTP, GUILayout.Width(25));
                if (GUILayout.Button("Reset", EditorStyles.miniButtonMid, GUILayout.Width(55)))
                {
                    terrainArea.ResetTextureRTP("ColorGlobal");
                }
                EditorGUILayout.EndHorizontal();


                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel("Assign normalmap");
                settings.autoNormalmapRTP = EditorGUILayout.Toggle(settings.autoNormalmapRTP, GUILayout.Width(25));
                if (GUILayout.Button("Reset", EditorStyles.miniButtonMid, GUILayout.Width(55)))
                {
                    terrainArea.ResetTextureRTP("NormalGlobal");
                }
                EditorGUILayout.EndHorizontal();
                if (GUI.changed) EditorUtility.SetDirty(settings);
            }

            TD.DrawSpacer();
        }
        
        public void DrawCreateTerrain()
        {
            EditorGUILayout.BeginHorizontal();
            if (terrainArea.createTerrainTab) GUI.backgroundColor = Color.green;
            if (GUILayout.Button("Create Terrain", EditorStyles.miniButtonMid, GUILayout.Width(100)))
            {
                terrainArea.createTerrainTab = !terrainArea.createTerrainTab;
            }
            GUI.backgroundColor = Color.white;
            
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(5);

            if (terrainArea.createTerrainTab)
            {
                TD.DrawLabelWidthUnderline("Create Terrain", 14);
                DrawCreateTerrain(0, 0);
            }
        }

        #if !UNITY_5 && !UNITY_2017 && !UNITY_2018_1 && !UNITY_2018_2
        public void DrawPerformance()
        {
            TD.DrawLabelWidthUnderline("Performance", 14);

            GUI.color = Color.green;
            EditorGUILayout.BeginVertical("Box");
            GUI.color = Color.white;
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Draw Instanced", GUILayout.Width(160));
            GUI.changed = false;
            terrainArea.drawInstanced = EditorGUILayout.Toggle(terrainArea.drawInstanced, GUILayout.Width(25));
            if (GUI.changed)
            {
                if (terrainArea.terrains.Count > 0 && terrainArea.terrains[0].terrain != null) terrainArea.terrains[0].terrain.drawInstanced = terrainArea.drawInstanced;
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
        }
        #endif
        
        public void DrawTerrainAreaTiles()
        {
            int countTiles = terrainArea.tiles.x * terrainArea.tiles.y;

            if (countTiles <= 1) return;
            
            int terrainIndex = 0;
            for (int y = terrainArea.tiles.y - 1; y >= 0; y--)
            {
                EditorGUILayout.BeginHorizontal();
                for (int x = 0; x < terrainArea.tiles.x; x++)
                {
                    terrainIndex = x + (y * terrainArea.tiles.x);
                    TCUnityTerrain tcTerrain = terrainArea.terrains[terrainIndex];

                    if (terrainArea.terrainSelect == terrainIndex)
                    {
                        if (tcTerrain.active) GUI.backgroundColor = Color.green; else GUI.backgroundColor = Color.yellow;
                    }
                    else if (tcTerrain.active) GUI.backgroundColor = Color.white; else GUI.backgroundColor = Color.red;

                    if (GUILayout.Button("", EditorStyles.miniButtonMid, GUILayout.Width(25), GUILayout.Height(25)))
                    {
                        if (eventCurrent.button == 2)
                        {
                            tcTerrain.active = !tcTerrain.active;

                            if (Event.current.shift)
                            {
                                for (int i = 0; i < terrainArea.terrains.Count; i++)
                                {
                                    TCUnityTerrain tcTerrain2 = terrainArea.terrains[i];
                                    if (tcTerrain2.terrain != null)
                                    {
                                        tcTerrain2.terrain.gameObject.SetActive(tcTerrain.active);
                                        tcTerrain2.active = tcTerrain.active;
                                    }
                                }
                            }
                            else
                            {
                                if (tcTerrain.terrain != null) tcTerrain.terrain.gameObject.SetActive(tcTerrain.active);
                            }
                            if (tcTerrain.active) TC.AutoGenerate();
                            continue;
                        }

                        terrainArea.terrainSelect = terrainIndex;
                        terrainArea.GetAll();
                        tcTerrain = terrainArea.terrains[terrainIndex];

                        if (eventCurrent.button == 1)
                        {
                            if (tcTerrain.terrain != null) Selection.activeTransform = tcTerrain.terrain.transform;
                            eventCurrent.Use();
                        }
                    }
                    GUILayout.Space(3);
                }
                EditorGUILayout.EndHorizontal();
            }

            GUI.backgroundColor = Color.white;
        }

        TerrainCollider terrainCollider;

        bool hasTerrain;
        bool hasTerrainData;
        bool validTerrain;

        public void ChangeTerrainData(Terrain terrain)
        {
            terrainCollider = terrain.GetComponent<TerrainCollider>();
            if (terrainCollider != null) terrainCollider.terrainData = terrain.terrainData;
        }

        public void DrawTerrain(int index, float space)
        {
            if (terrainArea.terrains == null) { terrainArea.createTerrainTab = true; return; }
            if (terrainArea.terrains.Count == 0) { terrainArea.createTerrainTab = true; return; }
            
            if (index > terrainArea.terrains.Count - 1) return;

            TD.DrawLabelWidthUnderline("Terrain Setup", 14);

            EditorGUILayout.BeginVertical("Box");
            GUILayout.Space(5);

            currentTerrain = terrainArea.terrains[index];
            hasTerrain = currentTerrain.terrain != null;

            if (!hasTerrain) hasTerrainData = false; else hasTerrainData = currentTerrain.terrain.terrainData != null;
            if (hasTerrain && hasTerrainData) validTerrain = true; else validTerrain = false;

            // TODO put somewhere else
            //EditorGUILayout.BeginHorizontal();
            //GUILayout.Space(space + 15));
            //if (hasTerrain) { EditorGUILayout.LabelField("Trees placed: " + currentTerrain.terrain.terrainData.treeInstances.Length, GUILayout.Width(250.0f)); }
            //EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(space + 15);

            space -= 15; 
            // terrain foldout
            currentTerrain.terrain = EditorGUILayout.ObjectField(currentTerrain.terrain, typeof(Terrain), true) as Terrain;
            
            if (hasTerrain && !hasTerrainData) EditorGUILayout.LabelField("Missing TerrainData. Fix this manually in the Scene or create a new terrain");
            
            EditorGUILayout.LabelField("Act", GUILayout.Width(28.0f));
            currentTerrain.active = EditorGUILayout.Toggle(currentTerrain.active, GUILayout.Width(25.0f));

            EditorGUILayout.EndHorizontal();

            // if (!hasTerrainData) return;

            EditorGUILayout.BeginHorizontal();
            GUI.backgroundColor = terrainArea.sizeTab ? Color.green : Color.white;
            if (GUILayout.Button(new GUIContent("Size", button_size), EditorStyles.miniButtonMid, GUILayout.Width(95.0f), GUILayout.Height(19.0f)))
            {
                if (!terrainArea.sizeTab)
                {
                    CloseTerrainTabs();
                    terrainArea.sizeTab = true;
                    currentTerrain.GetSize();
                }
                else CloseTerrainTabs();
            }

            GUI.backgroundColor = terrainArea.resolutionsTab ? Color.green : Color.white;
            if (GUILayout.Button(new GUIContent("Resolutions", button_resolution), EditorStyles.miniButtonMid, GUILayout.Width(95.0f), GUILayout.Height(19.0f)))
            {
                if (!terrainArea.resolutionsTab)
                {
                    CloseTerrainTabs();
                    terrainArea.resolutionsTab = true;
                    currentTerrain.GetResolutions();
                }
                else CloseTerrainTabs();
            }

            GUI.backgroundColor = terrainArea.resetTab ? Color.green : Color.white;
            if (GUILayout.Button(new GUIContent("Reset", button_reset), EditorStyles.miniButtonMid, GUILayout.Width(95.0f), GUILayout.Height(19.0f)))
            {
                if (!terrainArea.resetTab)
                {
                    CloseTerrainTabs();
                    terrainArea.resetTab = true;
                }
                else CloseTerrainTabs();
            }
            GUI.backgroundColor = terrainArea.settingsTab ? Color.green : Color.white;
            if (GUILayout.Button(new GUIContent("Settings", button_settings), EditorStyles.miniButtonMid, GUILayout.Width(95.0f), GUILayout.Height(19.0f)))
            {
                if (!terrainArea.settingsTab)
                {
                    CloseTerrainTabs();
                    terrainArea.settingsTab = true;
                }
                else CloseTerrainTabs();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(space + 30);
            GUI.backgroundColor = terrainArea.splatTab ? Color.green : Color.white;
            if (GUILayout.Button(new GUIContent("Splat Textures", button_splatmap), EditorStyles.miniButtonMid, GUILayout.Width(95.0f), GUILayout.Height(19.0f)))
            {
                if (!terrainArea.splatTab)
                {
                    CloseTerrainTabs();
                    terrainArea.splatTab = true;
                    currentTerrain.GetSplatTextures(terrainArea);
                }
                else CloseTerrainTabs();
            }

            GUI.backgroundColor = terrainArea.treeTab ? Color.green : Color.white;
            if (GUILayout.Button(new GUIContent("Trees", button_tree), EditorStyles.miniButtonMid, GUILayout.Width(95.0f), GUILayout.Height(19.0f)))
            {
                if (!terrainArea.treeTab)
                {
                    CloseTerrainTabs();
                    terrainArea.treeTab = true;
                    currentTerrain.GetTrees();
                }
                else CloseTerrainTabs();
            }

            GUI.backgroundColor = terrainArea.grassTab ? Color.green : Color.white;
            if (GUILayout.Button(new GUIContent("Grass/Details", button_grass), EditorStyles.miniButtonMid, GUILayout.Width(95.0f), GUILayout.Height(19.0f)))
            {
                if (!terrainArea.grassTab)
                {
                    CloseTerrainTabs();
                    terrainArea.grassTab = true;
                    currentTerrain.GetGrass();
                }
                else CloseTerrainTabs();
            }
            GUI.backgroundColor = Color.white;
            EditorGUILayout.EndHorizontal();


            GUILayout.Space(10);
            


            // Size Tab
            if (terrainArea.sizeTab)
            {
                TD.DrawLabelWidthUnderline("Terrain Size", 14);
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(30 + space);
                EditorGUILayout.LabelField("Size", GUILayout.Width(100));
                guiChanged = GUI.changed;
                GUI.changed = false;
                terrainArea.terrainSize = EditorGUILayout.Vector3Field("", terrainArea.terrainSize);
                if (GUI.changed)
                {
                    if (terrainArea.terrainSize.x < 1) terrainArea.terrainSize.x = 1;
                    if (terrainArea.terrainSize.y < 1) terrainArea.terrainSize.y = 1;
                    guiChanged = true;
                }
                GUI.changed = guiChanged;

                terrainArea.terrainSize.z = terrainArea.terrainSize.x;
                EditorGUILayout.EndHorizontal();
            }

            // Resolutions Tab
            if (terrainArea.resolutionsTab)
            {
                TD.DrawLabelWidthUnderline("Terrain Resolutions", 14);
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(30 + space);
                EditorGUILayout.LabelField("Heightmap Resolution", GUILayout.Width(135.0f));
                guiChanged = GUI.changed;
                GUI.changed = false;
                currentTerrain.heightmapResolutionList = (int)GUILayout.HorizontalSlider((float)currentTerrain.heightmapResolutionList, 9.0f, 0.0f, GUILayout.Width(210.0f));
                if (GUI.changed)
                {
                    if (currentTerrain.heightmapResolutionList > 7) { currentTerrain.heightmapResolutionList = 7; }
                    guiChanged = true;
                }
                GUI.changed = guiChanged;
                currentTerrain.heightmapResolutionList = EditorGUILayout.Popup(currentTerrain.heightmapResolutionList, TC_TerrainArea.heightmapResolutionList, GUILayout.Width(70.0f));
                if (terrainArea.terrains.Count > 1) EditorGUILayout.LabelField("(" + (terrainArea.tiles.x * currentTerrain.heightmapResolution).ToString() + ")");
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(30 + space);
                EditorGUILayout.LabelField("Splatmap Resolution", GUILayout.Width(135.0f));
                int list = currentTerrain.splatmapResolutionList + 1;
                guiChanged = GUI.changed;
                GUI.changed = false;
                list = (int)GUILayout.HorizontalSlider((float)list, 9.0f, 0.0f, GUILayout.Width(210.0f));
                if (GUI.changed)
                {
                    if (list > 8) { list = 8; }
                    if (list < 1) { list = 1; }
                    guiChanged = true;
                }
                GUI.changed = guiChanged;
                currentTerrain.splatmapResolutionList = list - 1;
                currentTerrain.splatmapResolutionList = EditorGUILayout.Popup(currentTerrain.splatmapResolutionList, TC_TerrainArea.splatmapResolutionList, GUILayout.Width(70.0f));
                if (terrainArea.terrains.Count > 1) EditorGUILayout.LabelField("(" + (terrainArea.tiles.x * currentTerrain.splatmapResolution).ToString() + ")");
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(30 + space);
                EditorGUILayout.LabelField("Basemap Resolution", GUILayout.Width(135.0f));
                list = currentTerrain.basemapResolutionList + 1;
                guiChanged = GUI.changed;
                GUI.changed = false;
                list = (int)GUILayout.HorizontalSlider((float)list, 9.0f, 0.0f, GUILayout.Width(210.0f));
                if (GUI.changed)
                {
                    if (list > 8) { list = 8; }
                    if (list < 1) { list = 1; }
                    guiChanged = true;
                }
                GUI.changed = guiChanged;
                currentTerrain.basemapResolutionList = list - 1;
                currentTerrain.basemapResolutionList = EditorGUILayout.Popup(currentTerrain.basemapResolutionList, TC_TerrainArea.splatmapResolutionList, GUILayout.Width(70.0f));
                if (terrainArea.terrains.Count > 1) EditorGUILayout.LabelField("(" + (terrainArea.tiles.x * currentTerrain.basemapResolution).ToString() + ")");
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(30 + space);
                EditorGUILayout.LabelField("Grass Resolution", GUILayout.Width(135 + 214.0f));
                guiChanged = GUI.changed;
                GUI.changed = false;
                currentTerrain.detailResolution = EditorGUILayout.IntField(currentTerrain.detailResolution, GUILayout.Width(70.0f));
                if (GUI.changed)
                {
                    if (currentTerrain.detailResolution < 16) { currentTerrain.detailResolution = 16; }
                    guiChanged = true;
                }
                GUI.changed = guiChanged;
                if (terrainArea.terrains.Count > 1) EditorGUILayout.LabelField("(" + (terrainArea.tiles.x * currentTerrain.detailResolution).ToString() + ")");
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(30 + space);
                EditorGUILayout.LabelField("Grass Per Patch", GUILayout.Width(135.0f));
                list = currentTerrain.detailResolutionPerPatchList;
                guiChanged = GUI.changed;
                GUI.changed = false;
                list = (int)GUILayout.HorizontalSlider((float)list, 0.0f, 4.0f, GUILayout.Width(210.0f));
                if (GUI.changed)
                {
                    if (list < 0) { list = 0; }
                    guiChanged = true;
                }
                GUI.changed = guiChanged;
                currentTerrain.detailResolutionPerPatchList = list;
                currentTerrain.detailResolutionPerPatchList = EditorGUILayout.Popup(currentTerrain.detailResolutionPerPatchList, TC_TerrainArea.detailResolutionPerPatchList, GUILayout.Width(70.0f));
                EditorGUILayout.EndHorizontal();
            }

            // Settings Tab
            if (terrainArea.settingsTab)
            {
                TD.DrawLabelWidthUnderline("Terrain Settings", 14);
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(30 + space);
                GUI.backgroundColor = terrainArea.settingsEditor ? Color.green : Color.white;
                if (GUILayout.Button("Editor", EditorStyles.miniButtonMid, GUILayout.Width(70.0f)))
                {
                    terrainArea.settingsEditor = true;
                    terrainArea.GetSettings();
                }
                GUI.backgroundColor = !terrainArea.settingsEditor ? Color.green : Color.white;
                if (GUILayout.Button("Runtime", EditorStyles.miniButtonMid, GUILayout.Width(70.0f)))
                {
                    terrainArea.settingsEditor = false;
                    terrainArea.GetSettings();
                }
                GUI.backgroundColor = Color.white;
                EditorGUILayout.EndHorizontal();

                if (validTerrain)
                {
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(30 + space);
                    EditorGUILayout.LabelField("Terrain Data", GUILayout.Width(160.0f));
                    guiChanged = GUI.changed;
                    GUI.changed = false;
                    currentTerrain.terrain.terrainData = (TerrainData)EditorGUILayout.ObjectField(currentTerrain.terrain.terrainData, typeof(TerrainData), false);
                    if (GUI.changed)
                    {
                        ChangeTerrainData(currentTerrain.terrain);
                        guiChanged = true;
                    }

                    GUI.changed = guiChanged;
                    EditorGUILayout.EndHorizontal();
                }

                guiChanged = GUI.changed;
                GUI.changed = false;

                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(45 + space);
                EditorGUILayout.LabelField("Base Terrain", EditorStyles.boldLabel);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(60 + space);
                EditorGUILayout.LabelField("Draw", GUILayout.Width(147.0f));
                currentTerrain.drawHeightmap = EditorGUILayout.Toggle(currentTerrain.drawHeightmap, GUILayout.Width(25.0f));
                EditorGUILayout.EndHorizontal();


                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(60 + space);
                GUI.color = !terrainArea.settingsEditor ? Color.green : Color.white;
                EditorGUILayout.LabelField("Pixel Error", GUILayout.Width(147.0f));
                currentTerrain.heightmapPixelError = EditorGUILayout.Slider(currentTerrain.heightmapPixelError, 1.0f, 200.0f);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(60 + space);
                GUI.color = !terrainArea.settingsEditor ? Color.green : Color.white;
                EditorGUILayout.LabelField("Heightmap Max LOD", GUILayout.Width(147.0f));
                currentTerrain.heightmapMaximumLOD = (int)EditorGUILayout.Slider((float)currentTerrain.heightmapMaximumLOD, 0.0f, 10.0f);
                EditorGUILayout.EndHorizontal();

                GUI.color = Color.white;
                // !! if (!current_terrain.rtp_script) {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(60 + space);
                EditorGUILayout.LabelField("Basemap Distance", GUILayout.Width(147.0f));
                currentTerrain.basemapDistance = EditorGUILayout.FloatField(currentTerrain.basemapDistance);
                if (currentTerrain.basemapDistance < 1) currentTerrain.basemapDistance = 1;
                EditorGUILayout.EndHorizontal();
                //}

                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(60 + space);
#if UNITY_5 || UNITY_2017 || UNITY_2018_1 || UNITY_2018_2 || UNITY_2018_3 || UNITY_2018_4
                EditorGUILayout.LabelField("Cast Shadows", GUILayout.Width(147.0f));
                currentTerrain.castShadows = EditorGUILayout.Toggle(currentTerrain.castShadows, GUILayout.Width(25.0f));
#else
                EditorGUILayout.LabelField("Shadow Casting Mode", GUILayout.Width(147.0f)); 
                currentTerrain.shadowCastingMode = (UnityEngine.Rendering.ShadowCastingMode)EditorGUILayout.EnumPopup(currentTerrain.shadowCastingMode);
#endif
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(60 + space);
                EditorGUILayout.LabelField("Material", GUILayout.Width(147.0f));
#if UNITY_5 || UNITY_2017 || UNITY_2018_1 || UNITY_2018_2 || UNITY_2018_3 || UNITY_2019_1
                currentTerrain.materialType = (Terrain.MaterialType)EditorGUILayout.EnumPopup(currentTerrain.materialType);
                EditorGUILayout.EndHorizontal();

                if (currentTerrain.materialType == Terrain.MaterialType.BuiltInStandard)
                {
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(60 + space);
                    EditorGUILayout.LabelField("Reflection Probes", GUILayout.Width(147.0f));
                    currentTerrain.reflectionProbeUsage = (UnityEngine.Rendering.ReflectionProbeUsage)EditorGUILayout.EnumPopup(currentTerrain.reflectionProbeUsage);
                    EditorGUILayout.EndHorizontal();
                }
                else if (currentTerrain.materialType == Terrain.MaterialType.BuiltInLegacySpecular)
                {
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(75 + space);
                    EditorGUILayout.LabelField("Specular Color", GUILayout.Width(147.0f));
                    currentTerrain.legacySpecular = EditorGUILayout.ColorField(currentTerrain.legacySpecular);
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(75 + space);
                    EditorGUILayout.LabelField("Shininess", GUILayout.Width(147.0f));
                    currentTerrain.legacyShininess = EditorGUILayout.FloatField(currentTerrain.legacyShininess);
                    EditorGUILayout.EndHorizontal();
                }
                else if (currentTerrain.materialType == Terrain.MaterialType.Custom && currentTerrain.terrain != null) 
                {
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(75 + space);
                    EditorGUILayout.LabelField("Custom Material", GUILayout.Width(147.0f));
                    currentTerrain.terrain.materialTemplate = (Material)EditorGUILayout.ObjectField(currentTerrain.terrain.materialTemplate, typeof(Material), true);
                    EditorGUILayout.EndHorizontal();
                }
#else
                    currentTerrain.materialTemplate = (Material)EditorGUILayout.ObjectField(currentTerrain.materialTemplate, typeof(Material), true);
                    EditorGUILayout.EndHorizontal(); 
#endif

                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(60 + space);
                EditorGUILayout.LabelField("Thickness", GUILayout.Width(147.0f));
                currentTerrain.thickness = EditorGUILayout.FloatField(currentTerrain.thickness);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(45 + space);
                EditorGUILayout.LabelField("Tree & Detail Terrain", EditorStyles.boldLabel);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(60 + space);
                EditorGUILayout.LabelField("Collect Detail Patches", GUILayout.Width(147.0f));
                currentTerrain.collectDetailPatches = EditorGUILayout.Toggle(currentTerrain.collectDetailPatches, GUILayout.Width(25.0f));
                EditorGUILayout.EndHorizontal();

                GUI.color = !terrainArea.settingsEditor ? Color.green : Color.white;
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(60 + space);
                EditorGUILayout.LabelField("Draw", GUILayout.Width(147.0f));
                currentTerrain.drawTreesAndFoliage = EditorGUILayout.Toggle(currentTerrain.drawTreesAndFoliage, GUILayout.Width(25.0f));
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(60 + space);
                EditorGUILayout.LabelField("Grass Distance", GUILayout.Width(147.0f));
                if (terrainArea.settingsEditor) currentTerrain.detailObjectDistance = EditorGUILayout.Slider(currentTerrain.detailObjectDistance, 0.0f, 250f);
                else currentTerrain.detailObjectDistance = EditorGUILayout.Slider(currentTerrain.detailObjectDistance, 0.0f, 500f);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(60 + space);
                EditorGUILayout.LabelField("Grass Density", GUILayout.Width(147.0f));
                currentTerrain.detailObjectDensity = EditorGUILayout.Slider(currentTerrain.detailObjectDensity, 0.0f, 1.0f);
                EditorGUILayout.EndHorizontal();

                GUILayout.Space(2.0f);

                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(60 + space);
                EditorGUILayout.LabelField("Tree Distance", GUILayout.Width(147.0f));
                if (terrainArea.settingsEditor) currentTerrain.treeDistance = EditorGUILayout.Slider(currentTerrain.treeDistance, 0.0f, 50000);
                else currentTerrain.treeDistance = EditorGUILayout.Slider(currentTerrain.treeDistance, 0.0f, 150000);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(60 + space);
                EditorGUILayout.LabelField("Billboard Start", GUILayout.Width(147.0f));
                currentTerrain.treeBillboardDistance = EditorGUILayout.Slider(currentTerrain.treeBillboardDistance, 0.0f, 2000.0f);
                EditorGUILayout.EndHorizontal();
                GUI.color = Color.white;

                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(60 + space);
                EditorGUILayout.LabelField("Fade Length", GUILayout.Width(147.0f));
                if (terrainArea.settingsEditor) currentTerrain.treeCrossFadeLength = EditorGUILayout.Slider(currentTerrain.treeCrossFadeLength, 0.0f, 200);
                else currentTerrain.treeCrossFadeLength = EditorGUILayout.Slider(currentTerrain.treeCrossFadeLength, 0.0f, 100);
                EditorGUILayout.EndHorizontal();

                GUI.color = !terrainArea.settingsEditor ? Color.green : Color.white;
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(60 + space);
                EditorGUILayout.LabelField("Max Mesh Trees", GUILayout.Width(147.0f));
                if (terrainArea.settingsEditor) currentTerrain.treeMaximumFullLODCount = (int)EditorGUILayout.Slider((float)currentTerrain.treeMaximumFullLODCount, 0.0f, 10000);
                else currentTerrain.treeMaximumFullLODCount = (int)EditorGUILayout.Slider((float)currentTerrain.treeMaximumFullLODCount, 0.0f, 10000);
                EditorGUILayout.EndHorizontal();
                GUI.color = Color.white;

                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(45 + space);
                EditorGUILayout.LabelField("Wind Settings", EditorStyles.boldLabel);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(60 + space);
                EditorGUILayout.LabelField("Speed", GUILayout.Width(147.0f));
                currentTerrain.wavingGrassStrength = EditorGUILayout.Slider(currentTerrain.wavingGrassStrength, 0.0f, 1.0f);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(60 + space);
                EditorGUILayout.LabelField("Size", GUILayout.Width(147.0f));
                currentTerrain.wavingGrassSpeed = EditorGUILayout.Slider(currentTerrain.wavingGrassSpeed, 0.0f, 1.0f);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(60 + space);
                EditorGUILayout.LabelField("Bending", GUILayout.Width(147.0f));
                currentTerrain.wavingGrassAmount = EditorGUILayout.Slider(currentTerrain.wavingGrassAmount, 0.0f, 1.0f);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(60 + space);
                EditorGUILayout.LabelField("Grass Tint", GUILayout.Width(147.0f));
                currentTerrain.wavingGrassTint = EditorGUILayout.ColorField(currentTerrain.wavingGrassTint);
                EditorGUILayout.EndHorizontal();

                if (GUI.changed)
                {
                    terrainArea.ApplySettings();
                    guiChanged = true;
                }
                GUI.changed = guiChanged;
            }

            // Splat Tab
            if (terrainArea.splatTab)
            {
                //if (settings.isRTPDetected)
                //{
                //    EditorGUILayout.LabelField("Splat textures need to be setup in the ReliefTerrain script.");
                //    EditorGUILayout.LabelField("Click on a terrain to see it.");
                //    EditorGUILayout.EndVertical();
                //    return;
                //}
                
                TD.DrawLabelWidthUnderline("Terrain Splat Textures", 14);
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(30 + space);

                if (currentTerrain.splatSettingsFoldout) GUI.backgroundColor = Color.green;
                if (GUILayout.Button("Settings", GUILayout.Width(70)))
                {
                    currentTerrain.splatSettingsFoldout = !currentTerrain.splatSettingsFoldout;
                }
                GUI.backgroundColor = Color.white;

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(30 + space);
                    // if (currentTerrain.splatPrototypes.Count == 0)
                    //{
                        if (TC_Settings.instance.global.tooltip) tooltipText = "Add a new Splat Texture";
                        if (GUILayout.Button(new GUIContent("+", tooltipText), GUILayout.Width(25.0f)))
                        {
                            UndoRegister("Add Splat Texture");
                            currentTerrain.AddSplatTexture(currentTerrain.splatPrototypes.Count);
                        }
                    //}

                    if (TC_Settings.instance.global.tooltip) tooltipText = "Enable/Disable Colormap as splat texture\n(Shift Click)";

                    GUI.backgroundColor = Color.white;
                EditorGUILayout.EndHorizontal();

                GUILayout.Space(5);

                guiChanged = GUI.changed;
                GUI.changed = false;

                for (int countSplat = 0; countSplat < currentTerrain.splatPrototypes.Count; ++countSplat)
                {
                    if (countSplat == 0)
                    {
                        EditorGUILayout.BeginHorizontal();
                            GUILayout.Space(59 + space);
                            EditorGUILayout.LabelField("Splat", GUILayout.Width(55.0f));
                            EditorGUILayout.LabelField("Normal", GUILayout.Width(55.0f));
                        EditorGUILayout.EndHorizontal();
                    }

                    EditorGUILayout.BeginHorizontal();
                        GUILayout.Space(30 + space);
                        EditorGUILayout.LabelField("" + (countSplat + 1) + ")", GUILayout.Width(25.0f));

                        currentTerrain.splatPrototypes[countSplat].texture = (Texture2D)EditorGUILayout.ObjectField(currentTerrain.splatPrototypes[countSplat].texture, typeof(Texture), true, GUILayout.Width(55.0f), GUILayout.Height(55.0f));
                        currentTerrain.splatPrototypes[countSplat].normalMap = (Texture2D)EditorGUILayout.ObjectField(currentTerrain.splatPrototypes[countSplat].normalMap, typeof(Texture), true, GUILayout.Width(55.0f), GUILayout.Height(55.0f));

                   
                        if (TC_Settings.instance.global.tooltip) tooltipText = "Move Splat Texture up";
                        if (countSplat > 0)
                        {
                            if (GUILayout.Button(new GUIContent("▲", tooltipText), GUILayout.Width(25.0f))) currentTerrain.SwapSplatTexture(countSplat, countSplat - 1);
                        }
                        else GUILayout.Space(29.0f);

                        if (countSplat < currentTerrain.splatPrototypes.Count - 1)
                        {
                            if (TC_Settings.instance.global.tooltip) tooltipText = "Move Splat Texture down";
                            if (GUILayout.Button(new GUIContent("▼", tooltipText), GUILayout.Width(25.0f))) currentTerrain.SwapSplatTexture(countSplat, countSplat + 1);
                        }
                        else GUILayout.Space(29.0f);

                        if (TC_Settings.instance.global.tooltip) tooltipText = "Insert a new Splat Texture";

                        if (GUILayout.Button(new GUIContent("+", tooltipText), GUILayout.Width(25.0f))) currentTerrain.AddSplatTexture(countSplat + 1);
                        if (TC_Settings.instance.global.tooltip) tooltipText = "Erase this Splat Texture";

                        if (GUILayout.Button(new GUIContent("-", tooltipText), GUILayout.Width(25.0f)))
                        {
                            currentTerrain.EraseSplatTexture(countSplat);
                            --countSplat;
                            Repaint();
                            continue;
                        }
                    EditorGUILayout.EndHorizontal();

                    if (currentTerrain != null) //! Rtp
                    {
                        if (currentTerrain.splatSettingsFoldout)
                        {
                            TC_SplatPrototype splat = currentTerrain.splatPrototypes[countSplat];

                            bool guiChanged2 = GUI.changed;
                            GUI.changed = false;

#if !UNITY_5 && !UNITY_2017 && !UNITY_2018_1 && !UNITY_2018_2
                            EditorGUILayout.BeginHorizontal();
                            GUILayout.Space(73 + space);
                            splat.normalScale = EditorGUILayout.FloatField("Normal Scale", splat.normalScale);
                            EditorGUILayout.EndHorizontal();
#endif

                            EditorGUILayout.BeginHorizontal();
                            GUILayout.Space(73 + space);

                            float tileSize = splat.tileSize.x;
                            tileSize = EditorGUILayout.FloatField("Tile Size", tileSize);
                           
                            EditorGUILayout.EndHorizontal();

                            EditorGUILayout.BeginHorizontal();
                            GUILayout.Space(73 + space);
                            // EditorGUILayout.LabelField("Tile Offset", GUILayout.Width(125.0f));
                            splat.tileOffset = EditorGUILayout.Vector2Field("Tile Offset", splat.tileOffset);
                            EditorGUILayout.EndHorizontal();

                            if (GUI.changed)
                            {
                                splat.tileSize = new Vector2(tileSize, tileSize);
                                Apply();
                                SceneView.RepaintAll();
                                guiChanged = true;
                            }
                            GUI.changed = guiChanged2;
                        }
                    }
                    else
                    {
                        EditorGUILayout.EndHorizontal();
                    }
                }

                if (GUI.changed)
                {
                    // Debug.Log("gui changed");
                    Apply();
                    SceneView.RepaintAll();
                    guiChanged = true;
                }
                GUI.changed = guiChanged;

                //    EditorGUILayout.BeginHorizontal();
                //    GUILayout.Space(30 + space);
                //    if (TC_Settings.instance.global.tooltip)
                //    {
                //        tooltipText = "Open Splat Texture Preset from saved file";
                //    }
                //    if (GUILayout.Button(new GUIContent("Open", tooltipText), GUILayout.Width(45.0f)))
                //    {
                //        string path_splat_open1 = EditorUtility.OpenFilePanel("Open Splat Preset", Application.dataPath + "/TerrainComposer/save/presets/splat", "prefab");

                //        if (path_splat_open1.Length != 0)
                //        {
                //            load_splat_preset(path_splat_open1, currentTerrain, 0, false);
                //        }
                //    }
                //    if (TC_Settings.instance.global.tooltip)
                //    {
                //        tooltipText = "Save Splat Texture Preset";
                //    }
                //    if (GUILayout.Button(new GUIContent("Save", tooltipText), GUILayout.Width(45.0f)))
                //    {
                //        string path_splat_save = EditorUtility.SaveFilePanel("Save Splat Preset", Application.dataPath + "/TerrainComposer/save/presets/splat", "", "prefab");

                //        if (path_splat_save.Length != 0)
                //        {
                //            save_splat_preset1(path_splat_save, currentTerrain);
                //        }
                //    }
                //    EditorGUILayout.EndHorizontal();
            }

            if (terrainArea.treeTab)
            {
                TD.DrawLabelWidthUnderline("Terrain Trees", 14);
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(30 + space);

                if (currentTerrain.treeSettingsFoldout) GUI.backgroundColor = Color.green;
                if (GUILayout.Button("Settings", GUILayout.Width(70)))
                {
                    currentTerrain.treeSettingsFoldout = !currentTerrain.treeSettingsFoldout;
                }
                GUI.backgroundColor = Color.white;

                EditorGUILayout.EndHorizontal();


                // if (currentTerrain.treePrototypes.Count == 0)
                // {
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(30 + space);
                    if (TC_Settings.instance.global.tooltip) tooltipText = "Add a new Tree";
                    
                    if (GUILayout.Button(new GUIContent("+", tooltipText), GUILayout.Width(25.0f)))
                    {
                        currentTerrain.AddTreePrototype(currentTerrain.treePrototypes.Count);
                        if (eventCurrent.shift && currentTerrain.treePrototypes.Count > 1)
                        {
                            // !script.copy_terrain_tree(currentTerrain.treePrototypes[currentTerrain.treePrototypes.Count - 2], currentTerrain.treePrototypes[currentTerrain.treePrototypes.Count - 1]);
                        }
                    }
                    if (TC_Settings.instance.global.tooltip) tooltipText = "Erase the last Tree\n\n(Control Click)";
                   
                    EditorGUILayout.EndHorizontal();
                //}
                GUILayout.Space(5);

                // GUI.changed = false;

                for (int countTree = 0; countTree < currentTerrain.treePrototypes.Count; ++countTree)
                {
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(30 + space);

                    if (currentTerrain.treePrototypes[countTree].prefab == null) { GUILayout.Button(new GUIContent("Empty"), EditorStyles.miniButtonMid, GUILayout.Width(64.0f), GUILayout.Height(64.0f)); }
                    else
                    {
                        if (TC_Settings.instance.global.tooltip) tooltipText = "Click to preview\n\nClick again to close preview"; else tooltipText = "";

                        if (GUILayout.Button(new GUIContent(AssetPreview.GetAssetPreview(currentTerrain.treePrototypes[countTree].prefab), tooltipText), EditorStyles.miniButtonMid, GUILayout.Width(64.0f), GUILayout.Height(64.0f)))
                        {
                            // create_preview_window(currentTerrain.treePrototypes[count_tree].texture, "Tree Preview");
                        }
                    }

                    EditorGUILayout.LabelField("" + (countTree + 1) + ").", GUILayout.Width(28.0f));
                    currentTerrain.treePrototypes[countTree].prefab = (GameObject)EditorGUILayout.ObjectField(currentTerrain.treePrototypes[countTree].prefab, typeof(GameObject), true, GUILayout.Width(250.0f));

                    if (currentTerrain.treePrototypes.Count > 1)
                    {
                        if (countTree > 0)
                        {
                            if (TC_Settings.instance.global.tooltip) tooltipText = "Move Tree up";
                            if (GUILayout.Button(new GUIContent("▲", tooltipText), GUILayout.Width(25.0f))) { currentTerrain.SwapTree(countTree, countTree - 1); }
                        }
                        else GUILayout.Space(29.0f);
                        
                        if (countTree < currentTerrain.treePrototypes.Count - 1)
                        {
                            if (TC_Settings.instance.global.tooltip) tooltipText = "Move Tree down";
                            
                            if (GUILayout.Button(new GUIContent("▼", tooltipText), GUILayout.Width(25.0f))) { currentTerrain.SwapTree(countTree, countTree + 1); }
                        }
                        else GUILayout.Space(29.0f);
                    }
                    if (TC_Settings.instance.global.tooltip) tooltipText = "Insert a new Tree";
                    
                    if (GUILayout.Button(new GUIContent("+", tooltipText), GUILayout.Width(25.0f)))
                    {
                        UndoRegister("Add Tree");
                        currentTerrain.AddTreePrototype(countTree + 1);
                        if (eventCurrent.shift)
                        {
                            currentTerrain.CopyTree(currentTerrain.treePrototypes[countTree], currentTerrain.treePrototypes[countTree + 1]);
                        } 
                    }
                    if (TC_Settings.instance.global.tooltip) tooltipText = "Erase this Tree\n\n(Control Click)";
                    
                    if (GUILayout.Button(new GUIContent("-", tooltipText), GUILayout.Width(25.0f)))
                    {
                        UndoRegister("Erase Tree");
                        currentTerrain.EraseTreeProtoType(countTree);
                    }
                    EditorGUILayout.EndHorizontal();
                    
                    if (currentTerrain.treeSettingsFoldout)
                    {
                        EditorGUILayout.BeginHorizontal();
                        GUILayout.Space(45 + space);
                        currentTerrain.treePrototypes[countTree].bendFactor = EditorGUILayout.FloatField("Bend Factor", currentTerrain.treePrototypes[countTree].bendFactor);
                        //if (TC_Settings.instance.global.tooltip)
                        //{
                        //    tooltipText = "Set this bendfactor to all Trees in this Terrain\n(Click)\n\nSet this bendfactor to all Trees in the Terrains\n(Shift Click)";
                        //}
                        //if (GUILayout.Button(new GUIContent(">Set All", tooltipText), EditorStyles.miniButtonMid, GUILayout.Width(65.0f)))
                        //{
                        //    // UndoRegister("Set All Trees Settings");
                        //    if (!eventCurrent.shift)
                        //    {
                                
                        //        // !script.set_all_trees_settings_terrain(current_terrain, count_tree);
                        //    }
                        EditorGUILayout.EndHorizontal();
                    }

                    //if (GUI.changed)
                    //{
                    //    Debug.Log("gui changed");
                    //    Apply();
                    //    SceneView.RepaintAll();
                    //}
                }

                //EditorGUILayout.BeginHorizontal();
                //GUILayout.Space(30 + space);
                //if (TC_Settings.instance.global.tooltip)
                //{
                //    tooltipText = "Open Tree Preset from saved file";
                //}
                //if (GUILayout.Button(new GUIContent("Open", tooltipText), GUILayout.Width(45.0f)))
                //{
                //    string path_tree_open1 = EditorUtility.OpenFilePanel("Open Tree Preset", Application.dataPath + "/TerrainComposer/save/presets/tree", "prefab");

                //    if (path_tree_open1.Length != 0)
                //    {
                //        load_tree_preset(path_tree_open1, currentTerrain, 0, false);
                //        this.Repaint();
                //    }
                //}
                //if (TC_Settings.instance.global.tooltip)
                //{
                //    tooltipText = "Save Tree Texture Preset";
                //}
                //if (GUILayout.Button(new GUIContent("Save", tooltipText), GUILayout.Width(45.0f)))
                //{
                //    string path_tree_save = EditorUtility.SaveFilePanel("Save Tree Preset", Application.dataPath + "/TerrainComposer/save/presets/tree", "", "prefab");

                //    if (path_tree_save.Length != 0)
                //    {
                //        save_tree_preset1(path_tree_save, currentTerrain);
                //    }
                //}
                //EditorGUILayout.EndHorizontal();
            }

            if (terrainArea.grassTab)
            {
                TD.DrawLabelWidthUnderline("Terrain Grass", 14);
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(30 + space);
                
                if (currentTerrain.detailSettingsFoldout) GUI.backgroundColor = Color.green;
                if (GUILayout.Button("Settings", GUILayout.Width(70)))
                {
                    currentTerrain.detailSettingsFoldout = !currentTerrain.detailSettingsFoldout;
                }
                GUI.backgroundColor = Color.white;
                    
                EditorGUILayout.EndHorizontal();

                //if (currentTerrain.detailPrototypes.Count == 0)
                //{
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(30 + space);
               
                    if (TC_Settings.instance.global.tooltip) tooltipText = "Add a new Grass/Detail";

                    if (GUILayout.Button(new GUIContent("+", tooltipText), GUILayout.Width(25.0f)))
                    {
                        currentTerrain.AddDetailPrototype(currentTerrain.detailPrototypes.Count);
                    }
                    EditorGUILayout.EndHorizontal();
                //}
                GUILayout.Space(5);
                
                for (int countGrass = 0; countGrass < currentTerrain.detailPrototypes.Count; ++countGrass)
                {
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(30 + space);
                    if (currentTerrain.detailPrototypes[countGrass].usePrototypeMesh)
                    {
                        EditorGUILayout.LabelField("" + (countGrass + 1) + ")", GUILayout.Width(25.0f));
                        currentTerrain.detailPrototypes[countGrass].prototype = EditorGUILayout.ObjectField(currentTerrain.detailPrototypes[countGrass].prototype, typeof(GameObject), true, GUILayout.Width(143.0f)) as GameObject;
                    }
                    else
                    {
                        EditorGUILayout.LabelField("" + (countGrass + 1) + ")", GUILayout.Width(24.0f));
                        currentTerrain.detailPrototypes[countGrass].prototypeTexture = (Texture2D)EditorGUILayout.ObjectField(currentTerrain.detailPrototypes[countGrass].prototypeTexture, typeof(Texture2D), true, GUILayout.Width(55.0f), GUILayout.Height(55.0f));
                        if (currentTerrain.detailPrototypes[countGrass].prototypeTexture != null)
                        {
                            EditorGUILayout.LabelField(currentTerrain.detailPrototypes[countGrass].prototypeTexture.name, GUILayout.Width(85));
                        }
                        else 
                        {
                            GUILayout.Space(60.0f);
                        }
                    }
                    EditorGUILayout.LabelField("Mesh", GUILayout.Width(35.0f));
                    currentTerrain.detailPrototypes[countGrass].usePrototypeMesh = EditorGUILayout.Toggle(currentTerrain.detailPrototypes[countGrass].usePrototypeMesh, GUILayout.Width(20.0f));
                    currentTerrain.detailPrototypes[countGrass].renderMode = (DetailRenderMode)EditorGUILayout.EnumPopup(currentTerrain.detailPrototypes[countGrass].renderMode, GUILayout.Width(100));

                    //if (currentTerrain.detailPrototypes[countGrass].renderMode == DetailRenderMode.VertexLit)
                    //    currentTerrain.detailPrototypes[countGrass].usePrototypeMesh = true;
                    //else
                    //    currentTerrain.detailPrototypes[countGrass].usePrototypeMesh = false;

                    GUILayout.Space(5);

                    if (countGrass > 0)
                    {
                        if (TC_Settings.instance.global.tooltip)
                        {
                            tooltipText = "Move Grass/Detail up";
                        }
                        if (GUILayout.Button(new GUIContent("▲", tooltipText), GUILayout.Width(25.0f)) && countGrass > 0) { TC.Swap(currentTerrain.detailPrototypes, countGrass, currentTerrain.detailPrototypes, countGrass - 1); }
                    }
                    else
                    {
                        GUILayout.Space(29.0f);
                    }
                    if (countGrass < currentTerrain.detailPrototypes.Count - 1)
                    {
                        if (TC_Settings.instance.global.tooltip) tooltipText = "Move Grass/Detail down";
                        if (GUILayout.Button(new GUIContent("▼", tooltipText), GUILayout.Width(25.0f))) { TC.Swap(currentTerrain.detailPrototypes, countGrass, currentTerrain.detailPrototypes, countGrass + 1); }
                    }
                    else
                    {
                        GUILayout.Space(29.0f);
                    }
                    if (TC_Settings.instance.global.tooltip)
                    {
                        tooltipText = "Insert a new Grass/Detail";
                    }
                    if (GUILayout.Button(new GUIContent("+", tooltipText), GUILayout.Width(25.0f)))
                    {
                        currentTerrain.AddDetailPrototype(countGrass + 1);
                        if (eventCurrent.shift)
                        {
                            // !script.copy_terrain_detail(currentTerrain.detailPrototypes[count_detail], currentTerrain.detailPrototypes[count_detail + 1]);
                        }
                    }
                    if (TC_Settings.instance.global.tooltip)
                    {
                        tooltipText = "Erase this Grass/Detail";
                    }
                    if (GUILayout.Button(new GUIContent("-", tooltipText), GUILayout.Width(25.0f)))
                    {
                        // UndoRegister("Erase Grass");
                        currentTerrain.EraseDetailPrototype(countGrass);
                        Repaint();
                        --countGrass;
                    }
                    EditorGUILayout.EndHorizontal();

                    if (currentTerrain.detailSettingsFoldout)
                    {
                        if (!currentTerrain.detailPrototypes[countGrass].usePrototypeMesh)
                        {
                            EditorGUILayout.BeginHorizontal();
                            GUILayout.Space(70 + space);
                            currentTerrain.detailPrototypes[countGrass].minWidth = EditorGUILayout.FloatField("Min. Width", currentTerrain.detailPrototypes[countGrass].minWidth);
                            EditorGUILayout.EndHorizontal();
                        }

                        EditorGUILayout.BeginHorizontal();
                        GUILayout.Space(70 + space);
                        if (!currentTerrain.detailPrototypes[countGrass].usePrototypeMesh)
                        {
                            currentTerrain.detailPrototypes[countGrass].maxWidth = EditorGUILayout.FloatField("Max. Width", currentTerrain.detailPrototypes[countGrass].maxWidth);
                        }
                        else
                        {
                            currentTerrain.detailPrototypes[countGrass].maxWidth = EditorGUILayout.FloatField("Ramdom Width", currentTerrain.detailPrototypes[countGrass].maxWidth);
                        }
                        EditorGUILayout.EndHorizontal();

                        if (!currentTerrain.detailPrototypes[countGrass].usePrototypeMesh)
                        {
                            EditorGUILayout.BeginHorizontal();
                            GUILayout.Space(70 + space);
                            currentTerrain.detailPrototypes[countGrass].minHeight = EditorGUILayout.FloatField("Min. Height", currentTerrain.detailPrototypes[countGrass].minHeight);
                            EditorGUILayout.EndHorizontal();
                        }

                        EditorGUILayout.BeginHorizontal();
                        GUILayout.Space(70 + space);
                        if (!currentTerrain.detailPrototypes[countGrass].usePrototypeMesh)
                        {
                            currentTerrain.detailPrototypes[countGrass].maxHeight = EditorGUILayout.FloatField("Max. Height", currentTerrain.detailPrototypes[countGrass].maxHeight);
                        }
                        else
                        {
                            currentTerrain.detailPrototypes[countGrass].maxHeight = EditorGUILayout.FloatField("Random Height", currentTerrain.detailPrototypes[countGrass].maxHeight);
                        }
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.BeginHorizontal();
                        GUILayout.Space(70 + space);
                        currentTerrain.detailPrototypes[countGrass].noiseSpread = EditorGUILayout.FloatField("Noise Spread", currentTerrain.detailPrototypes[countGrass].noiseSpread);
                        EditorGUILayout.EndHorizontal();

                        if (!currentTerrain.detailPrototypes[countGrass].usePrototypeMesh)
                        {
                            EditorGUILayout.BeginHorizontal();
                            GUILayout.Space(70 + space);
                            currentTerrain.detailPrototypes[countGrass].bendFactor = EditorGUILayout.FloatField("Bend Factor", currentTerrain.detailPrototypes[countGrass].bendFactor);
                            EditorGUILayout.EndHorizontal();
                        }

                        EditorGUILayout.BeginHorizontal();
                        GUILayout.Space(70 + space);
                        currentTerrain.detailPrototypes[countGrass].healthyColor = EditorGUILayout.ColorField("Healthy Color", currentTerrain.detailPrototypes[countGrass].healthyColor);
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.BeginHorizontal();
                        GUILayout.Space(70 + space);
                        currentTerrain.detailPrototypes[countGrass].dryColor = EditorGUILayout.ColorField("Dry Color", currentTerrain.detailPrototypes[countGrass].dryColor);
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.BeginHorizontal();
                        GUILayout.Space(70 + space);
                        currentTerrain.detailPrototypes[countGrass].renderMode = (DetailRenderMode)EditorGUILayout.EnumPopup("Render Mode", (Enum)currentTerrain.detailPrototypes[countGrass].renderMode);
                        EditorGUILayout.EndHorizontal();
                    }
                }

                if (currentTerrain.detailPrototypes.Count > 0)
                {
                    GUILayout.Space(5);

                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(30 + space);
                    EditorGUILayout.LabelField("Scale Multi", GUILayout.Width(147.0f));

                    currentTerrain.grassScaleMulti = EditorGUILayout.FloatField(currentTerrain.grassScaleMulti);
                    if (currentTerrain.grassScaleMulti < 0.01f) currentTerrain.grassScaleMulti = 0.01f;

                    EditorGUILayout.EndHorizontal();
                }

                //EditorGUILayout.BeginHorizontal();
                //GUILayout.Space(30 + space);
                //if (TC_Settings.instance.global.tooltip)
                //{
                //    tooltipText = "Open Grass/Detail Preset from saved file";
                //}
                //if (GUILayout.Button(new GUIContent("Open", tooltipText), GUILayout.Width(45.0f)))
                //{
                //    string path_grass_open1 = EditorUtility.OpenFilePanel("Open Grass/Detail Preset", Application.dataPath + "/TerrainComposer/save/presets/grass", "prefab");

                //    if (path_grass_open1.Length != 0)
                //    {
                //        load_grass_preset(path_grass_open1, currentTerrain, 0, false);
                //    }
                //}
                //if (TC_Settings.instance.global.tooltip)
                //{
                //    tooltipText = "Save grass Texture Preset";
                //}
                //if (GUILayout.Button(new GUIContent("Save", tooltipText), GUILayout.Width(45.0f)))
                //{
                //    string path_grass_save = EditorUtility.SaveFilePanel("Save Grass/Detail Preset", Application.dataPath + "/TerrainComposer/save/presets/grass", "", "prefab");

                //    if (path_grass_save.Length != 0)
                //    {
                //        save_grass_preset1(path_grass_save, currentTerrain);
                //    }
                //}
                //EditorGUILayout.EndHorizontal();
            }

            if (terrainArea.resetTab && currentTerrain.terrain != null)
            {
                TD.DrawLabelWidthUnderline("Terrain Reset", 14);
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(30 + space);
                // EditorGUILayout.LabelField("Reset ->", EditorStyles.boldLabel, GUILayout.Width(60.0f));

                if (TC_Settings.instance.global.tooltip)
                {
                    tooltipText = "Reset Heightmap Data of " + currentTerrain.terrain.name + " in Scene\n(Control Click)\n\nReset Heightmap Data of all Terrains\n(Control Shift Click)";
                }
                DrawReset(terrainArea, currentTerrain, space);
                if (TC_Settings.instance.global.tooltip)
                {
                    tooltipText = "Reset Splatmap Data of " + currentTerrain.terrain.name + " in Scene\n(Control Click)\n\nReset Splatmap Data of all Terrains\n(Control Shift Click)";
                }
                if (TC_Settings.instance.global.tooltip)
                {
                    tooltipText = "Reset placed Trees in " + currentTerrain.terrain.name + " in Scene\n(Control Click)\n\nReset placed Trees in all Terrains\n(Control Shift Click)";
                }
                if (TC_Settings.instance.global.tooltip)
                {
                    tooltipText = "Reset Detail/Grass Data of " + currentTerrain.terrain.name + " in Scene\n(Control Click)\n\nReset Detail/Grass Data of all Terrains\n(Control Shift Click)";
                }
                if (TC_Settings.instance.global.tooltip)
                {
                    tooltipText = "Erase placed Objects in Scene\n(Control Click)";
                }

                EditorGUILayout.EndHorizontal();
            }

            GUI.color = Color.white;

            GUILayout.Space(20);
            if (terrainArea.sizeTab || terrainArea.resolutionsTab || terrainArea.splatTab || terrainArea.treeTab || terrainArea.grassTab || terrainArea.resetTab)
            {
                EditorGUILayout.BeginHorizontal();
                if (terrainArea.splatTab || terrainArea.resetTab) EditorGUILayout.LabelField("Apply to", GUILayout.Width(100));
                else
                {
                    if (GUILayout.Button("Apply", GUILayout.Width(100))) Apply();
                    EditorGUILayout.LabelField("to", GUILayout.Width(20f));
                }
                terrainArea.applyChanges = (ApplyChanges)EditorGUILayout.EnumPopup(terrainArea.applyChanges);
                EditorGUILayout.EndHorizontal();
                GUILayout.Space(5);
            }
            
            EditorGUILayout.EndVertical();
        }

        public void Apply()
        {
            bool generate = false;
            // Debug.Log("Apply");

            if (terrainArea.sizeTab)
            {
                terrainArea.ApplySize();
                generate = true;
            }
            if (terrainArea.resolutionsTab)
            {
                terrainArea.ApplyResolution();
                generate = true;
            }
            if (terrainArea.splatTab)
            {
                if (terrainArea.applyChanges == ApplyChanges.Terrain) terrainArea.terrains[terrainArea.terrainSelect].ApplySplatTextures(terrainArea, null);
                else if (terrainArea.applyChanges == ApplyChanges.TerrainArea || terrainArea.applyChanges == ApplyChanges.AllTerrainAreas) terrainArea.ApplySplatTextures(currentTerrain);
                // else if (terrainArea.applyChanges == ApplyChanges.AllTerrainAreas) GlobalManager.singleton.ApplySplatTexturesTerrainAreas(currentTerrain);
                TC.RefreshOutputReferences(TC.splatOutput);
                generate = true;
            }
            if (terrainArea.treeTab)
            {
                if (terrainArea.applyChanges == ApplyChanges.Terrain) terrainArea.terrains[terrainArea.terrainSelect].ApplyTrees();
                else if (terrainArea.applyChanges == ApplyChanges.TerrainArea) terrainArea.ApplyTrees();
                TC.RefreshOutputReferences(TC.treeOutput);
                generate = true;
            }
            if (terrainArea.grassTab)
            {
                if (terrainArea.applyChanges == ApplyChanges.Terrain) terrainArea.terrains[terrainArea.terrainSelect].ApplyGrass();
                else if (terrainArea.applyChanges == ApplyChanges.TerrainArea) terrainArea.ApplyGrass();
                TC.RefreshOutputReferences(TC.grassOutput);
                generate = true;
            }

            if (generate)
            {
                if (TC_Generate.instance != null)
                {
                    if (TC_Generate.instance.autoGenerate) TC_Generate.instance.Generate(true);
                }
            }
        }


        public void DrawReset(TC_TerrainArea terrainArea, TCUnityTerrain preterrain, float space)
        {
            Rect rect = new Rect();
            rect = GUILayoutUtility.GetLastRect();
            rect.x += 64.0f;
            rect.y += 2.0f;

            if (GUILayout.Button("Heightmap", GUILayout.Width(75.0f)))
            {
                if (terrainArea.applyChanges == ApplyChanges.Terrain) terrainArea.terrains[terrainArea.terrainSelect].ResetHeightmap();
                else if (terrainArea.applyChanges == ApplyChanges.TerrainArea) terrainArea.ResetHeightmap();
            }
            rect = GUILayoutUtility.GetLastRect();
            rect.x += 79.0f;
            if (GUILayout.Button("Splatmap", GUILayout.Width(75.0f)))
            {
                if (terrainArea.applyChanges == ApplyChanges.Terrain) terrainArea.terrains[terrainArea.terrainSelect].ResetSplatmap();
                else if (terrainArea.applyChanges == ApplyChanges.TerrainArea) terrainArea.ResetSplatmap();
            }
            rect = GUILayoutUtility.GetLastRect();
            rect.x += 79.0f;
            if (GUILayout.Button("Trees", GUILayout.Width(75.0f)))
            {
                if (terrainArea.applyChanges == ApplyChanges.Terrain) terrainArea.terrains[terrainArea.terrainSelect].ResetTrees();
                else if (terrainArea.applyChanges == ApplyChanges.TerrainArea) terrainArea.ResetTrees();
            }
            rect = GUILayoutUtility.GetLastRect();
            rect.x += 79.0f;
            if (GUILayout.Button("Grass", GUILayout.Width(75.0f)))
            {
                if (terrainArea.applyChanges == ApplyChanges.Terrain) terrainArea.terrains[terrainArea.terrainSelect].ResetGrass();
                else if (terrainArea.applyChanges == ApplyChanges.TerrainArea) terrainArea.ResetGrass();
            }
            rect = GUILayoutUtility.GetLastRect();
            rect.x += 79.0f;
            if (GUILayout.Button("Objects", GUILayout.Width(75.0f)))
            {
                if (terrainArea.applyChanges == ApplyChanges.Terrain) terrainArea.terrains[terrainArea.terrainSelect].ResetObjects();
                else if (terrainArea.applyChanges == ApplyChanges.TerrainArea) terrainArea.ResetObjects();
            }
        }

        public TCUnityTerrain getSetPreterrain;
        bool openFolderPanel;

        public void DrawCreateTerrain(float space, int draw_from)
        {
            GUI.color = Color.green;
            EditorGUILayout.BeginVertical("Box");
            GUI.color = Color.white;
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(space);
            if (!terrainArea.terrainDataPath.Contains(Application.dataPath)) terrainArea.terrainDataPath = Application.dataPath;
            EditorGUILayout.LabelField("Path", GUILayout.Width(160.0f));
            if (draw_from == 0)
            {
                EditorGUILayout.LabelField("" + terrainArea.terrainDataPath);
            }
            else
            {
                //! EditorGUILayout.LabelField("" + script.terrain_slice_path);
            }

            Rect rect = GUILayoutUtility.GetLastRect();

            if (GUI.Button(new Rect(EditorGUIUtility.currentViewWidth - 98, rect.y, 90, 19), new GUIContent("Change", button_folder)))
            {
                if (!eventCurrent.shift)
                {
                    if (draw_from == 0)
                    {
                        openFolderPanel = true;
                    }
                    else
                    {
                        //! terrain_path = EditorUtility.OpenFolderPanel("Export File Path", script.terrain_slice_path, "");
                        //! if (terrain_path != "") { script.terrain_slice_path = terrain_path; }
                    }
                }
                else
                {
                    if (draw_from == 0)
                    {
                        terrainArea.terrainDataPath = Application.dataPath;
                    }
                    else
                    {
                        //! script.terrain_slice_path = Application.dataPath;
                    }
                }
            }
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(3);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(space);
            EditorGUILayout.LabelField("Terrain Name", GUILayout.Width(160.0f));
            terrainArea.terrainName = EditorGUILayout.TextField(terrainArea.terrainName);
            EditorGUILayout.EndHorizontal();

            if (terrainArea.terrains == null)
            {
                terrainArea.terrains = new List<TCUnityTerrain>();
                terrainArea.terrains.Add(new TCUnityTerrain());
            }

            if (terrainArea.terrains.Count > 1)
            {
                //EditorGUILayout.BeginHorizontal();
                //    GUILayout.Space(space);
                //    EditorGUILayout.LabelField("Copy Settings Terrain", GUILayout.Width(160.0f));
                //    // EditorGUILayout.LabelField("Terrain",GUILayout.Width(70));
                //    gui_changed_old = GUI.changed;
                //    gui_changed_window = GUI.changed; GUI.changed = false;
                //    current_terrain.copy_terrain = EditorGUILayout.IntField(current_terrain.copy_terrain, GUILayout.Width(50.0f));
                //    if (GUI.changed)
                //    {
                //        if (current_terrain.copy_terrain == i) { --current_terrain.copy_terrain; }
                //        if (current_terrain.copy_terrain < 0) { current_terrain.copy_terrain = 0; }
                //        if (current_terrain.copy_terrain > terrainArea.terrains.Count - 1) { current_terrain.copy_terrain = terrainArea.terrains.Count - 1; }
                //    }
                //    GUI.changed = gui_changed_old;
                //    current_terrain.copy_terrain_settings = EditorGUILayout.Toggle(current_terrain.copy_terrain_settings, GUILayout.Width(25.0f));
                //EditorGUILayout.EndHorizontal();


                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(space);
                EditorGUILayout.LabelField("Copy Terrain Material", GUILayout.Width(160.0f));
                // script.settings.copy_terrain_material = EditorGUILayout.Toggle(script.settings.copy_terrain_material, GUILayout.Width(25.0f));
                EditorGUILayout.EndHorizontal();

            }

            guiChanged = GUI.changed;
            GUI.changed = false;

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(space);
            EditorGUILayout.LabelField("Tiles X", GUILayout.Width(160.0f));
            int tileX = terrainArea.selectTiles.x;
            tileX = EditorGUILayout.IntSlider(tileX, 1, 32);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(space);
            EditorGUILayout.LabelField("Tiles Y", GUILayout.Width(131.0f));
            terrainArea.tileLink = EditorGUILayout.Toggle(terrainArea.tileLink, GUILayout.Width(25.0f));
            int tileY;
            if (terrainArea.tileLink) tileY = tileX; else tileY = terrainArea.selectTiles.y;
            tileY = EditorGUILayout.IntSlider(tileY, 1, 32);
            terrainArea.selectTiles = new Int2(tileX, tileY);
            EditorGUILayout.EndHorizontal();

#if !UNITY_5 && !UNITY_2017 && !UNITY_2018_1 && !UNITY_2018_2
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(space);
            EditorGUILayout.LabelField(new GUIContent("Share Splat Textures", "Use the same Splat Textures (U2018.3 new Terrain Layers) for all terrains"), GUILayout.Width(160.0f));
            terrainArea.useSameTerrainLayersForAllTerrains = EditorGUILayout.Toggle(terrainArea.useSameTerrainLayersForAllTerrains, GUILayout.Width(25));
            EditorGUILayout.EndHorizontal();
#endif

            if (GUI.changed)
            {
                if (terrainArea.tileLink) terrainArea.tiles = new Int2(terrainArea.tiles.x, terrainArea.tiles.x);
                // script.calc_terrain_needed_tiles();
                guiChanged = true;
            }
            GUI.changed = guiChanged;

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(space);
            if (GUILayout.Button("Create", GUILayout.Width(150.0f)))
            {
                terrainArea.CreateTerrains();
                TC.AutoGenerate();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();

            if (openFolderPanel && eventCurrent.type == EventType.Repaint)
            {
                openFolderPanel = false;
                string terrain_path = EditorUtility.OpenFolderPanel("Export File Path", terrainArea.terrainDataPath, "");
                if (terrain_path != "") { terrainArea.terrainDataPath = terrain_path; }
            }
        }

        public void generate_auto() { } // !

        public void CloseTerrainTabs()
        {
            terrainArea.sizeTab = terrainArea.resolutionsTab = terrainArea.settingsTab = terrainArea.splatTab = terrainArea.treeTab = terrainArea.grassTab = terrainArea.resetTab = false;
        }

        //public void save_splat_preset1(string path1, TCUnityTerrain preterrain1)
        //{
        //    FileInfo file_info = new FileInfo(path1);
        //    path1 = path1.Replace(Application.dataPath + "/", "Assets/");

        //    GameObject object_cs2 = new GameObject();
        //    // save_splat_preset script3 = object_cs2.AddComponent<save_splat_preset>();

        //    // !script3.splatPrototypes = preterrain1.splatPrototypes;

        //    AssetDatabase.DeleteAsset(path1);
        //    UnityEngine.Object prefab = PrefabUtility.CreateEmptyPrefab(path1);

        //    PrefabUtility.ReplacePrefab(object_cs2, prefab, ReplacePrefabOptions.ReplaceNameBased);

        //    // VersionControl.Provider.Checkout(prefab,CheckoutMode.Both);

        //    DestroyImmediate(object_cs2);

        //    AssetDatabase.SaveAssets();
        //    AssetDatabase.Refresh();
        //}

        //public void save_tree_preset1(string path1, TCUnityTerrain preterrain1)
        //{
        //    FileInfo file_info = new FileInfo(path1);
        //    path1 = path1.Replace(Application.dataPath + "/", "Assets/");

        //    GameObject object_cs3 = new GameObject();
        //    // save_tree_preset script3 = object_cs3.AddComponent<save_tree_preset>();

        //    // !script3.treePrototypes = preterrain1.treePrototypes;

        //    AssetDatabase.DeleteAsset(path1);
        //    UnityEngine.Object prefab = PrefabUtility.CreateEmptyPrefab(path1);

        //    PrefabUtility.ReplacePrefab(object_cs3, prefab, ReplacePrefabOptions.ReplaceNameBased);
        //    // VersionControl.Provider.Checkout(prefab,CheckoutMode.Both);

        //    DestroyImmediate(object_cs3);

        //    AssetDatabase.SaveAssets();
        //    AssetDatabase.Refresh();
        //}

        //public void save_grass_preset1(string path1, TCUnityTerrain preterrain1)
        //{
        //    FileInfo file_info = new FileInfo(path1);
        //    path1 = path1.Replace(Application.dataPath + "/", "Assets/");

        //    GameObject object_cs4 = new GameObject();
        //    // save_grass_preset script3 = object_cs4.AddComponent<save_grass_preset>();

        //    // !script3.detailPrototypes = preterrain1.detailPrototypes;

        //    AssetDatabase.DeleteAsset(path1);
        //    UnityEngine.Object prefab = PrefabUtility.CreateEmptyPrefab(path1);

        //    PrefabUtility.ReplacePrefab(object_cs4, prefab, ReplacePrefabOptions.ReplaceNameBased);
        //    // VersionControl.Provider.Checkout(prefab,CheckoutMode.Both);

        //    DestroyImmediate(object_cs4);

        //    AssetDatabase.SaveAssets();
        //    AssetDatabase.Refresh();
        //}

        //public void load_splat_preset(string path1, TCUnityTerrain preterrain1, int splat_index, bool add)
        //{
        //    path1 = path1.Replace(Application.dataPath + "/", "Assets/");

        //    GameObject object_cs5 = (GameObject)Instantiate(AssetDatabase.LoadAssetAtPath(path1, typeof(GameObject)));
        //    // save_splat_preset script3 = object_cs5.GetComponent<save_splat_preset>();

        //    //if (script3 != null)
        //    //{
        //    //    if (!add)
        //    //    {
        //    //        // !preterrain1.splatPrototypes = script3.splatPrototypes;
        //    //    }
        //    //    else
        //    //    {
        //    //        for (int count_splat = 0; count_splat < script3.splatPrototypes.Count; ++count_splat)
        //    //        {
        //    //            // !preterrain1.splatPrototypes.Insert(splat_index, script3.splatPrototypes[count_splat]);
        //    //        }
        //    //    }
        //    //    preterrain1.clear_null_splatprototype();
        //    //}
        //    //else { this.ShowNotification(new GUIContent("This file is not a Splat preset")); }

        //    DestroyImmediate(object_cs5);
        //}

        //public void load_tree_preset(string path1, TCUnityTerrain preterrain1, int tree_index, bool add)
        //{
        //    path1 = path1.Replace(Application.dataPath + "/", "Assets/");

        //    GameObject object_cs6 = (GameObject)Instantiate(AssetDatabase.LoadAssetAtPath(path1, typeof(GameObject)));
        //    // save_tree_preset script3 = object_cs6.GetComponent<save_tree_preset>();

        //    //if (script3 != null)
        //    //{
        //    //    if (!add)
        //    //    {
        //    //        // !preterrain1.treePrototypes = script3.treePrototypes;
        //    //    }
        //    //    else
        //    //    {
        //    //        for (int count_tree = 0; count_tree < script3.treePrototypes.Count; ++count_tree)
        //    //        {
        //    //            // !preterrain1.treePrototypes.Insert(tree_index, script3.treePrototypes[count_tree]);
        //    //        }
        //    //    }
        //    //    preterrain1.clear_null_treeprototype();
        //    //}
        //    //else { this.ShowNotification(new GUIContent("This file is not a Tree preset")); }


        //    DestroyImmediate(object_cs6);
        //}

        //public void load_grass_preset(string path1, TCUnityTerrain preterrain1, int grass_index, bool add)
        //{
        //    path1 = path1.Replace(Application.dataPath + "/", "Assets/");

        //    GameObject object_cs7 = (GameObject)Instantiate(AssetDatabase.LoadAssetAtPath(path1, typeof(GameObject)));
        //    // save_grass_preset script3 = object_cs7.GetComponent<save_grass_preset>();

        //    //if (script3 != null)
        //    //{
        //    //    if (!add)
        //    //    {
        //    //        // !preterrain1.detailPrototypes = script3.detailPrototypes;
        //    //    }
        //    //    else
        //    //    {
        //    //        for (int count_grass = 0; count_grass < script3.detailPrototypes.Count; ++count_grass)
        //    //        {
        //    //            // !preterrain1.detailPrototypes.Insert(grass_index, script3.detailPrototypes[count_grass]);
        //    //        }
        //    //    }
        //    //    preterrain1.clear_null_detailprototype();
        //    //}
        //    //else { this.ShowNotification(new GUIContent("This file is not a Grass/Detail preset")); }

        //    DestroyImmediate(object_cs7);
        //}

        void UndoRegister(string text)
        {
            return;
            // Undo.RecordObject(currentTerrain, text);
        }

        public void LoadButtonTextures()
        {
            // button_heightmap = (Texture)AssetDatabase.LoadAssetAtPath(TC.installPath + "/GUI/Buttons/button_heightmap.png", typeof(Texture));
            // button_colormap = (Texture)AssetDatabase.LoadAssetAtPath(TC.installPath + "/GUI/Buttons/button_colormap.png", typeof(Texture));
            button_splatmap = (Texture)AssetDatabase.LoadAssetAtPath(TC.installPath + "/GUI/Buttons/button_splatmap.png", typeof(Texture));
            button_tree = (Texture)AssetDatabase.LoadAssetAtPath(TC.installPath + "/GUI/Buttons/button_tree.png", typeof(Texture));
            button_grass = (Texture)AssetDatabase.LoadAssetAtPath(TC.installPath + "/GUI/Buttons/button_grass.png", typeof(Texture));
            // button_object = (Texture)AssetDatabase.LoadAssetAtPath(TC.installPath + "/GUI/Buttons/button_objects.png", typeof(Texture));
            // button_export = (Texture)AssetDatabase.LoadAssetAtPath(TC.installPath + "/GUI/Buttons/button_savedisk.png", typeof(Texture));
            // button_measure_tool = (Texture)AssetDatabase.LoadAssetAtPath(TC.installPath + "/GUI/Buttons/button_measure_tool.png", typeof(Texture));
            // button_meshcapture_tool = (Texture)AssetDatabase.LoadAssetAtPath(TC.installPath + "/GUI/Buttons/button_meshcapture_tool.png", typeof(Texture));
            // button_tools = (Texture)AssetDatabase.LoadAssetAtPath(TC.installPath + "/GUI/Buttons/button_tools.png", typeof(Texture));
            // button_terrain = (Texture)AssetDatabase.LoadAssetAtPath(TC.installPath + "/GUI/Buttons/button_terrain.png", typeof(Texture));
            // button_globe = (Texture)AssetDatabase.LoadAssetAtPath(TC.installPath + "/GUI/Buttons/button_globe.png", typeof(Texture));
            // button_help = (Texture)AssetDatabase.LoadAssetAtPath(TC.installPath + "/GUI/Buttons/button_help.png", typeof(Texture));

            button_reset = (Texture)AssetDatabase.LoadAssetAtPath(TC.installPath + "/GUI/Buttons/button_reset.png", typeof(Texture));
            button_settings = (Texture)AssetDatabase.LoadAssetAtPath(TC.installPath + "/GUI/Buttons/button_settings.png", typeof(Texture));
            button_resolution = (Texture)AssetDatabase.LoadAssetAtPath(TC.installPath + "/GUI/Buttons/button_resolution.png", typeof(Texture));
            button_size = (Texture)AssetDatabase.LoadAssetAtPath(TC.installPath + "/GUI/Buttons/button_size.png", typeof(Texture));
            // button_localArea = (Texture)AssetDatabase.LoadAssetAtPath(TC.installPath + "/GUI/Buttons/button_localArea.png", typeof(Texture));
            // button_script = (Texture)AssetDatabase.LoadAssetAtPath(TC.installPath + "/GUI/Buttons/button_script.png", typeof(Texture));
            // button_stitch = (Texture)AssetDatabase.LoadAssetAtPath(TC.installPath + "/GUI/Buttons/button_stitch.png", typeof(Texture));
            // button_smooth = (Texture)AssetDatabase.LoadAssetAtPath(TC.installPath + "/GUI/Buttons/button_smooth.png", typeof(Texture));
            // button_search = (Texture)AssetDatabase.LoadAssetAtPath(TC.installPath + "/GUI/Buttons/button_search.png", typeof(Texture));
            // button_open = (Texture)AssetDatabase.LoadAssetAtPath(TC.installPath + "/GUI/Buttons/button_open.png", typeof(Texture));
            // button_slice = (Texture)AssetDatabase.LoadAssetAtPath(TC.installPath + "/GUI/Buttons/button_slice.png", typeof(Texture));
            // button_convert = (Texture)AssetDatabase.LoadAssetAtPath(TC.installPath + "/GUI/Buttons/button_convert.png", typeof(Texture));
            // button_sun = (Texture)AssetDatabase.LoadAssetAtPath(TC.installPath + "/GUI/Buttons/button_sun.png", typeof(Texture));
            // button_global = (Texture)AssetDatabase.LoadAssetAtPath(TC.installPath + "/GUI/Buttons/button_global.png", typeof(Texture));
            button_folder = (Texture)AssetDatabase.LoadAssetAtPath(TC.installPath + "/GUI/Buttons/button_folder.png", typeof(Texture));
            // button_rtp = (Texture)AssetDatabase.LoadAssetAtPath(TC.installPath + "/GUI/Buttons/button_rtp.png", typeof(Texture));


            // palette_texture = (Texture)AssetDatabase.LoadAssetAtPath(TC.installPath + "/GUI/Buttons/Palette.png", typeof(Texture));
        }
    }
}