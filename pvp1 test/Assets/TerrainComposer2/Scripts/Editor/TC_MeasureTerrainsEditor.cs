using UnityEngine;
using System;
using UnityEditor;

namespace TerrainComposer2
{
    [CustomEditor(typeof(TC_MeasureTerrains), true)]
    public class TC_MeasureTerrainsEditor : Editor
    {
        public TC_MeasureTerrains mt;

        Ray ray;
        Event key;

        RaycastHit hit;

        void OnEnable()
        {
            Transform t = ((MonoBehaviour)target).transform;
            t.hideFlags = HideFlags.NotEditable | HideFlags.HideInInspector;
        }

        public void OnSceneGUI()
        {
            mt = (TC_MeasureTerrains)target;
            key = Event.current;

            ray = HandleUtility.GUIPointToWorldRay(key.mousePosition);

            if (Physics.Raycast(ray, out mt.hit))
            {
                hit = mt.hit;

                mt.terrain = hit.collider.GetComponent<Terrain>();
                mt.mr = hit.collider.GetComponent<MeshRenderer>();
                
                Terrain terrain = mt.terrain;

                // if (mt.terrain != null) Debug.Log(mt.terrain.name);

                if (terrain != null)
                {
                    mt.ReadTerrain();
                }
                else if (mt.mr != null)
                {
                    mt.localPos = new Vector2(hit.point.x - mt.mr.transform.position.x, hit.point.z - mt.mr.transform.position.z);
                }

                mt.angle = hit.normal.y * 90;
            }
        }

        public override void OnInspectorGUI()
        {
            mt = (TC_MeasureTerrains)target;

            TD.DrawSpacer();

            GUILayout.Space(5.0f);
            TD.DrawLabelWidthUnderline("Terrain", 14);
            
            EditorGUILayout.BeginVertical("Box");

            GUI.changed = false;
            EditorGUILayout.BeginHorizontal();
            if (mt.terrain != null)
            {
                EditorGUILayout.PrefixLabel("Terrain name");
                EditorGUILayout.LabelField(mt.terrain.name);
            }
            else
            {
                if (mt.mr != null)
                {
                    EditorGUILayout.PrefixLabel("Mesh");
                    EditorGUILayout.LabelField(mt.mr.name);
                }
                else
                {
                    EditorGUILayout.LabelField("-", GUILayout.Width(70.0f));
                }
            }
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(5.0f);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("World Position");
            EditorGUILayout.LabelField("X " + hit.point.x.ToString("F2") + "   Y " + hit.point.y.ToString("F2") + "   Z " + hit.point.z.ToString("F2"));
            EditorGUILayout.EndHorizontal();

            // GUILayout.Space(5.0f);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Local Position");
            EditorGUILayout.LabelField("X " + mt.localPos.x.ToString("F2") + "   Y " + mt.localPos.y.ToString("F2"));
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(5.0f);

            // ! inbed locked!

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Height");
            EditorGUILayout.LabelField(mt.height.ToString("F2") + "  (" + mt.normalizedHeight.ToString("F3") + ")");
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Angle");
            EditorGUILayout.LabelField(mt.angle.ToString("F2") + "  (" + hit.normal.y.ToString("F3") + ")");
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Normal");
            EditorGUILayout.LabelField("X " + hit.normal.x.ToString("F2") + " Z " + hit.normal.z.ToString("F2"));
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(10.0f);

            EditorGUILayout.EndVertical();

            GUILayout.Space(5.0f);

            EditorGUILayout.BeginHorizontal();
            if (mt.drawSplat) GUI.backgroundColor = Color.green;
            if (GUILayout.Button("Splat", GUILayout.Width(55))) mt.drawSplat = !mt.drawSplat;
            if (mt.drawGrass) GUI.backgroundColor = Color.green; else GUI.backgroundColor = Color.white;
            if (GUILayout.Button("Grass", GUILayout.Width(55))) mt.drawGrass = !mt.drawGrass;
            GUI.backgroundColor = Color.white;
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(5);
            // TD.DrawSpacer();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Zoom", GUILayout.Width(50));
            mt.textureSize = (int)EditorGUILayout.Slider(mt.textureSize, 25, 150);
            EditorGUILayout.EndHorizontal();
            //GUILayout.Space(5);
            TD.DrawSpacer();

            if (mt.drawSplat) { DrawSplat(); GUILayout.Space(15); }
            if (mt.drawGrass) DrawGrass();

            TD.DrawSpacer();

            Repaint();

            //GUILayout.Space(5.0f);

            //float detail_total = 0.0f;

            //for (int count_detail = 0; count_detail < detail_length; ++count_detail)
            //{
            //    EditorGUILayout.BeginHorizontal();
            //    GUILayout.Space(15.0f);
            //    EditorGUILayout.LabelField("Detail" + count_detail + "", GUILayout.Width(100.0f));
            //    EditorGUILayout.LabelField("" + detail[count_detail], GUILayout.Width(50.0f));
            //    detail_total += (float)detail[count_detail];
            //    GUILayout.Space(74.0f);
            //    EditorGUILayout.LabelField(locked);
            //    EditorGUILayout.EndHorizontal();
            //}

            //GUILayout.Space(5.0f);

            //EditorGUILayout.BeginHorizontal();
            //GUILayout.Space(15.0f);
            //if (global_script.settings.color_scheme) { GUI.color = Color.green; }
            //EditorGUILayout.LabelField("Grass Total", GUILayout.Width(100.0f));
            //EditorGUILayout.LabelField("" + detail_total.ToString("F2"), GUILayout.Width(50.0f));
            //GUILayout.Space(74.0f);
            //EditorGUILayout.LabelField(locked);
            //EditorGUILayout.EndHorizontal();

            //GUILayout.Space(5.0f);
            //if (global_script.settings.color_scheme) { GUI.color = Color.white; }

            //EditorGUILayout.BeginHorizontal();
            //GUILayout.Space(15.0f);
            //EditorGUILayout.LabelField("Min Height", GUILayout.Width(100.0f));
            //EditorGUILayout.LabelField(script.settings.terrainMinHeight.ToString("F2"), GUILayout.Width(50.0f));
            //EditorGUILayout.LabelField("(" + (script.settings.terrainMinHeight / heightmap_scale.y).ToString("F3") + ")", GUILayout.Width(70.0f));
            //if (GUILayout.Button("Calc", GUILayout.Width(55.0f)))
            //{
            //    if (script.settings.showTerrains) script.get_terrains_minmax();
            //    else script.get_meshes_minmax_height();
            //}
            //EditorGUILayout.EndHorizontal();

            //EditorGUILayout.BeginHorizontal();
            //GUILayout.Space(15.0f);
            //EditorGUILayout.LabelField("Max Height", GUILayout.Width(100.0f));
            //EditorGUILayout.LabelField(script.settings.terrainMaxHeight.ToString("F2"), GUILayout.Width(50.0f));
            //EditorGUILayout.LabelField("(" + (script.settings.terrainMaxHeight / heightmap_scale.y).ToString("F3") + ")", GUILayout.Width(70.0f));
            //EditorGUILayout.EndHorizontal();

            //EditorGUILayout.BeginHorizontal();
            //GUILayout.Space(15.0f);
            //EditorGUILayout.LabelField("Min Steepness", GUILayout.Width(100.0f));
            //EditorGUILayout.LabelField(script.settings.terrainMinDegree.ToString("F2"), GUILayout.Width(50.0f));
            //EditorGUILayout.LabelField("(" + (script.settings.terrainMinDegree / heightmap_scale.y).ToString("F3") + ")", GUILayout.Width(70.0f));
            //EditorGUILayout.EndHorizontal();

            //EditorGUILayout.BeginHorizontal();
            //GUILayout.Space(15.0f);
            //EditorGUILayout.LabelField("Max Steepness", GUILayout.Width(100.0f));
            //EditorGUILayout.LabelField(script.settings.terrainMaxDegree.ToString("F2"), GUILayout.Width(50.0f));
            //EditorGUILayout.LabelField("(" + (script.settings.terrainMaxDegree / heightmap_scale.y).ToString("F3") + ")", GUILayout.Width(70.0f));
            //EditorGUILayout.EndHorizontal();

            //GUILayout.Space(5.0f);

            //EditorGUILayout.BeginHorizontal();
            //GUILayout.Space(15.0f);
            //string range_text = null;
            //if (script.measure_tool_inrange) { range_text = "*"; } else { range_text = "?"; }
            //EditorGUILayout.LabelField("Measure Range", GUILayout.Width(100.0f));
            //script.measure_tool_range = EditorGUILayout.Slider(script.measure_tool_range, 1.0f, 100000.0f, GUILayout.Width(300.0f));
            //EditorGUILayout.LabelField(range_text, GUILayout.Width(100.0f));
            //EditorGUILayout.EndHorizontal();

            //EditorGUILayout.BeginHorizontal();
            //GUILayout.Space(15.0f);
            //EditorGUILayout.LabelField("Sphere Radius", GUILayout.Width(100.0f));
            //script.sphere_radius = EditorGUILayout.Slider(script.sphere_radius, 0.1f, 50.0f, GUILayout.Width(300.0f));
            //EditorGUILayout.EndHorizontal();

            //GUILayout.Space(5.0f);
            //EditorGUILayout.BeginHorizontal();
            //GUILayout.Space(15.0f);
            //EditorGUILayout.LabelField("Sphere Gizmos", GUILayout.Width(100.0f));
            //script.sphere_draw = EditorGUILayout.Toggle(script.sphere_draw, GUILayout.Width(25.0f));
            //EditorGUILayout.EndHorizontal();

            //GUILayout.Space(5.0f);
            //EditorGUILayout.BeginHorizontal();
            //GUILayout.Space(15.0f);
            //EditorGUILayout.LabelField("Converter Calculator");
            //EditorGUILayout.EndHorizontal();

            //EditorGUILayout.BeginHorizontal();
            //GUILayout.Space(15.0f);
            //EditorGUILayout.LabelField("Height", GUILayout.Width(100.0f));

            //GUI.changed = false;
            //script.measure_tool_converter_height_input = EditorGUILayout.FloatField(script.measure_tool_converter_height_input, GUILayout.Width(80.0f));
            //if (GUI.changed)
            //{
            //    if (terrain_measure != null) { script.measure_tool_converter_height = script.measure_tool_converter_height_input / terrain_measure.terrainData.size.y; }
            //}
            //EditorGUILayout.LabelField("-> " + script.measure_tool_converter_height.ToString("f3"));
            //EditorGUILayout.EndHorizontal();

            //EditorGUILayout.BeginHorizontal();
            //GUILayout.Space(15.0f);
            //EditorGUILayout.LabelField("Angle", GUILayout.Width(100.0f));
            //GUI.changed = false;
            //script.measure_tool_converter_angle_input = EditorGUILayout.FloatField(script.measure_tool_converter_angle_input, GUILayout.Width(80.0f));
            //if (GUI.changed)
            //{
            //    script.measure_tool_converter_angle = script.measure_tool_converter_angle_input / 90;
            //}
            //EditorGUILayout.LabelField("-> " + script.measure_tool_converter_angle.ToString("f3"));
            //EditorGUILayout.EndHorizontal();

            //if (script.measure_tool_active && !script.measure_tool_undock) { this.Repaint(); }
        }

        void DrawSplat()
        {
            if (mt.terrain != null && mt.splat != null)
            {
                TD.DrawLabelWidthUnderline("Splat", 14);

                GUI.color = Color.red * TD.editorSkinMulti;
                EditorGUILayout.BeginVertical("Box");
                GUI.color = Color.white;

                float splatTotal = 0;

                Texture2D[] splatTextures = TC.GetTerrainSplatTextures(mt.terrain);

                for (int i = 0; i < splatTextures.Length; ++i)
                {
                    EditorGUILayout.BeginHorizontal();
                    
                    TC_ItemBehaviourEditor.DrawPreviewTexture(splatTextures[i], Color.white, Color.white, mt.textureSize, mt.textureSize);
                    float splatValue = mt.splat[0, 0, i];
                    EditorGUILayout.LabelField("");
                    Rect rect = GUILayoutUtility.GetLastRect();
                    EditorGUI.ProgressBar(rect, splatValue, splatValue.ToString("F2"));
                    splatTotal += splatValue;
                    EditorGUILayout.EndHorizontal();
                }

                GUILayout.Space(5.0f);

                GUI.color = Color.yellow * TD.editorSkinMulti;
                EditorGUILayout.BeginVertical("Box");
                GUI.color = Color.white;

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel("Splat Total");
                EditorGUILayout.LabelField(splatTotal.ToString("F3"));
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();

                EditorGUILayout.EndVertical();
            }
        }
        
        void DrawGrass()
        {
            if (mt.terrain != null && mt.grassLayer != null)
            {
                TD.DrawLabelWidthUnderline("Grass", 14);

                GUI.color = Color.green * TD.editorSkinMulti;
                EditorGUILayout.BeginVertical("Box");
                GUI.color = Color.white;

                float grassTotal = 0;

                for (int i = 0; i < mt.grassLayer.Length; ++i)
                {
                    EditorGUILayout.BeginHorizontal();
                    TC_ItemBehaviourEditor.DrawPreviewTexture(mt.terrain.terrainData.detailPrototypes[i].prototypeTexture, Color.white, Color.white, mt.textureSize, mt.textureSize);
                    float grassValue = mt.grassLayer[i].grass[0, 0];
                    EditorGUILayout.LabelField("");
                    Rect rect = GUILayoutUtility.GetLastRect();
                    EditorGUI.ProgressBar(rect, grassValue / 16, grassValue.ToString());
                    grassTotal += grassValue;
                    EditorGUILayout.EndHorizontal();
                }

                GUILayout.Space(5.0f);

                GUI.color = Color.yellow * TD.editorSkinMulti;
                EditorGUILayout.BeginVertical("Box");
                GUI.color = Color.white;
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel("Grass Total");
                EditorGUILayout.LabelField(grassTotal.ToString("F3"));
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.EndVertical();

                EditorGUILayout.EndVertical();
            }
        }
    }
}