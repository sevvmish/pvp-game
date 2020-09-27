/*
 * By Martin Reintges 05/2016
 */

using System.Collections;
using UnityEngine;

// This is the water script from the standard assets,
// just optimized for our use
namespace nightowl.DistortionShaderPack
{
	[ExecuteInEditMode]
	public class RenderTextureCamera : MonoBehaviour
	{
		// Refs
		public int layerId = 2;
		public int width = 1024;
		public int height = 1024;
		public int depth = 16;
		public int cullingMask = -1;
		public int antiAliasing = 1;


		// Static Fields
		private static Hashtable RenderCameras;
		private static Hashtable UpdatedRenderCameras = new Hashtable();

		// Mono
		void Start()
		{
			gameObject.layer = layerId;
		}

		void Update()
		{
			Material[] materials = GetComponent<Renderer>().sharedMaterials;
			foreach (Material mat in materials)
			{
				if (mat.HasProperty("_UVOffset"))
				{
					Vector4 offset = mat.GetVector("_UVOffset");
					offset.z = offset.x * Time.time;
					offset.w = offset.y * Time.time;
					mat.SetVector("_UVOffset", offset);
				}
			}
		}

		void LateUpdate()
		{
			UpdatedRenderCameras.Clear();
		}

		void OnWillRenderObject()
		{
			var renderCam = GetOrCreateCamera();
			if (!renderCam)
				return;

			if (!UpdatedRenderCameras.Contains(renderCam))
			{
				renderCam.Render();
				UpdatedRenderCameras.Add(renderCam, true);
			}

			Material[] materials = GetComponent<Renderer>().sharedMaterials;
			foreach (Material mat in materials)
			{
				if (mat.HasProperty("_RenderTexture"))
					mat.SetTexture("_RenderTexture", renderCam.targetTexture);
			}
		}

		void OnDestroy()
		{
			DestroyAll();
		}

		// RenderTextureCamera
		private Camera GetOrCreateCamera()
		{
			Camera mainCamera = Camera.current;
			if (mainCamera == null)
				return null;

			Camera RenderCamera = GetRenderCamera(mainCamera);
			if (RenderCamera == null)
			{
				GameObject go = new GameObject("RRT Camera id" + GetInstanceID() + " for " + mainCamera.GetInstanceID());
				go.hideFlags = HideFlags.HideAndDontSave;
				go.transform.SetParent(mainCamera.transform, false);
				go.transform.localPosition = Vector3.zero;
				go.transform.rotation = Quaternion.identity;

				RenderCamera = go.AddComponent<Camera>();
				RenderCamera.targetTexture = CreateRenderTexture();
				RenderCamera.cullingMask = cullingMask > -1 ? cullingMask : ~(1 << gameObject.layer) & mainCamera.cullingMask;
				RenderCamera.enabled = false;
				RenderCameras[mainCamera] = RenderCamera;
				RenderCamera.Render();
			}

			RenderCamera.worldToCameraMatrix = mainCamera.worldToCameraMatrix;
			RenderCamera.projectionMatrix = mainCamera.projectionMatrix;

			return RenderCamera;
		}

		private Camera GetRenderCamera(Camera mainCamera)
		{
			if (!mainCamera)
				return null;

			if (RenderCameras == null)
				RenderCameras = new Hashtable();

			return RenderCameras[mainCamera] as Camera;
		}

		private RenderTexture CreateRenderTexture()
		{
			var renderTexture = new RenderTexture(width, height, depth, RenderTextureFormat.Default);
			renderTexture.antiAliasing = antiAliasing;
			renderTexture.Create();
			return renderTexture;
		}

		public void RecreateRenderTexture()
		{
			DestroyAll();
		}

		private void DestroyAll()
		{
			if (RenderCameras == null)
				return;

			foreach (DictionaryEntry cam in RenderCameras)
			{
				var renderCamera = (cam.Value as Camera);
				if (renderCamera == null)
					continue;

				var texture = renderCamera.targetTexture;
				renderCamera.targetTexture = null;
				DestroyImmediate(texture);
			}
			foreach (DictionaryEntry cam in RenderCameras)
			{
				var renderCamera = (cam.Value as Camera);
				if (renderCamera == null)
					continue;

				DestroyImmediate(renderCamera.gameObject);
			}
			RenderCameras = new Hashtable();
		}

		public void SetTextureWidth(int newWidth)
		{
			width = newWidth;
			var list = FindObjectsOfType<RenderTextureCamera>();
			for (int a = 0; a < list.Length; ++a)
			{
				list[a].width = newWidth;
			}
		}

		public void SetTextureHeight(int newHeight)
		{
			height = newHeight;
			var list = FindObjectsOfType<RenderTextureCamera>();
			for (int a = 0; a < list.Length; ++a)
			{
				list[a].height = newHeight;
			}
		}
	}
}