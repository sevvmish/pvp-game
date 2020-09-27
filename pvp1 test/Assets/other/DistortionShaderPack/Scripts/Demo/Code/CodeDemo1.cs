using UnityEngine;

namespace nightowl.DistortionShaderPack
{
	public class CodeDemo1 : MonoBehaviour
	{
		// Refs
		public Material material;

		// Mono
		void Update()
		{
			material.SetColor("_MainColor", Mathf.Abs(Mathf.Sin(Time.time))*Color.red);
		}
	}
}