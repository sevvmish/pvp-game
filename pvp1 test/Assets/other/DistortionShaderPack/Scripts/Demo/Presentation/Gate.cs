using UnityEngine;

namespace nightowl.DistortionShaderPack
{
	public class Gate : MonoBehaviour
	{
		// Refs
		public Material Material;

		// Fields
		public float Delay = 10;
		public float Duration = 2;

		private float currentTime;


		// Mono

		void Start()
		{
			Material.SetFloat("_TextureStrength", 1);
			Material.SetFloat("_NormalTexStrength", 1);
		}


		void Update()
		{
			currentTime += Time.deltaTime;

			if (currentTime < Delay)
				return;

			float frac = (currentTime - Delay) / Duration;
			Material.SetFloat("_TextureStrength", Mathf.Max(0, 0.9f - frac));
			Material.SetFloat("_NormalTexStrength", Mathf.Max(0, 0.9f - frac));

		}
	}
}