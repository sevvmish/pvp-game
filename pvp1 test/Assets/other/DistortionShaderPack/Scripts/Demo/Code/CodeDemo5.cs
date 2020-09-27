using UnityEngine;

namespace nightowl.DistortionShaderPack
{
	public class CodeDemo5 : MonoBehaviour
	{
		// Refs
		public Material material;

		// Mono
		void Update()
		{
			material.SetFloat("_NormalTexStrength", Mathf.Abs(Mathf.Sin(Time.time)));
		}
	}
}