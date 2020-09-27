/*
 * By Martin Reintges 05/2016
 */

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace nightowl.DistortionShaderPack
{
	[CustomEditor(typeof (RenderTextureCamera))]
	public class RenderTextureCameraEditor : Editor
	{
		private static readonly string[] displayOptionsDepth = new[] {"16", "24"};
		private static readonly int[] displayValuesDepth = new[] {16, 24};
		private static readonly string[] displayOptionsAntiAliasing = new[] {"1", "2", "4", "8"};
		private static readonly int[] displayValuesAntiAliasing = new[] {1, 2, 4, 8};


		public override void OnInspectorGUI()
		{
			var obj = (RenderTextureCamera) target;

			obj.layerId = EditorGUILayout.LayerField("Layer", obj.layerId);
			EditorGUI.BeginChangeCheck();

			obj.SetTextureWidth(EditorGUILayout.IntField("RT width", obj.width));
			obj.SetTextureHeight(EditorGUILayout.IntField("RT height", obj.height));
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.PrefixLabel("RT depth");
			obj.depth = EditorGUILayout.IntPopup(obj.depth, displayOptionsDepth, displayValuesDepth);
			EditorGUILayout.EndHorizontal();

			List<string> layers = new List<string>();
			for (int i = 0; i < 32; i++)
			{
				string layerName = LayerMask.LayerToName(i);
				layers.Add(layerName);
			}
			obj.cullingMask = EditorGUILayout.MaskField("cullingMask", obj.cullingMask, layers.ToArray());

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.PrefixLabel("RT AntiAliasing");
			obj.antiAliasing = EditorGUILayout.IntPopup(obj.antiAliasing, displayOptionsAntiAliasing, displayValuesAntiAliasing);
			EditorGUILayout.EndHorizontal();

			if (EditorGUI.EndChangeCheck())
			{
				obj.RecreateRenderTexture();
			}
		}
	}
}