using UnityEngine;

namespace nightowl.DistortionShaderPack
{
	public class CodeDemo8 : MonoBehaviour
	{
		// Refs
		public Material material;

		// Mono
		void Update()
		{
			material.SetFloat("_StrengthAlpha", Mathf.Abs(Mathf.Sin(Time.time)));
		}
	}
}