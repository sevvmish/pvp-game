using UnityEngine;
using UnityEditor;
using System.Collections;

namespace TerrainComposer2
{
    [CustomEditor (typeof(ReadMe))]
    public class ReadMeEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            ReadMe r = (ReadMe)target;

            Event eventCurrent = Event.current;

            GUI.changed = false;

            if (eventCurrent.control && eventCurrent.shift && eventCurrent.keyCode == KeyCode.E && eventCurrent.type == EventType.KeyDown)
            {
                r.buttonEdit = !r.buttonEdit;
                GUI.changed = true;
            }
            
            GUILayout.Space(5);

            if (r.buttonEdit)
            {
                r.readme = EditorGUILayout.TextArea(r.readme);
                r.buttonLink = EditorGUILayout.TextField(r.buttonLink);
            }
            else
            {
                EditorGUILayout.TextArea(r.readme);
                GUILayout.Space(5);
                GUI.backgroundColor = Color.green;
                if (GUILayout.Button("Download Examples Pack"))
                {
                    Application.OpenURL("https://mega.nz/#!7QBj3YzR!yHXkhDY2Njvk0obqSA36vzn5EVcN4biRC15nJSxK4ao");
                }
                if (GUILayout.Button("Download Stamp Pack"))
                {
                    Application.OpenURL("https://mega.nz/#!HNIjxZZA!OQTmlM1jiT5rOCPz7_PZBJ-8UwJOPOsW-ghs25wHAik");
                }
                if (GUILayout.Button("Documentation"))
                {
                    Application.OpenURL("http://www.terraincomposer.com/terraincomposer2-documentation/");
                }
                GUI.backgroundColor = Color.white;
            }


            if (GUI.changed) EditorUtility.SetDirty(target);
        }
    
    }
}