using UnityEngine;

namespace nightowl.DistortionShaderPack
{
	public class AnimatedNormalMap : MonoBehaviour
	{

		// Refs
		public Texture[] NormalMaps;
		public bool IsActive = true;
		public float FrameDelay = -1;
		public Material[] refMaterials;

		// Fields
		private int counter = 0;
		private float currTime = 0;

		// Mono
		void Start()
		{
			if (NormalMaps.Length == 0 || (refMaterials.Length == 0 && GetComponent<Renderer>() == null))
			{
				Debug.LogWarning("AnimatedNormalMap is not setup. normal maps = 0 or Reference material and renderer is missing");
				IsActive = false;
			}
		}

		void Update()
		{
			if (!IsActive || NormalMaps.Length <= 0)
				return;

			if (FrameDelay > 0)
			{
				UpdateTime();
			}
			else
			{
				NextFrame();
			}
		}

		// AnimatedNormalMap
		private void UpdateTime()
		{
			currTime += Time.deltaTime;
			if (currTime > FrameDelay)
			{
				currTime -= FrameDelay;
				NextFrame();
			}
		}

		private void NextFrame()
		{
			counter++;
			if (counter >= NormalMaps.Length)
			{
				counter = 0;
			}

			Material[] materials;
			if (refMaterials.Length > 0)
			{
				materials = refMaterials;
			}
			else
			{
				materials = GetComponent<Renderer>().sharedMaterials;
			}

			foreach (Material mat in materials)
			{
				SetTexture(mat);
			}
		}

		private void SetTexture(Material mat)
		{
			if (mat.HasProperty("_NormalTexture"))
			{
				mat.SetTexture("_NormalTexture", NormalMaps[counter]);
			}
		}
	}
}