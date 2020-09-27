using UnityEngine;

namespace nightowl.DistortionShaderPack
{
	public class CodeDemo4 : MonoBehaviour
	{
		// Refs
		public Material material;
		public Texture NormalTexture;

		// Mono
		void Update()
		{
			material.SetTexture("_NormalTexture", Mathf.Sin(Time.time) > 0 ? NormalTexture : null);
		}
	}
}