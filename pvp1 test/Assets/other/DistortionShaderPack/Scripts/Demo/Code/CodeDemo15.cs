using UnityEngine;

namespace nightowl.DistortionShaderPack
{
	public class CodeDemo15 : MonoBehaviour
	{
		// Refs
		public Material material;

		// Mono
		void Update()
		{
			material.SetFloat("_TextureDistortion", Mathf.Sin(Time.time)*0.5f + 0.5f);
		}
	}
}