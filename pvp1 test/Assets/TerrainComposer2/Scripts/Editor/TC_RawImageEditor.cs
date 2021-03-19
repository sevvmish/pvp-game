using UnityEngine;
using UnityEditor;
using System.Collections;

namespace TerrainComposer2
{
    [CustomEditor(typeof(TC_RawImage))]
    public class TC_RawImageEditor : Editor
    {
        void OnEnable()
        {
            Transform t = ((MonoBehaviour)target).transform;
            t.hideFlags = HideFlags.NotEditable | HideFlags.HideInInspector;
        }

        public override void OnInspectorGUI()
        {
            if (TC_Settings.instance == null) return;
            if (TC_Settings.instance.debugMode) DrawDefaultInspector(); else DrawCustomInspector();
        }

        void DrawCustomInspector()
        {
            TC_RawImage rawImage = (TC_RawImage)target;

            TD.DrawSpacer();

            TD.DrawLabelWidthUnderline("Loaded stamp texture for GPU", 14);

            GUILayout.Space(25);
            Rect rect = GUILayoutUtility.GetLastRect();

            float width = TC_NodeWindow.window.position.width - (rect.x * 2);
            if (width > 768) width = 768;
            float min = (768 - width);
            if (min > 50) min = 50;

            TD.DrawTexture(new Rect(rect.x, rect.y + width, width, -width), rawImage.tex, string.Empty, Color.white);
            GUILayout.Space(width - min);

            TD.DrawSpacer();

            TD.DrawLabelWidthUnderline("Path", 14);

            EditorGUILayout.BeginVertical("Box");
            EditorGUILayout.LabelField(rawImage.path);
            EditorGUILayout.EndVertical();

            GUILayout.Space(10);
            
            TD.DrawLabelWidthUnderline("Details", 14);
            
            EditorGUILayout.BeginVertical("Box");
            
            EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel("Resolution");
                EditorGUILayout.LabelField(rawImage.resolution.ToString());
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel("Node References");
                EditorGUILayout.LabelField(rawImage.referenceCount.ToString());
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();

            TD.DrawSpacer();
        }
    }
}