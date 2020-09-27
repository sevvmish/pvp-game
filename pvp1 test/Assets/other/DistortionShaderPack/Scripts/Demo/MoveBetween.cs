using UnityEngine;

namespace nightowl.DistortionShaderPack
{
	public class MoveBetween : MonoBehaviour
	{
		// Refs
		public Vector3 Start = Vector3.zero;
		public Vector3 End = Vector3.one;
		public float Speed = 0.1f;
		public bool local = true;
		public float TimeOffset = 0;

		// Mono
		void Update()
		{
			var frac = (Mathf.Sin((Time.time + TimeOffset)*Speed)*0.5f + 0.5f);
			var target = Start + Vector3.Lerp(Start, End, frac);

			if (local)
				transform.localPosition = target;
			else
				transform.position = target;
		}
	}
}