using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;


namespace TerrainComposer2
{
    [CustomEditor(typeof(TC_ItemBehaviour), true)]
    [CanEditMultipleObjects]
    public class TC_ItemBehaviourEditor : Editor {

        SerializedProperty posOffset, positionMode;

        SerializedProperty isLocked, lockTransform, lockPosX, lockPosY, lockPosZ, lockRotY, lockScaleX, lockScaleY, lockScaleZ, lockPosChildren;
        SerializedProperty overlay, posY, size;
        SerializedProperty previewEdit;
        SerializedProperty method;
        SerializedProperty doNormalize;

        SerializedProperty treeResolutionPM, objectResolutionPM, objectAreaSize, objectTransform, colormapResolution, meshResolution;

        SerializedProperty inputKind, inputTerrain, inputNoise, inputShape, inputFile, inputCurrent;
        SerializedProperty splatSelectIndex;

        SerializedProperty seed2;
        SerializedProperty noise, lacunarity, octaves, persistence, seed, amplitude, warp0, warp, damp0, damp, dampScale, cellType, distanceFunction;
        SerializedProperty noiseMode, cellNoiseMode;
        
        SerializedProperty wrapMode, radius;
        SerializedProperty stampTex, pathTexStamp, rawImage, collisionMask, collisionMode, heightDetectRange, range; //collisionDirection
        SerializedProperty image, texImage;
        SerializedProperty imageSettings, colChannels, colSelectMode;
        
        SerializedProperty shapes, topSize, bottomSize, shapeSize;
        SerializedProperty iterations, mipmapLevel, convexityMode, convexityStrength, blurMode, detectRange;
        SerializedProperty distanceRules;
        SerializedProperty notes;

        // SelectItemGroup 
        SerializedProperty mix, scaleMulti, scaleMinMaxMulti, linkScaleToMask, linkScaleToMaskAmount;

        // Item
        SerializedProperty selectIndex, texColor, brightness, saturation; //, splatCustom, splatCustomValues;

        // Tree
        SerializedProperty tree, scaleRange, nonUniformScale, scaleCurve, randomPosition, heightOffset;

        // SpawnObject
        SerializedProperty spawnObject, linkToPrefab, go, rotRangeX, rotRangeY, rotRangeZ, isSnapRot, isSnapRotX, isSnapRotY, isSnapRotZ, snapRotX, snapRotY, snapRotZ, lookAtTarget, lookAtX, heightRange;
        SerializedProperty includeTerrainHeight, includeTerrainAngle;
        SerializedProperty customScaleRange, scaleRangeX, scaleRangeY, scaleRangeZ, parentMode, parentName, parentT, parentToTerrain;

        // Portal
        SerializedProperty portalNode;

        SPCurve localCurve = new SPCurve();
        SPCurve worldCurve = new SPCurve();

        TC_ItemBehaviour item;
        TC_TerrainLayer layerLevel;
        TC_LayerGroup layerGroup;
        TC_LayerGroupResult groupResult;
        TC_Layer layer;
        TC_NodeGroup nodeGroup;
        TC_Node node;
        TC_SelectItemGroup selectItemGroup;
        TC_SelectItem selectItem;

        TC_Settings settings;
        TC_Generate generate;

        Event eventCurrent;

        float scaleYOld;

        string[] layers;
        
        public class SPCurve
        {
            public SerializedProperty curveEntry;
            public SerializedProperty active;
            public SerializedProperty range;
            public SerializedProperty curve;
            public SerializedProperty type;

            public void Init(TC_ItemBehaviourEditor editor, string name)
            {
                curveEntry = editor.serializedObject.FindProperty(name);
                active = curveEntry.FindPropertyRelative("active");
                range = curveEntry.FindPropertyRelative("range");
                curve = curveEntry.FindPropertyRelative("curve");
                type = curveEntry.FindPropertyRelative("type");
            }
        }

        void OnEnable()
        {
            // ((TC_ItemBehaviour)target).t.hideFlags = HideFlags.None;
            item = (TC_ItemBehaviour)target;
            if (item == null) return;

            Undo.undoRedoPerformed += Repaint;
            TC_ItemBehaviour.DoRepaint += Repaint;

            layerLevel = target as TC_TerrainLayer;
            layerGroup = target as TC_LayerGroup;
            groupResult = target as TC_LayerGroupResult;
            layer = target as TC_Layer;
            nodeGroup = target as TC_NodeGroup;
            node = target as TC_Node;
            selectItemGroup = target as TC_SelectItemGroup;
            selectItem = target as TC_SelectItem;

            localCurve.Init(this, "localCurve");
            worldCurve.Init(this, "worldCurve");


            notes = serializedObject.FindProperty("notes");
            isLocked = serializedObject.FindProperty("isLocked");
            lockTransform = serializedObject.FindProperty("lockTransform");
            lockPosChildren = serializedObject.FindProperty("lockPosChildren");

            lockPosX = serializedObject.FindProperty("lockPosX");
            lockPosY = serializedObject.FindProperty("lockPosY");
            lockPosZ = serializedObject.FindProperty("lockPosZ");
            lockRotY = serializedObject.FindProperty("lockRotY");
            lockScaleX = serializedObject.FindProperty("lockScaleX");
            lockScaleY = serializedObject.FindProperty("lockScaleY");
            lockScaleZ = serializedObject.FindProperty("lockScaleZ");
            
            posOffset = serializedObject.FindProperty("posOffset");
            positionMode = serializedObject.FindProperty("positionMode");
            posY = serializedObject.FindProperty("posY");
            size = serializedObject.FindProperty("size");
            overlay = serializedObject.FindProperty("overlay");
            method = serializedObject.FindProperty("method");
            doNormalize = serializedObject.FindProperty("doNormalize");

            if (layerGroup != null || groupResult != null || layer != null || nodeGroup != null)
            {
                seed2 = serializedObject.FindProperty("seed");
            }

            if (layer != null) distanceRules = serializedObject.FindProperty("distanceRules");
            
            TC_Reporter.Log("OnEnable");

            if (layerLevel != null)
            {
                treeResolutionPM = serializedObject.FindProperty("treeResolutionPM");
                objectResolutionPM = serializedObject.FindProperty("objectResolutionPM");
                objectAreaSize = serializedObject.FindProperty("objectAreaSize");
                objectTransform = serializedObject.FindProperty("objectTransform");
                colormapResolution = serializedObject.FindProperty("colormapResolution");
                meshResolution = serializedObject.FindProperty("meshResolution");
            }
            else if (node != null)
            {
                inputKind = serializedObject.FindProperty("inputKind");
                inputTerrain = serializedObject.FindProperty("inputTerrain");
                inputNoise = serializedObject.FindProperty("inputNoise");
                inputShape = serializedObject.FindProperty("inputShape");
                inputFile = serializedObject.FindProperty("inputFile");
                inputCurrent = serializedObject.FindProperty("inputCurrent");

                splatSelectIndex = serializedObject.FindProperty("splatSelectIndex");

                iterations = serializedObject.FindProperty("iterations");
                mipmapLevel = serializedObject.FindProperty("mipmapLevel");
                convexityStrength = serializedObject.FindProperty("convexityStrength");
                convexityMode = serializedObject.FindProperty("convexityMode");
                blurMode = serializedObject.FindProperty("blurMode");

                noise = serializedObject.FindProperty("noise");

                portalNode = serializedObject.FindProperty("portalNode");

                if (noise != null)
                {
                    // frequency = noise.FindPropertyRelative("frequency");
                    lacunarity = noise.FindPropertyRelative("lacunarity");
                    octaves = noise.FindPropertyRelative("octaves");
                    persistence = noise.FindPropertyRelative("persistence");
                    seed = noise.FindPropertyRelative("seed");
                    amplitude = noise.FindPropertyRelative("amplitude");
                    warp0 = noise.FindPropertyRelative("warp0");
                    warp = noise.FindPropertyRelative("warp");
                    damp0 = noise.FindPropertyRelative("damp0");
                    damp = noise.FindPropertyRelative("damp");
                    dampScale = noise.FindPropertyRelative("dampScale");

                    cellType = noise.FindPropertyRelative("cellType");
                    distanceFunction = noise.FindPropertyRelative("distanceFunction");

                    noiseMode = noise.FindPropertyRelative("mode");
                    cellNoiseMode = noise.FindPropertyRelative("cellMode");
                }
                else TC_Reporter.Log("No noise init");

                wrapMode = serializedObject.FindProperty("wrapMode");
                radius = serializedObject.FindProperty("radius");

                stampTex = serializedObject.FindProperty("stampTex");
                pathTexStamp = serializedObject.FindProperty("pathTexStamp");
                rawImage = serializedObject.FindProperty("rawImage");
                collisionMask = serializedObject.FindProperty("collisionMask");
                collisionMode = serializedObject.FindProperty("collisionMode");
                // collisionDirection = serializedObject.FindProperty("collisionDirection");
                range = serializedObject.FindProperty("range");
                heightDetectRange = serializedObject.FindProperty("heightDetectRange");
                includeTerrainHeight = serializedObject.FindProperty("includeTerrainHeight");
                
                shapes = serializedObject.FindProperty("shapes");

                if (shapes != null)
                {
                    topSize = shapes.FindPropertyRelative("topSize");
                    bottomSize = shapes.FindPropertyRelative("bottomSize");
                    shapeSize = shapes.FindPropertyRelative("size");
                }

                detectRange = serializedObject.FindProperty("detectRange");
            }
            else if (selectItemGroup != null)
            {
                mix = serializedObject.FindProperty("mix");
                scaleMinMaxMulti = serializedObject.FindProperty("scaleMinMaxMulti");
                scaleMulti = serializedObject.FindProperty("scaleMulti");
                linkScaleToMask = serializedObject.FindProperty("linkScaleToMask");
                linkScaleToMaskAmount = serializedObject.FindProperty("linkScaleToMaskAmount");
            }
            else if (selectItem != null)
            {
                selectIndex = serializedObject.FindProperty("selectIndex");
                size = serializedObject.FindProperty("size");
                wrapMode = serializedObject.FindProperty("wrapMode");
                // splatCustom = serializedObject.FindProperty("splatCustom");
                // splatCustomValues = serializedObject.FindProperty("splatCustomValues");
                distanceRules = serializedObject.FindProperty("distanceRules");
                texColor = serializedObject.FindProperty("texColor");
                brightness = serializedObject.FindProperty("brightness");
                saturation = serializedObject.FindProperty("saturation");

                if (selectItem.outputId == TC.treeOutput)
                {
                    tree = serializedObject.FindProperty("tree");

                    heightOffset = tree.FindPropertyRelative("heightOffset");
                    randomPosition = tree.FindPropertyRelative("randomPosition");
                     
                    scaleRange = tree.FindPropertyRelative("scaleRange");
                    scaleMulti = tree.FindPropertyRelative("scaleMulti");
                    nonUniformScale = tree.FindPropertyRelative("nonUniformScale");
                    scaleCurve = tree.FindPropertyRelative("scaleCurve");
                }
                else if (selectItem.outputId == TC.objectOutput)
                {
                    spawnObject = serializedObject.FindProperty("spawnObject");
                    go = spawnObject.FindPropertyRelative("go");

                    linkToPrefab = spawnObject.FindPropertyRelative("linkToPrefab");
                    parentMode = spawnObject.FindPropertyRelative("parentMode");
                    parentName = spawnObject.FindPropertyRelative("parentName");
                    parentT = spawnObject.FindPropertyRelative("parentT");
                    parentToTerrain = spawnObject.FindPropertyRelative("parentToTerrain");
                    
                    heightRange = spawnObject.FindPropertyRelative("heightRange");
                    heightOffset = spawnObject.FindPropertyRelative("heightOffset");

                    randomPosition = spawnObject.FindPropertyRelative("randomPosition");
                    includeTerrainHeight = spawnObject.FindPropertyRelative("includeTerrainHeight");
                    includeTerrainAngle = spawnObject.FindPropertyRelative("includeTerrainAngle");
                    
                    rotRangeX = spawnObject.FindPropertyRelative("rotRangeX");
                    rotRangeY = spawnObject.FindPropertyRelative("rotRangeY");
                    rotRangeZ = spawnObject.FindPropertyRelative("rotRangeZ");

                    isSnapRot = spawnObject.FindPropertyRelative("isSnapRot");
                    isSnapRotX = spawnObject.FindPropertyRelative("isSnapRotX");
                    isSnapRotY = spawnObject.FindPropertyRelative("isSnapRotY");
                    isSnapRotZ = spawnObject.FindPropertyRelative("isSnapRotZ");

                    snapRotX = spawnObject.FindPropertyRelative("snapRotX");
                    snapRotY = spawnObject.FindPropertyRelative("snapRotY");
                    snapRotZ = spawnObject.FindPropertyRelative("snapRotZ");

                    customScaleRange = spawnObject.FindPropertyRelative("customScaleRange");
                    scaleRangeX = spawnObject.FindPropertyRelative("scaleRangeX");
                    scaleRangeY = spawnObject.FindPropertyRelative("scaleRangeY");
                    scaleRangeZ = spawnObject.FindPropertyRelative("scaleRangeZ");

                    scaleRange = spawnObject.FindPropertyRelative("scaleRange");
                    scaleMulti = spawnObject.FindPropertyRelative("scaleMulti");
                    nonUniformScale = spawnObject.FindPropertyRelative("nonUniformScale");
                    scaleCurve = spawnObject.FindPropertyRelative("scaleCurve");

                    lookAtTarget = spawnObject.FindPropertyRelative("lookAtTarget");
                    lookAtX = spawnObject.FindPropertyRelative("lookAtX");
                }
            }
        }

        void OnDisable()
        {
            if (item != null)
            {
                if (item.controlDown)
                {
                    if (item.lockPosChildren)
                    {
                        item.lockPosChildren = false;
                        item.SetLockChildrenPosition(false);
                    }
                    item.controlDown = false;
                }
                TC_ItemBehaviour.DoRepaint -= Repaint;
            }
            Tools.hidden = false;
            Undo.undoRedoPerformed -= Repaint; 
        }
         
        void OnDestroy()
        {
            Undo.undoRedoPerformed -= Repaint;
            item = (TC_ItemBehaviour)target;
            if (item != null) TC_ItemBehaviour.DoRepaint -= Repaint; 
        }

        bool keyUp, cmdDuplicate, cmdDelete;
        
        void OnSceneGUI()
        {
            eventCurrent = Event.current;

            if (eventCurrent.type == EventType.KeyUp) keyUp = true;
            else if (eventCurrent.type == EventType.KeyDown) keyUp = false;

            if ((eventCurrent.commandName == "Delete" || eventCurrent.commandName == "SoftDelete") && eventCurrent.type == EventType.Repaint) { eventCurrent.Use(); cmdDelete = true; }
            else if (eventCurrent.commandName == "Duplicate" && eventCurrent.type == EventType.Repaint) { eventCurrent.Use(); cmdDuplicate = true; }
            
            if (cmdDelete && keyUp) { cmdDelete = keyUp = false; TC_NodeWindow.DeleteKey(); return; }
            else if (cmdDuplicate && keyUp) { cmdDuplicate = keyUp = false; TC_NodeWindow.DuplicateKey(); return; }
            
            if (Tools.current == Tool.Rotate || Tools.current == Tool.Move || Tools.current == Tool.Scale) Tools.hidden = true;
            else { Tools.hidden = false; return; }

            if (selectItemGroup != null) return;

            if (selectItem != null)
            {
                if (selectItem.parentItem == null) return;
                if (selectItem.parentItem.itemList.Count > 1) return;
                if (selectItem.texColor == null) return;
            }

            CheckKeyLockOnSelection(eventCurrent);

            if (Tools.current == Tool.Move)
            {
                Undo.RecordObject(item.transform, "Move");
                Vector3 posOld;

                Vector3 pos = posOld = item.t.position;

                // if (GUIUtility.hotControl != 0 && node != null) pos.y *= node.t.lossyScale.y;
                
                GUI.changed = false;
                Undo.RecordObject(item.t, "Edit Transform");
                Undo.RecordObject(item, "Edit Transform");
                
                pos = Handles.PositionHandle(pos, Quaternion.identity);

                if (item.lockTransform)
                {
                    if (item.lockPosX && pos.x != posOld.x) { TC.AddMessage(item.name + " position X is locked."); pos.x = posOld.x; }
                    if (item.lockPosY && pos.y != posOld.y) { TC.AddMessage(item.name + " position Y is locked."); pos.y = posOld.y; }
                    if (item.lockPosZ && pos.z != posOld.z) { TC.AddMessage(item.name + " position Z is locked."); pos.z = posOld.z; }
                }
                
                if (!item.lockPosParent)
                {
                    if (item.t.position != pos) item.t.position = pos;

                    bool posYChanged = false;

                    if (!(item.lockTransform && item.lockPosY))
                    {
                        if (node == null)
                        {
                            float deltaY = pos.y - posOld.y;

                            if (deltaY != 0)
                            {
                                item.ChangeYPosition(deltaY);
                                posYChanged = true;
                                item.posY = pos.y;
                            }
                            if (GUIUtility.hotControl == 0) item.posY = 0;
                        }
                        else
                        {
                            if (GUIUtility.hotControl != 0)
                            {
                                float newPosY = (pos.y / node.t.lossyScale.y);
                                node.posY = node.posYOld + newPosY;
                                if (node.posY != node.posYOld) posYChanged = true;
                            }
                            else node.posYOld = node.posY;
                        }

                        if (GUIUtility.hotControl == 0)
                        {
                            item.t.position = new Vector3(pos.x, 0, pos.z);
                            item.t.hasChanged = false;
                        }
                    }

                    if ((pos.x != posOld.x || pos.z != posOld.z) || posYChanged)
                    {
                        item.UpdateTransforms();
                        item.t.hasChanged = false;
                        AutoGenerate();
                        DoRepaint();
                    }
                }
                else if (GUI.changed) TC.AddMessage(item.name + " positioning is locked.");
            }
            
            else if (Tools.current == Tool.Rotate)
            {
                Undo.RecordObject(item.transform, "Rotate");
                Handles.color = Color.blue;
                Handles.Slider(item.t.position, item.t.localRotation * new Vector3(0, 0, 1));
                #if UNITY_5_1 || UNITY_5_2 || UNITY_5_3 || UNITY_5_4 || UNITY_5_5
                Handles.SphereCap(0, item.t.position, Quaternion.identity, 0.15f * HandleUtility.GetHandleSize(item.t.position));
                #else
                Handles.SphereHandleCap(0, item.t.position, Quaternion.identity, 0.15f * HandleUtility.GetHandleSize(item.t.position), EventType.Repaint);
                #endif
                Handles.color = Color.white; 
                GUI.changed = false;
                
                Quaternion rot = Handles.Disc(item.t.rotation, item.t.position, new Vector3(0, 1, 0), 1.5f * HandleUtility.GetHandleSize(item.t.position), false, 0);

                if (!(item.lockTransform && item.lockRotY))
                {
                    if (item.t.rotation != rot)
                    {
                        item.t.rotation = rot;
                        item.UpdateTransforms();
                        item.t.hasChanged = false;
                        AutoGenerate();
                        DoRepaint();
                    }
                }
                else { TC.AddMessage(item.name + " rotation Y is locked."); }
            }
            else if (Tools.current == Tool.Scale)
            {
                Undo.RecordObject(item.transform, "Scale");
                Vector3 scaleOld = item.t.localScale;
                GUI.changed = false;
                Vector3 scale = Handles.ScaleHandle(scaleOld, item.t.position, node != null ? item.t.rotation : Quaternion.identity, HandleUtility.GetHandleSize(item.t.position));

                bool freezeScaleY = false;

                if (node != null)
                {
                    if (node.nodeType == NodeGroupType.Mask) freezeScaleY = true;
                }

                if (item.outputId != TC.heightOutput) freezeScaleY = true;

                if (freezeScaleY)
                {
                    if (scale.x != scaleOld.x || scale.z != scaleOld.z) scale.y = scaleOld.y;
                }

                if (node == null)
                {
                    if (scale.x != scaleOld.x) scale.z = scale.x;
                    else if (scale.z != scaleOld.z) scale.x = scale.z;
                }

                if (item.lockTransform)
                {
                    if (item.lockScaleX && scale.x != scaleOld.x) { TC.AddMessage(item.name + " scale X is locked."); scale.x = scaleOld.x; }
                    if (item.lockScaleY && scale.y != scaleOld.y) { TC.AddMessage(item.name + " scale Y is locked."); scale.y = scaleOld.y; }
                    if (item.lockScaleZ && scale.z != scaleOld.z) { TC.AddMessage(item.name + " scale Z is locked."); scale.z = scaleOld.z; }
                }
                
                if (scale.x != scaleOld.x || scale.y != scaleOld.y || scale.z != scaleOld.z)
                {
                    item.t.localScale = scale;
                    item.UpdateTransforms();
                    item.t.hasChanged = false;
                    AutoGenerate();
                    DoRepaint();
                }
            }

            if (node != null)
            {
                if (node.portalNode != null)
                {
                    node.portalNode.t.position = item.t.position;
                    node.portalNode.t.rotation = item.t.rotation;
                    node.portalNode.t.localScale = item.t.localScale;
                }
            }
        }

        public void DoRepaint()
        {
            TC.repaintNodeWindow = true;
            Repaint();
        }

        public override void OnInspectorGUI()
        {
            settings = TC_Settings.instance;
            generate = TC_Generate.instance;

            if (settings == null) return;
            serializedObject.Update();
            
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space((EditorGUIUtility.currentViewWidth / 2) - 130);

            string control = GUI.GetNameOfFocusedControl();
            if (control == "PreviewEdit") { GUI.color = Color.red; TD.PreviewEdit(item); }

            GUI.SetNextControlName("PreviewEdit");
            if (GUILayout.Button("", GUILayout.Width(260), GUILayout.Height(260))) GUI.FocusControl("PreviewEdit");
            GUI.color = Color.white;
            Rect rect = GUILayoutUtility.GetLastRect();
            EditorGUILayout.EndHorizontal();

            Rect previewTexRect = new Rect((EditorGUIUtility.currentViewWidth / 2) - 111, rect.y + 5f, 250, 250);

            bool splatCustom = false;
            if (selectItem != null)
            {
                if (selectItem.splatCustom) splatCustom = true;
            }

            bool drawColor = false;

            if (selectItem != null && item.outputId == TC.colorOutput)
            {
                if (selectItem.parentItem.itemList.Count != 1 || selectItem.texColor == null) drawColor = true;
            }

            if (drawColor)
            {
                GUI.color = selectItem.color;
                EditorGUI.DrawPreviewTexture(previewTexRect, Texture2D.whiteTexture);
                GUI.color = Color.white;
            }
            else if (splatCustom) TC_SelectItemGUI.DrawSplatCustomPreview(selectItem, previewTexRect);
            else if (item.rtDisplay != null) EditorGUI.DrawPreviewTexture(previewTexRect, item.rtDisplay);
            else if (item.preview.tex != null) EditorGUI.DrawPreviewTexture(previewTexRect, item.preview.tex);

            TD.DrawSpacer();

            if (node != null)
            {
                bool drawNode;
                if (node.inputKind == InputKind.Terrain && node.inputTerrain == InputTerrain.Collision) drawNode = false; else drawNode = true;

                if (drawNode)
                {
                    DrawCustomTransform();
                    DrawImageSettings();
                    TD.DrawSpacer();
                }
            }

            if (layerGroup != null)
            {
                if (layerGroup.level == 0)
                {
                    if (layerGroup.outputId == TC.heightOutput) DrawTerrainHeightSlider(new Rect(previewTexRect.x + 265, previewTexRect.y, 25, previewTexRect.height));
                    else if (layerGroup.outputId == TC.treeOutput || layerGroup.outputId == TC.objectOutput || layerGroup.outputId == TC.colorOutput) DrawLayerGroupOutput();
                }
            }

            if (layerGroup != null || groupResult != null || layer != null || nodeGroup != null)
            {
                GUI.color = Color.red * TD.editorSkinMulti;
                EditorGUILayout.BeginVertical("Box");
                GUI.color = Color.white;

                DrawGlobalScale();

                string tooltip = "The pivot can be moved without affecting the position of all the children.\nWhen rescaling the scale will be taken from each node seperately.\n\nPress 'F' key to toggle.\nHold 'Control' key to enable.";

                TD.DrawProperty(lockPosChildren, new GUIContent("Lock Position Children",tooltip));

                if (GUI.changed)
                {
                    item.lockPosChildren = lockPosChildren.boolValue;
                    item.SetLockChildrenPosition(false);
                }

                EditorGUILayout.EndVertical();

                DrawFlipScale(true);

                GUI.color = Color.red * TD.editorSkinMulti;
                EditorGUILayout.BeginVertical("Box");
                GUI.color = Color.white;

                TD.DrawProperty(seed2);

                EditorGUILayout.EndVertical();
            }

            if (layerLevel != null)
            {
                TD.DrawProperty(treeResolutionPM, new GUIContent("Tree resolution per meter"));
                TD.DrawProperty(objectResolutionPM);
                TD.DrawProperty(objectAreaSize);
                TD.DrawProperty(objectTransform);
                TD.DrawProperty(colormapResolution);
                TD.DrawProperty(meshResolution);
            }

            if (item.outputId == TC.splatOutput)
            {
                if (layer != null || layerGroup != null)
                {
                    GUI.color = Color.red * TD.editorSkinMulti;
                    EditorGUILayout.BeginVertical("Box");
                    GUI.color = Color.white;
                        TD.DrawProperty(doNormalize, new GUIContent("Normalize"));
                    EditorGUILayout.EndVertical();
                }
            }

            bool drawSpacer = true;

            if (node != null)
            {
                GUI.color = Color.white;
                TD.DrawProperty(inputKind);
                if (GUI.changed)
                {
                    node.Init();
                    TC.RefreshOutputReferences(node.outputId);
                }

                if (node.inputKind == InputKind.Terrain)
                {
                    if (node.outputId == TC.heightOutput)
                    {
                        InputTerrainHeight popup = InputTerrainHeight.Collision;
                        EditorGUILayout.EnumPopup("Input Terrain", popup);
                        inputTerrain.enumValueIndex = (int)popup;
                    }
                    else TD.DrawProperty(inputTerrain);

                    if (node.inputTerrain == InputTerrain.Convexity)
                    {
                        TD.DrawProperty(convexityMode);
                        if (convexityStrength.floatValue < 0)
                        {
                            convexityMode.enumValueIndex = 1;
                            convexityStrength.floatValue *= -1;
                        }
                        TD.DrawProperty(convexityStrength, new GUIContent("Convexity Strength"));
                        if (GUI.changed)
                        {
                            if (convexityStrength.floatValue < 0) convexityStrength.floatValue = 0;
                        }
                        DrawIntSlider(mipmapLevel, 1, 11, new GUIContent("Radius"));
                    }
                    else if (node.inputTerrain == InputTerrain.Splatmap)
                    {
                        if (settings.hasMasterTerrain)
                        {
                            EditorGUILayout.BeginHorizontal();

                            Texture2D splatTexture;
                            int splatLength = TC.GetTerrainSplatTexture(settings.masterTerrain, splatSelectIndex.intValue, out splatTexture);
                            
                            DrawIntSlider(splatSelectIndex, 0, splatLength - 1, new GUIContent("Splat Index"));
                            EditorGUILayout.EndHorizontal();
                            EditorGUILayout.BeginHorizontal();
                            GUILayout.Space(5);
                            EditorGUILayout.PrefixLabel(" ");
                            if (splatSelectIndex.intValue < splatLength)
                            {
                                DrawPreviewTexture(splatTexture, Color.white, Color.white, 150, 150);
                            }

                            EditorGUILayout.EndHorizontal();
                        }
                    }
                    else if (node.inputTerrain == InputTerrain.Normal)
                    {
                        DrawCommingSoon("The normal node will be added in the next release.");
                    }
                    else if (node.inputTerrain == InputTerrain.Collision)
                    {
                        if (layers == null) layers = new string[32];
                        else if (layers.Length != 32) layers = new string[32];

                        for (int i = 0; i < 32; i++)
                        {
                            layers[i] = LayerMask.LayerToName(i);
                            if (layers[i] == "") layers[i] = "Default";
                        }

                        GUI.changed = false;
                        int mask = collisionMask.intValue;
                        mask = EditorGUILayout.MaskField(new GUIContent("Collision Mask"), mask, layers);
                        if (GUI.changed)
                        {
                            collisionMask.intValue = mask;
                            AutoGenerate();
                        }

                        GUILayout.Space(5);

                        TD.DrawProperty(collisionMode);
                        // TD.DrawProperty(collisionDirection);
                        EditorGUILayout.BeginHorizontal();
                        TD.DrawProperty(heightDetectRange, new GUIContent("Height Detect Range"));
                        if (heightDetectRange.boolValue)
                        {
                            TD.DrawProperty(range, new GUIContent(""));
                        }
                        EditorGUILayout.EndHorizontal();
                        // if (heightDetectRange.boolValue) TD.DrawProperty(includeTerrainHeight, new GUIContent("  Include Terrain Height"));
                    }
                }
                else if (node.inputKind == InputKind.Noise) { DrawNoise(); }
                else if (node.inputKind == InputKind.Shape) { DrawShape(); }
                else if (node.inputKind == InputKind.File)
                {
                    TD.DrawProperty(inputFile);

                    if (node.inputFile == InputFile.RawImage) DrawRawImage();
                    else if (node.inputFile == InputFile.Image) DrawImage();
                }
                else if (node.inputKind == InputKind.Current)
                {
                    TD.DrawProperty(inputCurrent);

                    if (node.inputCurrent == InputCurrent.Blur)
                    {
                        TD.DrawProperty(blurMode);
                    }
                    else if (node.inputCurrent == InputCurrent.EdgeDetect)
                    {
                        DrawMinMaxSlider(detectRange, 1.0f / 255f, 1, 1.0f / 255f, new GUIContent("Detect Range"));
                        // if (threshold.vector2Value.x < 1.0f / 255f) threshold.vector2Value = new Vector2(1.0f / 255f , threshold.vector2Value.y);
                        // else if (threshold.floatValue > 1) threshold.floatValue = 1;
                    }
                    else if (node.inputCurrent == InputCurrent.Distortion)
                    {
                        TD.DrawProperty(radius);
                        DrawNoise();
                    }

                    if (node.inputCurrent != InputCurrent.Distortion && node.inputCurrent != InputCurrent.EdgeDetect)
                    {
                        // DrawIntSlider(iterations, 1, 30);
                        TD.DrawProperty(iterations);
                        if (iterations.intValue < 1) iterations.intValue = 1;
                        else if (iterations.intValue > 1000) iterations.intValue = 1000;
                    }
                }
                else if (node.inputKind == InputKind.Portal)
                {
                    DrawPortal();
                }
            }
            else if (selectItemGroup != null)
            {
                if (selectItemGroup.outputId != TC.heightOutput) DrawSelectItemGroup();
            }
            else if (selectItem != null)
            {
                if (selectItem.outputId == TC.colorOutput) { DrawColorSelectItem(); drawSpacer = false; }
                else if (selectItem.outputId == TC.splatOutput) DrawSelectItem();
                else if (selectItem.outputId == TC.grassOutput) DrawSelectItem();
                else if (selectItem.outputId == TC.treeOutput) DrawTreeSelectItem();
                else if (selectItem.outputId == TC.objectOutput) DrawObjectSelectItem();
            }

            if (nodeGroup != null || node != null)
            {
                TD.DrawSpacer();
                TD.DrawLabelWidthUnderline("Curves", 14);
                DrawCurve(localCurve, "Local Height Curve");
                GUILayout.Space(5);
                DrawCurve(worldCurve, "Global Height Curve");
            }

            if (generate.generateDone != generate.generateDoneOld)
            {
                // Debug.Log("Generate Done");
                Repaint();
                generate.generateDoneOld = generate.generateDone;
            }

            // DrawMethod();
            // Draw Inspector
            // if (node == null)
            
            if (layerGroup != null)
            {
                if (layerGroup.level == 0)
                {
                    if (layerGroup.outputId == TC.heightOutput) DrawExport(settings.heightmapFilename, 0);
                    if (layerGroup.outputId == TC.splatOutput) DrawExport(settings.splatmapFilename, 1);
                    else if (layerGroup.outputId == TC.colorOutput) DrawExport(settings.colormapFilename, 2);
                }
            }

            DrawUsedAsPortal();

            if (drawSpacer) TD.DrawSpacer();

            //if (layer != null)
            //{
            //    if (layer.outputId == TC.treeOutput || layer.outputId == TC.objectOutput)
            //    {
            //        DrawDistantRules();
            //        TD.DrawSpacer();
            //    }
            //}

            TD.DrawLabelWidthUnderline("Notes", 14);

            notes.stringValue = EditorGUILayout.TextArea(notes.stringValue);

            if (settings.drawDefaultInspector) base.OnInspectorGUI();  
            TD.DrawSpacer();
            
            serializedObject.ApplyModifiedProperties();

            if (selectItem == null && selectItemGroup == null)
            {
                if (item.t.hasChanged)
                {
                    item.t.hasChanged = false;
                    if (node == null)
                    {
                        Vector3 scale = item.t.localScale;
                        scale.z = scale.x;
                        item.t.localScale = scale;
                    }
                    // Debug.Log(item.name);
                    AutoGenerate();
                }
            }
        }

        void DrawUsedAsPortal()
        {
            if (item.usedAsPortalList != null && item.usedAsPortalList.Count > 0)
            {
                TD.DrawSpacer();
                TD.DrawLabelWidthUnderline("Used as a Portal", 14);
                List<TC_ItemBehaviour> usedAsPortalList = item.usedAsPortalList;

                GUI.color = Color.cyan * TD.editorSkinMulti;
                EditorGUILayout.BeginVertical("Box");
                GUI.color = Color.white;

                for (int i = 0; i < usedAsPortalList.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    if (usedAsPortalList[i] == null) { item.isPortalCount--; usedAsPortalList.RemoveAt(i--); EditorUtility.SetDirty(item); continue; }
                    EditorGUILayout.ObjectField(usedAsPortalList[i], typeof(TC_ItemBehaviour), true);
                    
                    if (GUILayout.Button("Select", EditorStyles.miniButtonMid, GUILayout.Width(70)))
                    {
                        Selection.activeTransform = usedAsPortalList[i].t;
                        DoRepaint();
                    }
                    EditorGUILayout.EndHorizontal();
                }

                EditorGUILayout.EndVertical();
            }
        }

        void DrawPortal()
        {
            EditorGUILayout.BeginHorizontal();
            {
                bool guiChangedOld = GUI.changed;
                GUI.changed = false;
                TC_ItemBehaviour oldPortalNode = portalNode.objectReferenceValue as TC_ItemBehaviour;
                EditorGUILayout.PropertyField(portalNode);

                TC_ItemBehaviour _portalNode = portalNode.objectReferenceValue as TC_ItemBehaviour;
                if (_portalNode != null)
                {
                    // EditorGUILayout.LabelField(_portalNode.isPortalCount.ToString(), GUILayout.Width(25));
                    if (GUILayout.Button("X", EditorStyles.miniButtonMid, GUILayout.Width(25)))
                    {
                        portalNode.objectReferenceValue = null;
                        GUI.changed = true;
                    }
                    GUILayout.Space(25);
                }

                if (GUI.changed)
                {
                    _portalNode = portalNode.objectReferenceValue as TC_ItemBehaviour;
                    bool verifyPortal = TC.VerifyPortal(_portalNode);
                    
                    if (oldPortalNode != null && (_portalNode == null || verifyPortal))
                    {
                        item.RemoveFromPortalNode();
                    }
                    
                    if (_portalNode != null)
                    {
                        if (verifyPortal)
                        {
                            _portalNode.isPortalCount++;
                            if (_portalNode.usedAsPortalList == null) _portalNode.usedAsPortalList = new List<TC_ItemBehaviour>();
                            _portalNode.usedAsPortalList.Add(item);
                            item.CopyTransform(_portalNode);
                        }
                        else portalNode.objectReferenceValue = oldPortalNode;
                    }
                    TC.RefreshOutputReferences(item.outputId);

                    Selection.activeTransform = node.t;
                    guiChangedOld = true;
                }
                GUI.changed = guiChangedOld;
                if (portalNode.objectReferenceValue != null)
                {
                    if (GUILayout.Button("Select", EditorStyles.miniButtonMid, GUILayout.Width(70)))
                    {
                        Selection.activeTransform = ((TC_ItemBehaviour)portalNode.objectReferenceValue).t;
                        DoRepaint();
                    }
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        void DrawCustomTransform()
        {
            GUI.color = Color.red * TD.editorSkinMulti;
            EditorGUILayout.BeginVertical("Box");
            GUI.color = Color.white;

            DrawGlobalScale();

            TD.DrawProperty(posY, new GUIContent("Height"));
            TD.DrawProperty(size);
            // TD.DrawProperty(clamp);

            bool fileInput = false;

            if (node != null)
            {
                if (node.inputKind == InputKind.File) fileInput = true;
            }
            else fileInput = true;

            if (fileInput)
            {
                TD.DrawProperty(wrapMode);
            }
            else
            {
                WrapMode popup = (WrapMode)Mathf.Clamp(wrapMode.enumValueIndex - 1, 0, 1);
                GUI.changed = false;
                popup = (WrapMode)EditorGUILayout.EnumPopup("Wrap Mode", popup);
                wrapMode.enumValueIndex = (int)popup + 1;
                if (GUI.changed)
                {
                    AutoGenerate();
                }
            }

            DrawFlipScale(false);

            EditorGUILayout.EndVertical();
        }
        void DrawDistantRules()
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(0);
            Rect rect = GUILayoutUtility.GetLastRect();
            distanceRules.isExpanded = EditorGUI.Foldout(new Rect(rect.x, rect.y + 3, 25, 18), distanceRules.isExpanded, "");
            if (distanceRules.isExpanded) TD.DrawLabelWidthUnderline(distanceRules.displayName, 12); else TD.DrawLabel(distanceRules.displayName, 12);
            EditorGUILayout.EndHorizontal();

            if (distanceRules.isExpanded)
            {
                GUILayout.Space(5);
                EditorGUILayout.BeginVertical("Box");
                EditorGUILayout.BeginHorizontal();
                    distanceRules.arraySize = EditorGUILayout.IntField("Size", distanceRules.arraySize);
                    if (GUILayout.Button("+", EditorStyles.miniButtonMid, GUILayout.Width(25)))
                    {
                        distanceRules.InsertArrayElementAtIndex(0);
                    }
                EditorGUILayout.EndHorizontal();
                
                EditorGUI.indentLevel++;

                for (int i = 0; i < distanceRules.arraySize; i++)
                {
                    SerializedProperty elementProperty = distanceRules.GetArrayElementAtIndex(i);

                    EditorGUILayout.BeginHorizontal();
                        TD.DrawProperty(elementProperty, new GUIContent("Rule " + (i + 1)));
                        if (GUILayout.Button("-", EditorStyles.miniButtonMid, GUILayout.Width(25)))
                        {
                            distanceRules.DeleteArrayElementAtIndex(i--); continue;
                        }
                    EditorGUILayout.EndHorizontal();

                    if (elementProperty.isExpanded)
                    {
                        SerializedProperty range = elementProperty.FindPropertyRelative("range");
                        if (range != null)
                        {
                            DrawVector2(range, false, "Min", "Max", new GUIContent("Distance Range"), 55, 55);
                        }

                        SerializedProperty ruleItems = elementProperty.FindPropertyRelative("items");
                        if (ruleItems != null)
                        {
                            EditorGUI.indentLevel++;
                            DrawRuleItmes(ruleItems);
                            EditorGUI.indentLevel--;
                        }
                    }
                }

                EditorGUI.indentLevel--;
                EditorGUILayout.EndVertical();
            }
        }

        void DrawRuleItmes(SerializedProperty ruleItems)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(0);
            Rect rect = GUILayoutUtility.GetLastRect();
            ruleItems.isExpanded = EditorGUI.Foldout(new Rect(rect.x, rect.y + 3, 25, 18), ruleItems.isExpanded, "");
            TD.DrawLabel(ruleItems.displayName, 12);
            EditorGUILayout.EndHorizontal();

            if (ruleItems.isExpanded)
            {
                EditorGUILayout.BeginVertical("Box");
                EditorGUI.indentLevel++;
                ruleItems.arraySize = EditorGUILayout.IntField("Size", ruleItems.arraySize);
                EditorGUI.indentLevel--;

                EditorGUI.indentLevel++;

                for (int i = 0; i < ruleItems.arraySize; i++)
                {
                    SerializedProperty elementProperty = ruleItems.GetArrayElementAtIndex(i);

                    TD.DrawProperty(elementProperty, new GUIContent("Item " + (i + 1)));
                    DrawRuleItem(elementProperty);
                }
                EditorGUI.indentLevel--;
                EditorGUILayout.EndVertical();
            }
        }

        void DrawRuleItem(SerializedProperty ruleItemElement)
        {
            TC_ItemBehaviour ruleItem = ruleItemElement.objectReferenceValue as TC_ItemBehaviour;
            if (ruleItem == null) return;

            TC_Layer ruleLayer = ruleItem as TC_Layer;

            if (ruleLayer != null)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(60);
                for (int i = 0; i < ruleLayer.selectItemGroup.itemList.Count; i++)
                {
                    TC_SelectItem selectItem = ruleLayer.selectItemGroup.itemList[i];
                    GUILayout.Button(new GUIContent(selectItem.preview.tex), GUILayout.Width(50), GUILayout.Height(50));
                }
                EditorGUILayout.EndHorizontal();
            }
        }

        void DrawTerrainDataFiles()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(" ");
            if (GUILayout.Button("+", GUILayout.Width(25)))
            {
                settings.importTerrains.Add(null);
            }

            EditorGUILayout.EndHorizontal();

            int tileX = 0;
            int tileY = 0;

            for (int i = 0; i < settings.importTerrains.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PrefixLabel("Tile x" + tileX + "_y" + tileY);
                    settings.importTerrains[i] = EditorGUILayout.ObjectField(settings.importTerrains[i], typeof(TerrainData), false) as TerrainData;
                    if (GUILayout.Button("-", GUILayout.Width(25)))
                    {
                        settings.importTerrains.RemoveAt(i--);
                    }
                EditorGUILayout.EndHorizontal();
                if (++tileX >= settings.importTiles.x) { tileX = 0; ++tileY; }
            }

            GUILayout.Space(5);

            EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel("Tiles X");
                settings.importTiles.x = EditorGUILayout.IntField(settings.importTiles.x);
                if (settings.importTiles.x < 0) settings.importTiles.x = 0;
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel("Tiles Y");
                settings.importTiles.y = EditorGUILayout.IntField(settings.importTiles.y);
                if (settings.importTiles.y < 0) settings.importTiles.y = 0;
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(10);
            // TD.DrawSpacer();
        }
        
        void DrawExport(string filename, int mode)
        {
            TD.DrawSpacer();

            GUI.changed = false;

            TD.DrawLabelWidthUnderline("Export", 14);

            GUI.color = Color.blue * TD.editorSkinMulti;
            EditorGUILayout.BeginVertical("Box");
            GUI.color = Color.white;

            if (mode == 0)
            {
                EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PrefixLabel("Export Source");
                    settings.importSource = (TC_Settings.ImportSource)EditorGUILayout.EnumPopup(settings.importSource);
                EditorGUILayout.EndHorizontal();

                if (settings.importSource == TC_Settings.ImportSource.TerrainData_Files) DrawTerrainDataFiles();
            }
            
            
            EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel("Path");
                EditorGUILayout.LabelField(settings.exportPath);
                if (GUILayout.Button("Select", GUILayout.Width(50))) SelectExportPath();
            EditorGUILayout.EndHorizontal();

            string filenameText, buttonText;
            int buttonWidth;

            if (mode == 0) { filenameText = "Heightmap Filename"; buttonText = "Export Heightmap"; buttonWidth = 125; } else { filenameText = "Filename"; buttonText = "Export"; buttonWidth = 50; }

            filename = EditorGUILayout.TextField(filenameText, filename);

            if (mode == 0)
            {
                settings.combineHeightmapImage = EditorGUILayout.Toggle("Combine Image", settings.combineHeightmapImage);
            }
            else if (mode == 1 || mode == 2)
            {
                settings.imageExportFormat = (TC_Settings.ImageExportFormat)EditorGUILayout.EnumPopup("Image Format", settings.imageExportFormat);
            }

            if (GUI.changed)
            {
                EditorUtility.SetDirty(settings);
            }

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(" ");
            if (GUILayout.Button(buttonText, GUILayout.Width(buttonWidth)))
            {
                if (mode == 0)
                {
                    if (settings.combineHeightmapImage) generate.ExportHeightmapCombined(settings.exportPath); else generate.ExportHeightmap(settings.exportPath);
                }
                else if (mode == 1) generate.ExportSplatmap(settings.exportPath);
                else if (mode == 2) generate.ExportColormap(settings.exportPath, true);

            }
            if (mode == 0)
            {
                if (GUILayout.Button("Create Layer", GUILayout.Width(100)))
                {
                    generate.CreateLayerFromExportedHeightmap(settings.exportPath);
                }
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();

            if (mode == 0)
            {
                GUILayout.Space(10);
                GUI.color = Color.green * TD.editorSkinMulti;
                EditorGUILayout.BeginVertical("Box");
                GUI.color = Color.white;

                filename = EditorGUILayout.TextField("Normal map Filename", settings.normalmapFilename);
                settings.imageExportFormat = (TC_Settings.ImageExportFormat)EditorGUILayout.EnumPopup("Image Format", settings.imageExportFormat);
                settings.normalmapStrength = EditorGUILayout.FloatField("Normal map Strength", settings.normalmapStrength);

                EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PrefixLabel(" ");

                    if (GUILayout.Button("Export Normal map", GUILayout.Width(buttonWidth)))
                    {
                        generate.ExportNormalmap(settings.exportPath, true);
                    }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.EndVertical();
            }
            
        }

        public void SelectExportPath()
        {
            string path = settings.exportPath;

            if (path == "") path = Application.dataPath;

            string newPath = EditorUtility.SaveFolderPanel("Export path", path, "");

            if (newPath == "") return;

            settings.exportPath = newPath;
        }

        void AutoGenerate()
        {
            TC.AutoGenerate();
            DoRepaint();
        }

        void DrawGlobalScale()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(new GUIContent("Global Scale", "Green color if local Scale is global Scale , red if not."));
            if (item.t.lossyScale.x != item.t.localScale.x) GUI.backgroundColor = new Color(1f, 0.5f, 0.5f); else GUI.backgroundColor = Color.white;
            EditorGUILayout.LabelField("X " + item.t.lossyScale.x.ToString("F3"), EditorStyles.miniButtonMid, GUILayout.Width(60));
            if (item.t.lossyScale.y != item.t.localScale.y) GUI.backgroundColor = new Color(1f, 0.5f, 0.5f); else GUI.backgroundColor = Color.white;
            EditorGUILayout.LabelField("Y " + item.t.lossyScale.y.ToString("F3"), EditorStyles.miniButtonMid, GUILayout.Width(60));
            if (item.t.lossyScale.z != item.t.localScale.z) GUI.backgroundColor = new Color(1f, 0.5f, 0.5f); else GUI.backgroundColor = Color.white;
            EditorGUILayout.LabelField("Z " + item.t.lossyScale.z.ToString("F3"), EditorStyles.miniButtonMid, GUILayout.Width(60));

            GUI.backgroundColor = Color.white;

            EditorGUILayout.EndHorizontal();

        }

        static public void CheckKeyLockOnSelection(Event eventCurrent)
        {
            // Debug.Log(EditorWindow.focusedWindow.titleContent.text); 
            if (EditorWindow.focusedWindow == null) return;
            if (EditorWindow.focusedWindow.titleContent.text != "Scene") return;
            for (int i = 0; i < Selection.transforms.Length; i++)
            {
                TC_ItemBehaviour item = Selection.transforms[i].GetComponent<TC_ItemBehaviour>();

                if (item != null)
                {
                    if (item.GetType() != typeof(TC_SelectItem) && item.GetType() != typeof(TC_SelectItemGroup))
                    {
                        if (item.GetType() != typeof(TC_Node))
                        {
                            if (eventCurrent.control)
                            {
                                if (!item.lockPosChildren) item.controlDown = true;
                                if (!item.lockPosChildren)
                                {
                                    item.lockPosChildren = true;
                                    item.SetLockChildrenPosition(false);
                                }
                            }
                            else
                            {
                                if (item.lockPosChildren && item.controlDown)
                                {
                                    item.controlDown = false;
                                    item.lockPosChildren = false;
                                    item.SetLockChildrenPosition(false);
                                }
                            }
                        }

                        if (eventCurrent.keyCode == KeyCode.L && eventCurrent.type == EventType.KeyDown)
                        {
                            if (item.GetType() != typeof(TC_Node))
                            {
                                item.lockPosChildren = !item.lockPosChildren;
                                item.SetLockChildrenPosition(false);
                            }
                            else item.lockTransform = !item.lockTransform;

                            // EditorUtility.SetDirty(item);
                        }
                    }
                }
            }
        }

        void DrawMethod()
        {
            if (item.level > 1 && nodeGroup == null && selectItem == null && selectItemGroup == null)
            {
                if (item.t.parent != null)
                {
                    if (item.t.GetSiblingIndex() != item.t.parent.childCount - 1)
                    {
                        TD.DrawSpacer();

                        if (node != null)
                        {
                            if (node.inputKind != InputKind.Current) TD.DrawProperty(method);
                        }
                        else
                        {
                            if (item.outputId != TC.treeOutput && item.outputId != TC.objectOutput) TD.DrawProperty(method);
                            else
                            {
                                MethodItem m = (MethodItem)method.enumValueIndex;
                                m = (MethodItem)EditorGUILayout.EnumPopup(new GUIContent("Method"), m);
                                method.enumValueIndex = (int)m;
                            }
                        }

                        if (((Method)method.enumValueIndex) == Method.Lerp) DrawSlider(overlay, 0, 1);
                    }
                }
            }
        }

        public void DrawPosOffset()
        {
            EditorGUILayout.BeginVertical("Box");
            // -------
            item.t.localPosition = EditorGUILayout.Vector3Field("Position", item.t.localPosition);

            float rotY = item.t.localEulerAngles.y;
            GUI.changed = false;
            rotY = EditorGUILayout.Slider("Rotation", rotY, -180, 180);
            if (GUI.changed) item.t.localEulerAngles = new Vector3(0, rotY, 0);

            item.t.localScale = EditorGUILayout.Vector3Field("Scale", item.t.localScale);
            GUILayout.Space(5);
            TD.DrawProperty(posOffset);
            TD.DrawProperty(positionMode);
            // -------
            EditorGUILayout.EndVertical();
        }

        public string[] FindPresets(string path)
        {
            return AssetDatabase.FindAssets("t:Prefab", new string[] { path });
        }

        public void LoadPresetMenu(string path)
        {
            string[] files = FindPresets(path);

            GenericMenu menu = new GenericMenu();
            
            for (int i = 0; i < files.Length; i++)
            {
                string filename = TC.GetFileName(AssetDatabase.GUIDToAssetPath(files[i]));
                menu.AddItem(new GUIContent(filename), false, PresetMenu, files[i]);
            }

            menu.ShowAsContext();
        }

        void PresetMenu(object obj)
        {
            string cmd = obj.ToString();

            string path = AssetDatabase.GUIDToAssetPath(cmd);

            if (path != "")
            {

                GameObject prefab = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;
                if (prefab != null)
                {
                    GameObject go = Instantiate(prefab) as GameObject;

                    if (go != null)
                    {
                        TC_Node node = go.GetComponent<TC_Node>();
                        Undo.RegisterCreatedObjectUndo(go, "Created " + go.name);

                        node.method = this.node.method;

                        if (node != null)
                        {
                            go.transform.parent = item.t.parent;
                            go.transform.SetSiblingIndex(item.t.GetSiblingIndex());
                            Undo.DestroyObjectImmediate(item.gameObject);
                            Selection.activeTransform = go.transform;
                        }
                    }
                }
            }
        }

        //void SavePreset()
        //{
        //    // GenericMenu menu = new GenericMenu();

        //    // menu.AddItem(new GUIContent("Save..."), false, SavePresetMenu, "Save");
        //    // menu.AddItem(new GUIContent("Save && Make Default"), false, SavePresetMenu, "SaveDefault");

        //    // menu.ShowAsContext();
        //}

        string GetPresetFullPath()
        {
            return TC.GetApplicationPath() + GetPresetPathFile();
        }

        string GetPresetPath()
        {
            string path = TC.installPath + "/Examples/Presets/Nodes/";

            path += ((InputKind)inputKind.enumValueIndex).ToString() + "/";

            if (inputKind.enumValueIndex == (int)InputKind.Noise) path += ((InputNoise)inputNoise.enumValueIndex).ToString();
            if (inputKind.enumValueIndex == (int)InputKind.Shape) path += ((InputShape)inputShape.enumValueIndex).ToString();
            
            return path;
        }

        string GetPresetPathFile()
        {
            return GetPresetPath() + "/" + item.name + ".prefab";
        }


        void SavePreset()
        {
            string path = GetPresetPathFile();
            string fullPath = GetPresetFullPath();
            
            // Debug.Log(path);
            // Debug.Log(fullPath);

            bool create = false;

            if (TC.FileExists(fullPath))
            {
                if (EditorUtility.DisplayDialog("Overwrite " + item.name + " Preset", "Preset " + item.name + " already exists, do you want to overwrite it?", "Yes", "Cancel")) create = true;
            }
            else create = true;

            if (create)
            {
                TD.CreatePrefab(path, item.gameObject);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }

        public void SelectProjectPathFirstPreset(string path)
        {
            string[] files = FindPresets(path);

            if (files.Length > 0)
            {
                path = AssetDatabase.GUIDToAssetPath(files[0]);
                SelectProjectPath(path);
            }
        }

        public void SelectProjectPath(string path)
        {
            Object obj = AssetDatabase.LoadAssetAtPath(path, typeof(Object));
            Selection.activeObject = obj;
            EditorGUIUtility.PingObject(obj);
            Selection.activeTransform = item.t;
        }

        public void DrawPreset()
        {
            string path = GetPresetPath();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Preset");
            if (GUILayout.Button("Load", EditorStyles.miniButtonMid, GUILayout.Width(50))) { LoadPresetMenu(path); }
            if (GUILayout.Button("Save", EditorStyles.miniButtonMid, GUILayout.Width(50))) { SavePreset(); }

            if (GUILayout.Button("Show", EditorStyles.miniButtonMid, GUILayout.Width(50)))
            {
                string fullPath = GetPresetFullPath();
                // Debug.Log(fullPath);
                if (TC.FileExists(fullPath)) SelectProjectPath(GetPresetPathFile());
                else
                {
                    SelectProjectPathFirstPreset(path);
                    Selection.activeTransform = item.t;
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        public void DrawNoise()
        {
            TD.DrawProperty(inputNoise);
            // if (GUI.changed) node.Init();
            DrawPreset();
            GUILayout.Space(5);

            if (inputNoise.enumValueIndex == (int)InputNoise.Cell) TD.DrawProperty(cellNoiseMode);
            else
            {
                if (inputNoise.enumValueIndex == (int)InputNoise.IQ || inputNoise.enumValueIndex == (int)InputNoise.Swiss || inputNoise.enumValueIndex == (int)InputNoise.Jordan)
                {
                    if (noiseMode.enumValueIndex < 1) noiseMode.enumValueIndex = 1;
                    NoiseMode2 popup = (NoiseMode2)noiseMode.enumValueIndex;
                    GUI.changed = false;
                    popup = (NoiseMode2)EditorGUILayout.EnumPopup("Noise Mode", popup);
                    if (GUI.changed)
                    {
                        AutoGenerate();
                    }
                    noiseMode.enumValueIndex = (int)popup;
                }
                else TD.DrawProperty(noiseMode);
            }

            if (inputNoise.enumValueIndex != (int)InputNoise.Random)
            {
                // TD.DrawProperty(frequency);
                TD.DrawProperty(lacunarity);
                TD.DrawProperty(persistence);
                TD.DrawProperty(octaves);

                if (GUI.changed)
                {
                    if (octaves.intValue < 1) octaves.intValue = 1;
                    else if (octaves.intValue > 12) octaves.intValue = 12;
                }
                TD.DrawProperty(seed);
                
                GUILayout.Space(15);

                if (inputNoise.enumValueIndex == (int)InputNoise.Swiss || inputNoise.enumValueIndex == (int)InputNoise.Jordan)
                {
                    TD.DrawProperty(amplitude, new GUIContent("Radius"));
                    if (amplitude.floatValue < 1) amplitude.floatValue = 1;

                    TD.DrawProperty(warp0);
                    TD.DrawProperty(warp);

                    if (inputNoise.enumValueIndex == (int)InputNoise.Jordan)
                    {

                        TD.DrawProperty(damp0);
                        TD.DrawProperty(damp);
                        TD.DrawProperty(dampScale);
                    }
                }

                if (inputNoise.enumValueIndex == (int)InputNoise.Cell && cellNoiseMode.enumValueIndex == (int)CellNoiseMode.Normal)
                {
                    DrawIntSlider(cellType, 1, 9);
                    DrawIntSlider(distanceFunction, 1, 7);
                }
            }
        }

        public void DrawCommingSoon(string text)
        {
            GUILayout.Space(5);
            TD.DrawLabel(text, 14);
            GUILayout.Space(5);
        }

        public void DrawCurve(SPCurve spCurve, string label)
        {
            GUI.color = Color.blue * TD.editorSkinMulti;
            EditorGUILayout.BeginVertical("Box");
            GUI.color = Color.white;
            // -------

            TD.DrawProperty(spCurve.active);
            // GUILayout.Space(3);
            bool active = spCurve.active.boolValue;

            if (!active) GUI.color = new Color(1, 1, 1, 0.35f);

            float multi = 1;

            if (node != null)
            {
                if (node.inputKind == InputKind.Terrain)
                {
                    if (node.inputTerrain == InputTerrain.Height) multi = 1;
                    else if (node.inputTerrain == InputTerrain.Angle) multi = 90;
                }
            }

            spCurve.range.vector2Value *= multi;

            TD.DrawProperty(spCurve.range, new GUIContent("Curve Range"));
            if (spCurve.range.vector2Value.x < 0) spCurve.range.vector2Value = new Vector2(0, spCurve.range.vector2Value.y);
            else if (spCurve.range.vector2Value.x > multi - 0.001f) spCurve.range.vector2Value = new Vector2(multi - 0.001f, spCurve.range.vector2Value.y);

            if (spCurve.range.vector2Value.y < 0.001f) spCurve.range.vector2Value = new Vector2(spCurve.range.vector2Value.x, 0.001f);
            else if (spCurve.range.vector2Value.y > multi) spCurve.range.vector2Value = new Vector2(spCurve.range.vector2Value.x, multi);

            spCurve.range.vector2Value /= multi;

            DrawMinMaxSlider(spCurve.range, 0, 1, 0.001f, new GUIContent(" "));
            
            // GUILayout.Space(3);

            EditorGUILayout.BeginHorizontal();
            TD.DrawProperty(spCurve.curve, new GUIContent(label));
            //if (GUILayout.Button("C", GUILayout.Width(25)))
            //{
            //    AnimationCurve curve = spCurve.curve.animationCurveValue;

            //    for (int i = 0; i < curve.keys.Length; i++)
            //    {
            //        Keyframe key = curve.keys[i];
            //        Debug.Log(key.time + ", " + key.value + ", " + key.inTangent + ", " + key.outTangent);
            //    }
            //}

            if (GUILayout.Button("Options", EditorStyles.miniButtonMid, GUILayout.Width(50)))
            {
                clickedCurve = spCurve;
                ClickCurveMenu();
            }
            EditorGUILayout.EndHorizontal();

            GUI.color = Color.white;
            // TD.DrawProperty(spCurve.type);

            // -------
            EditorGUILayout.EndVertical();
        }

        public SPCurve clickedCurve;

        void ClickCurveMenu()
        {
            GenericMenu menu = new GenericMenu();
            
            menu.AddItem(new GUIContent("Invert"), false, CurveMenuInput, "invert");
            menu.AddSeparator("");
            menu.AddItem(new GUIContent("Linear Curve"), false, CurveMenuInput, "linearCurve");
            menu.AddItem(new GUIContent("Linear Bell Curve"), false, CurveMenuInput, "linearBellCurve");
            menu.AddItem(new GUIContent("Smooth Bell Curve"), false, CurveMenuInput, "smoothBellCurve");

            menu.ShowAsContext();
        }

        void CurveMenuInput(object obj)
        {
            string cmd = obj.ToString();

            if (cmd == "invert") clickedCurve.curve.animationCurveValue = Mathw.InvertCurve(clickedCurve.curve.animationCurveValue);
            else if (cmd == "linearCurve") clickedCurve.curve.animationCurveValue = AnimationCurve.Linear(0, 0, 1, 1);
            else if (cmd == "linearBellCurve")
            {
                Keyframe[] frames = new Keyframe[3];
                frames[0] = new Keyframe(0, 0, 0, 2);
                frames[1] = new Keyframe(0.5f, 1, 2, -2);
                frames[2] = new Keyframe(1, 0, -2, 0);

                clickedCurve.curve.animationCurveValue = new AnimationCurve(frames);
            }
            else if (cmd == "smoothBellCurve")
            {
                clickedCurve.curve.animationCurveValue = new AnimationCurve(new Keyframe[] { new Keyframe(0, 0), new Keyframe(0.5f, 1), new Keyframe(1, 0) });
            }

            serializedObject.ApplyModifiedProperties();

            AutoGenerate();
        }

        public void DrawShape()
        {
            TD.DrawProperty(inputShape);
            DrawPreset();
            GUILayout.Space(5);

            if (shapeSize == null) TC_Reporter.Log("Shape Size = null");
            if (node.inputShape == InputShape.Circle) TD.DrawProperty(shapeSize, new GUIContent("Diameter"));
            else if (node.inputShape == InputShape.Gradient) TD.DrawProperty(shapeSize, new GUIContent("Size"));
            else if (node.inputShape == InputShape.Rectangle)
            {
                GUI.changed = false;
                EditorGUILayout.PropertyField(topSize);
                EditorGUILayout.PropertyField(bottomSize);
                if (GUI.changed)
                {
                    topSize.vector2Value = new Vector2(topSize.vector2Value.x, topSize.vector2Value.x * (bottomSize.vector2Value.y / bottomSize.vector2Value.x));
                    AutoGenerate();
                }
            }
        }

        public void DrawImageSettings()
        {
            // EditorGUILayout.PrefixLabel(" ");
            // EditorGUILayout.LabelField("Clamp", GUILayout.Width(40));

            GUI.color = Color.red * TD.editorSkinMulti;
            EditorGUILayout.BeginVertical("Box");
            GUI.color = Color.white;

            TD.DrawProperty(isLocked);
            if (GUI.changed)
            {
                item.Lock(isLocked.boolValue);
            }

            TD.DrawProperty(lockTransform);

            if (lockTransform.boolValue)
            {
                EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PrefixLabel(" Position");
                    if (lockPosX.boolValue) GUI.backgroundColor = Color.green;
                    if (GUILayout.Button("X", EditorStyles.miniButtonMid, GUILayout.Width(25))) lockPosX.boolValue = !lockPosX.boolValue;
                    GUI.backgroundColor = lockPosY.boolValue ? Color.green : Color.white;
                    if (GUILayout.Button("Y", EditorStyles.miniButtonMid, GUILayout.Width(25))) lockPosY.boolValue = !lockPosY.boolValue;
                    GUI.backgroundColor = lockPosZ.boolValue ? Color.green : Color.white;
                    if (GUILayout.Button("Z", EditorStyles.miniButtonMid, GUILayout.Width(25))) lockPosZ.boolValue = !lockPosZ.boolValue;
                    GUI.backgroundColor = Color.white;
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PrefixLabel(" Rotation");
                    GUI.backgroundColor = lockRotY.boolValue ? Color.green : Color.white;
                    GUILayout.Space(25);
                    if (GUILayout.Button("Y", EditorStyles.miniButtonMid, GUILayout.Width(25))) lockRotY.boolValue = !lockRotY.boolValue;
                    GUI.backgroundColor = Color.white;
                EditorGUILayout.EndHorizontal();

                
                EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PrefixLabel(" Scale");
                    if (lockScaleX.boolValue) GUI.backgroundColor = Color.green;
                    if (GUILayout.Button("X", EditorStyles.miniButtonMid, GUILayout.Width(25))) lockScaleX.boolValue = !lockScaleX.boolValue;
                    GUI.backgroundColor = lockScaleY.boolValue ? Color.green : Color.white;
                    if (GUILayout.Button("Y", EditorStyles.miniButtonMid, GUILayout.Width(25))) lockScaleY.boolValue = !lockScaleY.boolValue;
                    GUI.backgroundColor = lockScaleZ.boolValue ? Color.green : Color.white;
                    if (GUILayout.Button("Z", EditorStyles.miniButtonMid, GUILayout.Width(25))) lockScaleZ.boolValue = !lockScaleZ.boolValue;
                    GUI.backgroundColor = Color.white;
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndVertical();
        }

        public void DrawFlipScale(bool drawBox)
        {
            if (drawBox)
            {
                GUI.color = Color.red * TD.editorSkinMulti;
                EditorGUILayout.BeginVertical("Box");
                GUI.color = Color.white;
            }

            EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel("Flip Scale");
                if (item.t.localScale.x < 0) GUI.backgroundColor = Color.green;
                if (GUILayout.Button("X", EditorStyles.miniButtonMid, GUILayout.Width(25))) SetTransformScale(new Vector3(-item.t.localScale.x, item.t.localScale.y, item.t.localScale.z), "Flip X");
                GUI.backgroundColor = item.t.localScale.y < 0 ? Color.green : Color.white;
                if (GUILayout.Button("Y", EditorStyles.miniButtonMid, GUILayout.Width(25))) SetTransformScale(new Vector3(item.t.localScale.x, -item.t.localScale.y, item.t.localScale.z), "Flip Y");
                GUI.backgroundColor = item.t.localScale.z < 0 ? Color.green : Color.white;
                if (GUILayout.Button("Z", EditorStyles.miniButtonMid, GUILayout.Width(25))) SetTransformScale(new Vector3(item.t.localScale.x, item.t.localScale.y, -item.t.localScale.z), "Flip Z");
                GUI.backgroundColor = Color.white;
            EditorGUILayout.EndHorizontal();

            if (drawBox) EditorGUILayout.EndVertical();
        }

        public void SetTransformScale(Vector3 scale, string undoName)
        {
            Undo.RecordObject(item.t, undoName);
            item.t.localScale = scale;
        }

        public void DrawRawImage()
        {
            if (settings.presetMode == PresetMode.StampMode)
            {
                GUI.changed = false;
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel("Stamp Texture");
                stampTex.objectReferenceValue = (Texture)EditorGUILayout.ObjectField((Texture)stampTex.objectReferenceValue, typeof(Texture), false, GUILayout.Width(75), GUILayout.Height(75));
                if (stampTex.objectReferenceValue != null)
                {
                    EditorGUILayout.LabelField(stampTex.objectReferenceValue.name);
                }

                if (GUI.changed)
                {
                    TC.RefreshOutputReferences(item.outputId);
                    // Debug.Log("GUI changed");
                    Texture tex = (Texture)stampTex.objectReferenceValue;
                    if (tex == null)
                    {
                        // Debug.Log("tex = null");
                        if (rawImage.objectReferenceValue != null)
                        {
                            UnregisterRawFile();
                            stampTex.objectReferenceValue = null;
                            AutoGenerate();
                            DoRepaint();
                        }
                    }
                    else if (!node.DropTextureEditor(tex))
                    {
                        if (rawImage.objectReferenceValue != null) UnregisterRawFile();
                        stampTex.objectReferenceValue = null;
                        DoRepaint();
                    }
                    else AutoGenerate();
                    // Debug.Log(node.rawImageIndex);
                }
                EditorGUILayout.EndHorizontal();

                // DrawIntSlider(mipmapLevel, 0, 11, new GUIContent("Blur"));
            }
        }

        public void UnregisterRawFile()
        {
            if (rawImage.objectReferenceValue != null)
            {
                // Debug.Log("Unregister");
                node.rawImage.UnregisterReference();
                
            }
            pathTexStamp.stringValue = "";
            // Debug.Log("Do " + node.pathTexStamp);
            TC.RefreshOutputReferences(node.outputId);
        }

        public void DrawImage()
        {
            image = serializedObject.FindProperty("image");

            if (image != null)
            {
                // texImage = image.FindPropertyRelative("tex");
                // rtImage = image.FindPropertyRelative("rt");
            }

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Texture");
            GUI.changed = false;
            stampTex.objectReferenceValue = (Texture)EditorGUILayout.ObjectField((Texture)stampTex.objectReferenceValue, typeof(Texture), false, GUILayout.Width(75), GUILayout.Height(75));
            if (GUI.changed)
            {
                TC.RefreshOutputReferences(node.outputId);
                AutoGenerate();
            }
            EditorGUILayout.EndHorizontal();

            imageSettings = serializedObject.FindProperty("imageSettings");

            if (imageSettings != null)
            {
                colChannels = imageSettings.FindPropertyRelative("colChannels");
                colSelectMode = imageSettings.FindPropertyRelative("colSelectMode");
            }

            TD.DrawProperty(colSelectMode, new GUIContent("Color Select Mode"));

            GUILayout.Space(5);

            EditorGUILayout.BeginVertical("Box");
            
            Color colStart = Color.black, colEnd = Color.black;
            Color colStartOld = colStart, colEndOld = colEnd;
            
            for (int i = 0; i < 4; i++) {
                if (node.imageSettings.colChannels[i].active)
                {
                    colStartOld[i] = node.imageSettings.colChannels[i].range.x / 255.0f;
                    colEndOld[i] = node.imageSettings.colChannels[i].range.y / 255.0f;
                }
            }

            EditorGUILayout.BeginHorizontal();
            colStart = EditorGUILayout.ColorField(colStartOld);
            if (colStart != colStartOld)
            {
                for (int i = 0; i < 4; i++)
                {
                    if (node.imageSettings.colChannels[i].active) node.imageSettings.colChannels[i].range.x = colStart[i] * 255;
                }
            }
            if (colSelectMode.enumValueIndex == (int)ColorSelectMode.ColorRange)
            {
                colEnd = EditorGUILayout.ColorField(colEndOld);
                if (colEnd != colEndOld)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        if (node.imageSettings.colChannels[i].active) node.imageSettings.colChannels[i].range.y = colEnd[i] * 255;
                    }
                }
            }
            EditorGUILayout.EndHorizontal();
            
            for (int i = 0; i < 4; i++)
            {
                SerializedProperty colChannel = colChannels.GetArrayElementAtIndex(i);
                SerializedProperty active = colChannel.FindPropertyRelative("active");
                SerializedProperty range = colChannel.FindPropertyRelative("range");

                EditorGUILayout.BeginHorizontal();
                    GUI.color = TC.colChannel[i];
                    if (!active.boolValue) GUI.color = new Color(1, 1, 1, 0.35f);
                    EditorGUILayout.LabelField(TC.colChannelNames[i], GUILayout.Width(50));
                    TD.DrawProperty(active, new GUIContent(""), 25);
                    if (active.boolValue) GUI.color = Color.white;
                    
                    if (colSelectMode.enumValueIndex == (int)ColorSelectMode.Color)
                    {
                        int rangeX = (int)range.vector2Value.x;
                        rangeX = EditorGUILayout.IntSlider(rangeX, 0, 255);
                        if (rangeX != range.vector2Value.x) AutoGenerate();
                        range.vector2Value = new Vector2(rangeX, range.vector2Value.y);
                    }
                    else
                    {
                        GUI.changed = false;
                        range.vector2Value = new Vector2(EditorGUILayout.IntField((int)range.vector2Value.x, GUILayout.Width(30)), range.vector2Value.y);
                        if (GUI.changed) AutoGenerate();
                        DrawMinMaxSlider(range, 0, 255, 0f, new GUIContent(""));
                        GUI.changed = false;
                        range.vector2Value = new Vector2(range.vector2Value.x, EditorGUILayout.IntField((int)range.vector2Value.y, GUILayout.Width(30)));
                        if (GUI.changed) AutoGenerate();
                    }

                    if (GUILayout.Button("R", GUILayout.Width(25))) { range.vector2Value = new Vector2(0, 255); AutoGenerate(); }
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndVertical();
        }

        void DrawSelectItemGroup()
        {
            if (selectItemGroup.itemList.Count == 0) return;

            EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Mixing", GUILayout.Width(50));
                DrawSlider(mix, 0, 1.5f, new GUIContent("","Mixes " + TC.outputNames[item.outputId] + " Items to have more overlap\n 0 = no overlap, 1.5 = max overlap."));
            
                if (GUI.changed)
                {
                    serializedObject.ApplyModifiedProperties();
                    selectItemGroup.CalcPreview();
                    DoRepaint();
                }
            EditorGUILayout.EndHorizontal();
            
            if (GUILayout.Button("Reset Sliders")) selectItemGroup.ResetRanges();

            GUI.color = TD.editorSkinMulti != 1 ? new Color(0.1f, 0.0f, 0.0f, 0.5f) : Color.red;
            EditorGUILayout.BeginVertical("Box");
            GUI.color = Color.white;

            for (int i = 0; i < selectItemGroup.itemList.Count; i++)
            {
                TC_SelectItem selectItem1 = selectItemGroup.itemList[i];

                EditorGUILayout.BeginHorizontal();
                if (selectItemGroup.outputId != TC.colorOutput)
                {
                    DrawPreviewTexture(selectItem1.preview.tex, (TD.hoverItem == selectItem1 ? Color.green : Color.white) * (selectItem1.active ? 1 : 0.75f), Color.white);
                }
                else DrawPreviewTexture(Texture2D.whiteTexture, Color.white, selectItem1.color);

                DrawRangeSlider(selectItem1, true);
                EditorGUILayout.EndHorizontal();
            }
            
            EditorGUILayout.EndVertical();

            if (selectItemGroup.outputId == TC.treeOutput || selectItemGroup.outputId == TC.objectOutput)
            {
                TD.DrawSpacer();

                GUI.color = Color.blue * TD.editorSkinMulti;
                EditorGUILayout.BeginVertical("Box");
                GUI.color = Color.white;

                TD.DrawLabelWidthUnderline("Scale", 14);
                
                DrawVector2(scaleMinMaxMulti, true, "Min", "Max", new GUIContent("Scale Range Multiplier"));
                
                GUI.changed = false;
                TD.DrawProperty(scaleMulti, new GUIContent("Scale Multiplier")); 
                if (GUI.changed)
                {
                    if (scaleMulti.floatValue < 0.01f) scaleMulti.floatValue = 0.01f;
                }

                EditorGUILayout.BeginHorizontal();
                    TD.DrawProperty(linkScaleToMask);
                    if (linkScaleToMask.boolValue) DrawSlider(linkScaleToMaskAmount, 0, 1, new GUIContent(""));
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.EndVertical();
            }
        }

        static public void DrawPreviewTexture(Texture2D tex, Color color, Color colTex, int width = 50, int height = 50)
        {
            GUI.backgroundColor = color;
            if (GUILayout.Button("", GUILayout.Width(width), GUILayout.Height(height))) { }
            GUI.backgroundColor = Color.white;

            Rect rect = GUILayoutUtility.GetLastRect();
            rect.x += 4;
            rect.y += 4;
            rect.width -= 8;
            rect.height -= 8;
            
            if (tex != null)
            {
                GUI.color = colTex;
                EditorGUI.DrawPreviewTexture(rect, tex);
                GUI.color = Color.white;
            }
        }

        void DrawTerrainHeightSlider(Rect rect)
        {
            if (!settings.hasMasterTerrain) return;

            TC_Area2D area2D = TC_Area2D.instance;
            if (area2D.currentTerrainArea == null) area2D.currentTerrainArea = area2D.terrainAreas[0];

            Vector3 size = area2D.currentTerrainArea.terrainSize;

            GUI.changed = false;
            size.y = (int)GUI.VerticalSlider(rect, size.y, area2D.terrainHeightRange.y, area2D.terrainHeightRange.x);

            EditorGUILayout.BeginHorizontal();
            size.y = EditorGUILayout.FloatField("Terrain Height", size.y);
            if (GUI.changed)
            {
                if (size.y < 1) size.y = 1;
                Undo.RecordObject(area2D.currentTerrainArea, "Height Slider");
                area2D.currentTerrainArea.terrainSize = size;
            }
            if (area2D.currentTCUnityTerrain != null)
            {
                if (area2D.currentTCUnityTerrain.CheckValidUnityTerrain())
                {
                    if (area2D.currentTerrainArea.terrainSize.y != area2D.currentTCUnityTerrain.terrain.terrainData.size.y) GUI.backgroundColor = Color.red;
                }
            }
            if (GUILayout.Button("Refresh", GUILayout.Width(60))) ApplyTerrainHeight(size);
            GUI.backgroundColor = Color.white;
            
            EditorGUILayout.EndHorizontal();
        }

        void ApplyTerrainHeight(Vector3 size)
        {
            TC_Area2D area2D = TC_Area2D.instance;

            area2D.currentTerrainArea.ApplySize();
            DoRepaint();
            generate.Generate(true);
        }

        void DrawSelectItem()
        {
            if (!settings.hasMasterTerrain) return;

            int length = selectItem.GetItemTotalFromTerrain();

            selectIndex.intValue++;
            DrawIntSlider(selectIndex, 1, length, new GUIContent(TC.outputNames[selectItem.outputId] + " Texture"));
            // if (GUI.changed) serializedObject.ApplyModifiedProperties();
            selectIndex.intValue--;

            if (GUI.changed)
            {
                serializedObject.ApplyModifiedProperties();
                selectItem.Refresh();
            }

            if (selectItem.outputId == TC.splatOutput)
            {
                //TD.DrawProperty(splatCustom);
                //if (GUI.changed)
                //{
                //    serializedObject.ApplyModifiedProperties();
                //    selectItem.Refresh();
                //}
                
                //if (splatCustom.boolValue)
                //{
                //    if (splatCustomValues.arraySize != length) splatCustomValues.arraySize = length;

                //    EditorGUILayout.BeginVertical("Box");
                //    for (int i = 0; i < splatCustomValues.arraySize; i++)
                //    {
                //        EditorGUILayout.BeginHorizontal();
                //        GUILayout.Space(15);
                //        DrawPreviewTexture(settings.masterTerrain.terrainData.splatPrototypes[i].texture, Color.white, Color.white);
                //        GUILayout.Space(5);
                //        DrawSlider(splatCustomValues.GetArrayElementAtIndex(i), 0, 1, new GUIContent(""));
                //        if (GUI.changed)
                //        {
                //            selectItem.parentItem.CreateSplatMixBuffer();
                //        }
                //        EditorGUILayout.EndHorizontal();
                //    }
                //    GUILayout.Space(5);
                //    EditorGUILayout.EndVertical();
                //}
            }
        }

        void DrawColorSelectItem()
        {
            // TD.DrawProperty(color);
            if (selectItem.parentItem.itemList.Count == 1)
            {
                if (texColor.objectReferenceValue != null)
                {
                    DrawCustomTransform();

                    GUI.color = Color.blue;
                    EditorGUILayout.BeginVertical("Box");
                    GUI.color = Color.white;

                    TD.DrawProperty(brightness);
                    TD.DrawProperty(saturation);
                    if (saturation.floatValue < 0) saturation.floatValue = 0;
                }

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel("Texture");
                GUI.changed = false;
                texColor.objectReferenceValue = (Texture)EditorGUILayout.ObjectField((Texture)texColor.objectReferenceValue, typeof(Texture), false, GUILayout.Width(75), GUILayout.Height(75));
                if (GUI.changed) AutoGenerate();
                EditorGUILayout.EndHorizontal();
            }
        }

        void DrawLayerGroupOutput()
        {
            TC_TerrainLayer terrainLayer = layerGroup.parentItem as TC_TerrainLayer;

            if (terrainLayer != null)
            {
                Undo.RecordObject(terrainLayer, "Resolution");
                GUI.changed = false;

                EditorGUILayout.BeginHorizontal();
                if (layerGroup.outputId == TC.treeOutput)
                {
                    terrainLayer.treeResolutionPM = EditorGUILayout.FloatField(new GUIContent("Tree Resolution Per Meter"), terrainLayer.treeResolutionPM);
                    if (terrainLayer.treeResolutionPM > 89) terrainLayer.treeResolutionPM = 89;
                }
                else if (layerGroup.outputId == TC.objectOutput)
                {
                    terrainLayer.objectResolutionPM = EditorGUILayout.FloatField(new GUIContent("Object Resolution Per Meter"), terrainLayer.objectResolutionPM);
                    if (terrainLayer.objectResolutionPM > 89) terrainLayer.objectResolutionPM = 89;
                }
                else if (layerGroup.outputId == TC.colorOutput) terrainLayer.colormapResolution = EditorGUILayout.IntField(new GUIContent("Colormap Resolution"), terrainLayer.colormapResolution);

                if (layerGroup.active)
                {
                    if (GUILayout.Button("Refresh", GUILayout.Width(60))) AutoGenerate();
                }
                EditorGUILayout.EndHorizontal();

                if (GUI.changed) EditorUtility.SetDirty(terrainLayer);
                
                TD.DrawSpacer();
            }
        }
        
        void DrawTreeSelectItem()
        {
            if (!settings.hasMasterTerrain) return;

            int treeLength = settings.masterTerrain.terrainData.treePrototypes.Length;

            DrawIntSlider(selectIndex, 0, treeLength - 1, new GUIContent("Tree Index"));
            if (GUI.changed) selectItem.Refresh();

            TD.DrawSpacer();

            GUI.color = Color.red * TD.editorSkinMulti;
            EditorGUILayout.BeginVertical("Box");
                GUI.color = Color.white;
                TD.DrawLabelWidthUnderline("Position", 14);
                DrawSlider(randomPosition, 0, 1, new GUIContent("Random Position"));
                if (GUI.changed)
                {
                    serializedObject.ApplyModifiedProperties();
                    selectItem.parentItem.CreateMixBuffer();
                }
            // TD.DrawProperty(heightOffset);
            EditorGUILayout.EndVertical();

            TD.DrawSpacer();

            DrawScale(false);
        }

        void DrawScale(bool isObjectItem)
        {
            GUI.color = Color.blue * TD.editorSkinMulti;
            EditorGUILayout.BeginVertical("Box");
            GUI.color = Color.white;

            TD.DrawLabelWidthUnderline("Scale", 14);

            if (isObjectItem)
            {
                TD.DrawProperty(customScaleRange, new GUIContent("Custom Scale"));
            }

            if (isObjectItem)
            {
                if (customScaleRange.boolValue)
                {
                    DrawVector2(scaleRangeX, true, "Min", "Max", new GUIContent("Scale Range X"));
                    DrawVector2(scaleRangeY, true, "Min", "Max", new GUIContent("Scale Range Y"));
                    DrawVector2(scaleRangeZ, true, "Min", "Max", new GUIContent("Scale Range Z"));
                }
                else
                {
                    DrawVector2(scaleRange, true, "Min", "Max", new GUIContent("Scale Range"));
                    DrawSlider(nonUniformScale, 0, 1);
                }
            }
            else
            {
                DrawVector2(scaleRange, true, "Min", "Max", new GUIContent("Scale Range"));
                DrawSlider(nonUniformScale, 0, 1);
            }

            GUI.changed = false;
            TD.DrawProperty(scaleMulti);
            if (GUI.changed)
            {
                if (scaleMulti.floatValue < 0.01f) scaleMulti.floatValue = 0.01f;
            }
            
            TD.DrawProperty(scaleCurve);

            TC_SelectItemGroup selectableItemGroup = selectItem.parentItem;
            if (selectableItemGroup != null)
            {
                if (selectableItemGroup.itemList.Count == 1)
                {
                    Undo.RecordObject(selectableItemGroup, "Link Scale To Mask");
                    GUI.changed = false;
                    selectableItemGroup.linkScaleToMask = EditorGUILayout.Toggle("Link Scale To Mask", selectableItemGroup.linkScaleToMask);
                    if (GUI.changed) AutoGenerate();
                }
            }
            else Repaint();

            GUILayout.Space(5);

            EditorGUILayout.EndVertical();
        }
        

        void DrawObjectSelectItem()
        {
            EditorGUILayout.BeginVertical("Box");

            TD.DrawProperty(go, new GUIContent("Spawn Object"));
            if (GUI.changed)
            {
                // selectItem.spawnObject.go = (GameObject)go.objectReferenceValue;
                serializedObject.ApplyModifiedProperties();
                selectItem.Refresh();
                DoRepaint();
            }

            Color color = Color.white;
            
            TD.DrawProperty(parentMode);
            if (GUI.changed) TC.RefreshOutputReferences(TC.objectOutput);

            if (parentMode.enumValueIndex == (int)TC_SelectItem.SpawnObject.ParentMode.Create)
            {
                TD.DrawProperty(parentName);
                if (GUI.changed) TC.RefreshOutputReferences(TC.objectOutput);
                TD.DrawProperty(parentToTerrain);
            }
            else if (parentMode.enumValueIndex == (int)TC_SelectItem.SpawnObject.ParentMode.Existing)
            {
                TD.DrawProperty(parentT);
                if (GUI.changed) TC.RefreshOutputReferences(TC.objectOutput);
            }

            if (linkToPrefab.boolValue == true)
            {
                if (go.objectReferenceValue != null)
                {
                    if (!TD.IsPrefab(go.objectReferenceValue)) color = Color.red;
                }
            }

            GUI.color = color;
            TD.DrawProperty(linkToPrefab);

            GUI.color = Color.white;

            EditorGUILayout.EndVertical();

            TD.DrawSpacer();
            
            GUI.color = Color.red * TD.editorSkinMulti;
            EditorGUILayout.BeginVertical("Box");
            GUI.color = Color.white;

            TD.DrawLabelWidthUnderline("Position", 14);
            
            DrawSlider(randomPosition, 0, 1, new GUIContent("Random Position"));
            if (GUI.changed)
            {
                serializedObject.ApplyModifiedProperties();
                selectItem.parentItem.CreateMixBuffer();
            }
            DrawVector2(heightRange, false, "Min", "Max", new GUIContent("Height Range"));
            // TD.DrawProperty(includeScale); TODO: Is this needed?
            TD.DrawProperty(heightOffset);
            TD.DrawProperty(includeTerrainHeight);
            TD.DrawProperty(includeTerrainAngle);
            EditorGUILayout.EndVertical();
            
            TD.DrawSpacer();

            GUI.color = Color.yellow * TD.editorSkinMulti;
            EditorGUILayout.BeginVertical("Box");
            GUI.color = Color.white;

            TD.DrawLabelWidthUnderline("Rotation", 14);

            DrawVector2(rotRangeX, false, "Min", "Max", new GUIContent("Rotation Range X"));
            DrawVector2(rotRangeY, false, "Min", "Max", new GUIContent("Rotation Range Y"));
            DrawVector2(rotRangeZ, false, "Min", "Max", new GUIContent("Rotation Range Z"));

            TD.DrawProperty(isSnapRot, new GUIContent("Snap Rotation"));
            if (isSnapRot.boolValue)
            {
                DrawSnapRotation(isSnapRotX, snapRotX, "  X");
                DrawSnapRotation(isSnapRotY, snapRotY, "  Y");
                DrawSnapRotation(isSnapRotZ, snapRotZ, "  Z");
            }

            TD.DrawProperty(lookAtTarget);
            if (lookAtTarget.objectReferenceValue != null) TD.DrawProperty(lookAtX, new GUIContent("  Include X-Axis"));
            EditorGUILayout.EndVertical();

            TD.DrawSpacer();

            DrawScale(true);
        }

        void DrawSnapRotation(SerializedProperty isSnapRot, SerializedProperty snapRotAxis, string label)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(label);
            GUI.changed = false;
            EditorGUILayout.PropertyField(isSnapRot, new GUIContent(""), GUILayout.Width(25));
            EditorGUILayout.PropertyField(snapRotAxis, new GUIContent(""));
            if (GUI.changed) AutoGenerate();
            EditorGUILayout.EndHorizontal();
        }

        public void DrawRangeSlider(TC_SelectItem item, bool labelSpace)
        {
            if (labelSpace) EditorGUILayout.LabelField("");
            Rect rect = GUILayoutUtility.GetLastRect();
            
            if (item.active)
            {
                GUI.color = (TD.hoverItem == item ? Color.green : Color.white);
                item.range = TD.MinMaxSlider.Draw(rect, item.range, 0.0f, 1.0f, new Vector2(200.0f, 25.0f));

                if (TD.MinMaxSlider.changed)
                {
                    selectItemGroup.SetRanges(item);
                    DoRepaint();
                }
            }
            else
            {
                GUI.color = (TD.hoverItem == item ? new Color(0, 1, 0, 0.35f) : new Color(1, 1, 1, 0.35f));
                TD.MinMaxSlider.Draw(rect, item.range, 0.0f, 1.0f, new Vector2(200.0f, 25.0f));
            }

            // if (GUIW.changed) Repaint();

            //if (global_script.tooltip_mode != 0)
            //{
            //    tooltip_text = "Center this value to 50";
            //}
            GUILayout.Space(3);

            if (GUILayout.Button(new GUIContent("C", "Center this value"), EditorStyles.miniButtonMid, GUILayout.Width(25.0f))) selectItemGroup.CenterRange(item);
            EditorGUILayout.LabelField(item.range.x.ToString("F2") + " - " + item.range.y.ToString("F2"), GUILayout.Width(90.0f));

            GUI.color = Color.white;
        }

        public void DrawVector2(SerializedProperty property, bool useLimit, string xLabel, string yLabel, GUIContent guiContent = null, float xLabelWidth = 35, float yLabelWidth = 35)
        {
            GUI.changed = false;

            float labelWidth = EditorGUIUtility.labelWidth;

            EditorGUILayout.BeginHorizontal();
            if (guiContent != null) EditorGUILayout.PrefixLabel(guiContent);
            else EditorGUILayout.PrefixLabel(property.name);
            
            EditorGUIUtility.labelWidth = xLabelWidth;
            float x = EditorGUILayout.FloatField(xLabel, property.vector2Value.x);

            EditorGUIUtility.labelWidth = yLabelWidth;
            float y = EditorGUILayout.FloatField(yLabel, property.vector2Value.y);
            
            property.vector2Value = new Vector2(x, y);

            EditorGUILayout.EndHorizontal();

            EditorGUIUtility.labelWidth = labelWidth;

            if (GUI.changed)
            {
                if (useLimit)
                {
                    if (property.vector2Value.x < 0.001f) property.vector2Value = new Vector2(0.001f, property.vector2Value.y);
                    if (property.vector2Value.x > property.vector2Value.y) property.vector2Value = new Vector2(property.vector2Value.x, property.vector2Value.x + 0.001f);
                }
                AutoGenerate();
            }
        }

        public void DrawMinMaxSlider(SerializedProperty property, float minValue, float maxValue, float limit, GUIContent guiContent, float width = -1)
        {
            GUI.changed = false;
            EditorGUILayout.BeginHorizontal();
            Vector2 v = property.vector2Value;
            
            if (width == -1)
            {
                EditorGUILayout.PrefixLabel(guiContent);
                EditorGUILayout.MinMaxSlider(ref v.x, ref v.y, minValue, maxValue);
                //EditorGUILayout.LabelField("");
                //rect = GUILayoutUtility.GetLastRect();
                //v = GUIW.MinMaxSlider(rect, v, minValue, maxValue, new Vector2(rect.width, 50));
            }
            else
            {
                EditorGUILayout.PrefixLabel(guiContent);
                EditorGUILayout.MinMaxSlider(ref v.x, ref v.y, minValue, maxValue);
            }
            property.vector2Value = v;
            EditorGUILayout.EndHorizontal();
            if (property.vector2Value.x + limit > property.vector2Value.y) property.vector2Value = new Vector2(property.vector2Value.x, property.vector2Value.x + limit);

            if (GUI.changed)
            {
                // if (property.vector2Value.x  < limit) property.vector2Value = new Vector2(limit, property.vector2Value.y);
                
                AutoGenerate();
            }
        }

        public void DrawSlider(SerializedProperty property, float leftValue, float rightValue, GUIContent guiContent = null, float width = -1)
        {
            GUI.changed = false;
            if (width == -1)
            {
                if (guiContent == null) EditorGUILayout.Slider(property, leftValue, rightValue);
                else EditorGUILayout.Slider(property, leftValue, rightValue, guiContent);
            }
            else
            {
                if (guiContent == null) EditorGUILayout.Slider(property, leftValue, rightValue, GUILayout.Width(width));
                else EditorGUILayout.Slider(property, leftValue, rightValue, guiContent, GUILayout.Width(width));
            }
            if (GUI.changed) AutoGenerate();
        }

        public void DrawIntSlider(SerializedProperty property, int leftValue, int rightValue, GUIContent guiContent = null, float width = -1)
        {
            GUI.changed = false;
            if (width == -1)
            {
                if (guiContent == null) EditorGUILayout.IntSlider(property, leftValue, rightValue);
                else EditorGUILayout.IntSlider(property, leftValue, rightValue, guiContent);
            }
            else
            {
                if (guiContent == null) EditorGUILayout.IntSlider(property, leftValue, rightValue, GUILayout.Width(width));
                else EditorGUILayout.IntSlider(property, leftValue, rightValue, guiContent, GUILayout.Width(width));
            }
            if (GUI.changed) AutoGenerate();
        }
    }
}