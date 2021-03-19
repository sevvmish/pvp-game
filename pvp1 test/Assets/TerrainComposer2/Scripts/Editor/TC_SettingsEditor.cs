using UnityEngine;
using UnityEditor;
using System.Collections;

namespace TerrainComposer2
{
    [CustomEditor(typeof(TC_Settings))]
    public class TC_SettingsEditor : Editor
    {
        readonly int[] previewResolutions = new[] { 64, 128, 192, 256, 384, 512 };
        readonly string[] previewResolutionsDisplay = new[] { "64", "128", "192", "256", "384", "512" };

        // Local Settings
        SerializedProperty masterTerrain;
        SerializedProperty previewResolution;
        SerializedProperty hideTerrainGroup;
        SerializedProperty useTCRuntime;
        SerializedProperty defaultTerrainHeight;
        SerializedProperty generateOffset;
        SerializedProperty resExpandBorderPercentage;
        // SerializedProperty scrollAdd;

        // Global Settings
        SerializedObject global;
        SerializedProperty tooltip;
        SerializedProperty previewColors;

        SerializedProperty colLayerGroup;
        SerializedProperty colLayer;
        SerializedProperty colMaskNodeGroup;
        SerializedProperty colMaskNode;
        SerializedProperty colSelectNodeGroup;
        SerializedProperty colSelectNode;
        SerializedProperty colSelectItemGroup;
        SerializedProperty colSelectItem;

        SerializedProperty keyZoomIn, keyZoomOut;
        SerializedProperty showResolutionWarnings;
        SerializedProperty linkScaleToMaskDefault;

        public void OnEnable()
        { 
            masterTerrain = serializedObject.FindProperty("masterTerrain");
            previewResolution = serializedObject.FindProperty("previewResolution");
            hideTerrainGroup = serializedObject.FindProperty("hideTerrainGroup");
            useTCRuntime = serializedObject.FindProperty("useTCRuntime");
            defaultTerrainHeight = serializedObject.FindProperty("defaultTerrainHeight");
            generateOffset = serializedObject.FindProperty("generateOffset");
            resExpandBorderPercentage = serializedObject.FindProperty("resExpandBorderPercentage");
            // scrollAdd = serializedObject.FindProperty("scrollAdd");

            global = new SerializedObject(((TC_Settings)target).global);
            
            tooltip = global.FindProperty("tooltip");
            
            previewColors = global.FindProperty("previewColors");

            colLayerGroup = global.FindProperty("colLayerGroup");
            colLayer = global.FindProperty("colLayer");
            colMaskNodeGroup = global.FindProperty("colMaskNodeGroup");
            colMaskNode = global.FindProperty("colMaskNode");
            colSelectNodeGroup = global.FindProperty("colSelectNodeGroup");
            colSelectNode = global.FindProperty("colSelectNode");
            colSelectItemGroup = global.FindProperty("colSelectItemGroup");
            colSelectItem = global.FindProperty("colSelectItem");

            keyZoomIn = global.FindProperty("keyZoomIn");
            keyZoomOut = global.FindProperty("keyZoomOut");

            showResolutionWarnings = global.FindProperty("showResolutionWarnings");
            linkScaleToMaskDefault = global.FindProperty("linkScaleToMaskDefault");

            Transform t = ((MonoBehaviour)target).transform;
            t.hideFlags = HideFlags.NotEditable | HideFlags.HideInInspector;
        }

        public override void OnInspectorGUI()
        {
            if (TC_Settings.instance == null) return;
            TC_NodeWindow.Keys();
            
            if (TC_Settings.instance.debugMode) DrawDefaultInspector(); else DrawCustomInspector();
        }

        public void DrawCustomInspector()
        {
            TC_GlobalSettings globalSettings = TC_Settings.instance.global;
            
            serializedObject.Update();
            global.Update();
            
            TD.DrawSpacer();
            TD.DrawLabelWidthUnderline("Local Settings", 14);

            EditorGUILayout.BeginVertical("Box");

            GUILayout.Space(5);

            TD.DrawProperty(masterTerrain, new GUIContent("Master Terrain", globalSettings.tooltip ? "This terrain is used for selecting the splat textures, grass textures and trees in the nodes." : ""));

            EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel(new GUIContent("Node Preview Image Resolution", globalSettings.tooltip ? "The resolution of the node preview images." : ""));
                previewResolution.intValue = EditorGUILayout.IntPopup(previewResolution.intValue, previewResolutionsDisplay, previewResolutions);
            EditorGUILayout.EndHorizontal();

            TD.DrawProperty(hideTerrainGroup, new GUIContent("Hide TerrainLayer GameObject"));
            if (GUI.changed)
            {
                serializedObject.ApplyModifiedProperties();
                TC_NodeWindow.DebugMode();
            }

            TD.DrawProperty(useTCRuntime);
            if (GUI.changed)
            {
                if (!useTCRuntime.boolValue) TC_Settings.instance.transform.parent.tag = "EditorOnly"; else TC_Settings.instance.transform.parent.tag = "Untagged";
            }
            TD.DrawProperty(defaultTerrainHeight);
            TD.DrawProperty(generateOffset);
            EditorGUILayout.Slider(resExpandBorderPercentage, 0, 1);
            // TD.DrawProperty(scrollAdd);
            EditorGUILayout.EndVertical();

            GUILayout.Space(10);
            TD.DrawSpacer();
            GUILayout.Space(10);

            TD.DrawLabelWidthUnderline("Global Settings", 14);
            GUILayout.Space(5);

            EditorGUILayout.BeginVertical("Box");
            TD.DrawProperty(tooltip);
            TD.DrawProperty(showResolutionWarnings);
            
            GUI.changed = false;
            Vector3 defaultTerrainSize = globalSettings.defaultTerrainSize;
            defaultTerrainSize = EditorGUILayout.Vector3Field("Default Node Size", defaultTerrainSize);
            if (GUI.changed)
            {
                Undo.RecordObject(globalSettings, "Default Terrain Size");
                globalSettings.defaultTerrainSize = defaultTerrainSize;
                EditorUtility.SetDirty(globalSettings);
            }
            EditorGUILayout.EndVertical();

            GUILayout.Space(5);

            TD.DrawLabelWidthUnderline("Trees and Objects", 12);
            EditorGUILayout.BeginVertical("Box");
                TD.DrawProperty(linkScaleToMaskDefault, new GUIContent("Link Scale To Mask Default"));
            EditorGUILayout.EndVertical();

            GUILayout.Space(5);

            TD.DrawLabelWidthUnderline("Keys", 12);

            EditorGUILayout.BeginVertical("Box");
            TD.DrawProperty(keyZoomIn);
            TD.DrawProperty(keyZoomOut);
            EditorGUILayout.EndVertical();

            TD.DrawLabelWidthUnderline("Node Colors", 12);

            EditorGUILayout.BeginVertical("Box");

            TD.DrawProperty(colLayerGroup, new GUIContent("Color Layer Group"));
            TD.DrawProperty(colLayer, new GUIContent("Color Layer"));
            TD.DrawProperty(colMaskNodeGroup, new GUIContent("Color Mask Node Group"));
            TD.DrawProperty(colMaskNode, new GUIContent("Color Mask Node"));
            TD.DrawProperty(colSelectNodeGroup, new GUIContent("Color Select Node Group"));
            TD.DrawProperty(colSelectNode, new GUIContent("Color Select Node"));
            TD.DrawProperty(colSelectItemGroup, new GUIContent("Color Select Item Group"));
            TD.DrawProperty(colSelectItem, new GUIContent("Color Select Item"));

            EditorGUILayout.EndVertical();

            GUILayout.Space(5);

            TD.DrawPropertyArray(previewColors);

            TD.DrawSpacer();
            
            serializedObject.ApplyModifiedProperties();
            global.ApplyModifiedProperties();
        }
    }
}
