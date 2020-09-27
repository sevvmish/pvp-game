using UnityEngine;

namespace nightowl.DistortionShaderPack
{
	public class CodeDemo3 : MonoBehaviour
	{
		// Refs
		public Material material;

		// Mono
		void Update()
		{
			material.SetFloat("_DistortionCircle", Mathf.Abs(Mathf.Sin(Time.time)));
		}
	}
}