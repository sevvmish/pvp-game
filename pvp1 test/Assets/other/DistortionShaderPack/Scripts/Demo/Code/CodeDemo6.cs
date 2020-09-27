using UnityEngine;

namespace nightowl.DistortionShaderPack
{
	public class CodeDemo6 : MonoBehaviour
	{
		// Refs
		public Material material;

		// Mono
		void Update()
		{
			material.SetFloat("_NormalTexFrameless", Mathf.Abs(Mathf.Sin(Time.time)));
		}
	}
}