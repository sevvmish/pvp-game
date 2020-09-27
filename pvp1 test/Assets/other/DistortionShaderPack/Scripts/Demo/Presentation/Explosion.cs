using UnityEngine;

namespace nightowl.DistortionShaderPack
{
	public class Explosion : MonoBehaviour
	{
		// Enum
		private enum State
		{
			Pause = 0,
			Implode,
			Explode
		}

		// Refs
		public Transform ExplosionQuad;
		public Material ExplosionMat;

		public AnimationCurve ScaleCurve;
		public AnimationCurve DistortionCurve;

		// Fields
		public float PauseDuration = 5f;
		public float ImplosionDuration = 1f;
		public float ExplosionDuration = 2f;

		private State currentState = State.Pause;
		private float currentTime = 0;

		// Mono
		void Start()
		{
			Reset();
		}

		void Update()
		{
			currentTime += Time.deltaTime;
			float stateDuration = currentTime/GetStateDuration();
			if (stateDuration >= 1.0f)
			{
				NextState();
				stateDuration = 0;
				currentTime = 0;
			}

			if (currentState == State.Implode)
				UpdateImplosion(stateDuration);
			else if (currentState == State.Explode)
				UpdateExplosion(stateDuration);
		}

		// Explosion
		private void Reset()
		{
			ExplosionQuad.localScale = Vector3.one;

			ExplosionMat.SetFloat("_DistortionStrength", 0);
			ExplosionMat.SetFloat("_StrengthAlpha", 0);
			ExplosionMat.SetColor("_MainColor", Color.black);
		}

		private void UpdateImplosion(float fraction)
		{
			ExplosionMat.SetFloat("_DistortionStrength", 1.5f*fraction + Random.value*0.1f);
			ExplosionMat.SetColor("_MainColor", Color.black*(1 - fraction) + Color.white*fraction);
		}

		private void UpdateExplosion(float fraction)
		{
			float scale = ScaleCurve.Evaluate(fraction);
			ExplosionQuad.localScale = Vector3.one*scale;
			float distorion = DistortionCurve.Evaluate(fraction);
			ExplosionMat.SetFloat("_DistortionStrength", distorion);
			fraction = Mathf.Min(1f, fraction*4f);
			ExplosionMat.SetColor("_MainColor", Color.white*(1 - fraction) + Color.black*fraction);
		}

		private float GetStateDuration()
		{
			if (currentState == State.Pause)
				return PauseDuration;
			else if (currentState == State.Implode)
				return ImplosionDuration;
			else
				return ExplosionDuration;
		}

		private void NextState()
		{
			if (currentState == State.Pause)
			{
				currentState = State.Implode;
			}
			else if (currentState == State.Implode)
			{
				currentState = State.Explode;
			}
			else
			{
				currentState = State.Pause;
				Reset();
			}
		}
	}
}