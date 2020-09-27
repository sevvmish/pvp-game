using UnityEngine;

namespace nightowl.DistortionShaderPack
{
	public class ConfigurationDemo8 : MonoBehaviour
	{
		// Refs
		public Material Material;
		public Color PortalColor = Color.magenta;
		public float TargetDistortionStrength = -2;
		public float TargetNormalDistortion = 1;
		public float Speed = 0.3f;

		// Mono
		void Update()
		{
			float frac = (Mathf.Sin(Time.time)*0.5f + 0.5f)/Speed;

			Material.SetFloat("_DistortionStrength", frac*TargetDistortionStrength);
			Material.SetFloat("_NormalTexStrength", frac*TargetNormalDistortion);
			Material.SetColor("_MainColor", frac*PortalColor);
		}
	}
}