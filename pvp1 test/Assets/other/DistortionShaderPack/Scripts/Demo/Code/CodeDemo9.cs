using UnityEngine;

namespace nightowl.DistortionShaderPack
{
	public class CodeDemo9 : MonoBehaviour
	{
		// Refs
		public Material material;
		public Texture Texture;

		// Mono
		void Update()
		{
			material.SetTexture("_MainTexture", Mathf.Sin(Time.time) > 0 ? Texture : null);
		}
	}
}