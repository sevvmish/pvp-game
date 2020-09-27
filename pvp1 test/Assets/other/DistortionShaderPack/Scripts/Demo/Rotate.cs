using UnityEngine;

namespace nightowl.DistortionShaderPack
{
	public class Rotate : MonoBehaviour
	{
		// Refs
		public Vector3 RotateAxis = Vector3.up;

		// Mono
		void Update()
		{
			transform.Rotate(RotateAxis, 1f);
		}
	}
}