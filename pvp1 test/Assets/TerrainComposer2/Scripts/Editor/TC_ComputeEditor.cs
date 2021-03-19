using UnityEngine;
using UnityEditor;
using System.Collections;

namespace TerrainComposer2
{
    [CustomEditor(typeof(TC_Compute))]
    public class TC_ComputeEditor : Editor
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
            TD.DrawSpacer();
            TD.DrawLabel("This script is needed for generating on the GPU", 12);
            TD.DrawSpacer();
        }
    }
}