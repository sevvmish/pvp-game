using UnityEngine;
using UnityEditor;
using System.Collections;

namespace TerrainComposer2
{
    [CustomEditor(typeof(TC_Area2D), true)]
    [CanEditMultipleObjects]
    public class TC_Area2DEditor : Editor
    {
        SerializedProperty terrainAreas;

        void OnEnable()
        {
            Transform t = ((MonoBehaviour)target).transform;
            t.hideFlags = HideFlags.NotEditable | HideFlags.HideInInspector;

            terrainAreas = serializedObject.FindProperty("terrainAreas");
        }

        public override void OnInspectorGUI()
        {
            if (TC_Settings.instance == null) return;

            serializedObject.Update();
            if (TC_Settings.instance.debugMode) DrawDefaultInspector(); else DrawCustomInspector();

            serializedObject.ApplyModifiedProperties();
        }

        void DrawCustomInspector()
        {
            // TD.DrawSpacer();
            // TD.DrawLabelWidthUnderline("Area 2D script", 14);

            TD.DrawSpacer();
                TD.DrawPropertyArray(terrainAreas);
            TD.DrawSpacer();
            // TD.DrawProperty(terrainLayer);
        }
    }
}
