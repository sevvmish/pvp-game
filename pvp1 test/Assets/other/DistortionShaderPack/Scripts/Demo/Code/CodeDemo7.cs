using UnityEngine;

namespace nightowl.DistortionShaderPack
{
	public class CodeDemo7 : MonoBehaviour
	{
		// Refs
		public Material material;

		// Mono
		void Update()
		{
			material.SetVector("_UVOffset", new Vector4(Mathf.Sin(Time.time), Mathf.Cos(Time.time), 0, 0).normalized*0.1f);
		}
	}
}