using UnityEngine;

namespace nightowl.DistortionShaderPack
{
	public class CodeDemo2 : MonoBehaviour
	{
		// Refs
		public Material material;

		// Mono
		void Update()
		{
			material.SetFloat("_DistortionStrength", Mathf.Sin(Time.time));
		}
	}
}