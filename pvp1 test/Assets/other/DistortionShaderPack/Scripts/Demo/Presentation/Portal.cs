using UnityEngine;

namespace nightowl.DistortionShaderPack
{
	public class Portal : MonoBehaviour
	{
		// Refs
		public Material Material;
		public Light Light;
		public Color PortalColor = Color.magenta;
		public float TargetDistortionStrength = -2;
		public float TargetNormalDistortion = 1;
		public float Speed = 0.3f;
		public float TimeOffset = 0;

		// Mono
		void Update()
		{
			float frac = (Mathf.Sin(Time.time + TimeOffset)*0.5f + 0.5f)/Speed;

			Material.SetFloat("_DistortionStrength", frac*TargetDistortionStrength);
			Material.SetFloat("_NormalTexStrength", frac*TargetNormalDistortion);
			Material.SetColor("_MainColor", frac*PortalColor);

			Light.intensity = frac;
		}

		// Portal
		private void UpdateValues()
		{
		}
	}
}