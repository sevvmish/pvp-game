using UnityEngine;

namespace nightowl.DistortionShaderPack
{
	public class BounceSphere : MonoBehaviour
	{
		// Fields
		public Rigidbody Rigidbody;
		public float AddForceDelay = 2f;
		public float AddForce = 10f;

		public bool XAxis = true;
		public bool YAxis = true;
		public bool ZAxis = true;

		private float currentTime = 0;

		// Mono
		void Update()
		{
			currentTime += Time.deltaTime;
			if (currentTime >= AddForceDelay)
			{
				currentTime -= AddForceDelay;
				AddRandomForce();
			}
		}

		// BounceSphere
		private void AddRandomForce()
		{
			Vector3 direction = new Vector3(XAxis ? Random.value - 0.5f : 0, YAxis ? Random.value : 0,
				ZAxis ? Random.value - 0.5f : 0);
			Rigidbody.AddForce(direction.normalized*AddForce);
		}
	}
}