using UnityEngine;
using System.Collections.Generic;

namespace nightowl.DistortionShaderPack
{
	public class CameraPath : MonoBehaviour
	{
		// Refs
		public float MoveSpeed = 0.1f;
		public float StayDuration = 1f;
		public List<Transform> DemoObjects;
		public float TargetPositionTolerance = 0.05f;
		public Vector3 Offset;

		// Fields
		private float currentTime = 0f;
		private int currentListIndex = 0;
		private Vector3 EndPoint;

		// Mono
		void Start()
		{
			currentListIndex = DemoObjects.Count - 1;
			StartNextMove();
		}

		void Update()
		{
			currentTime += Time.deltaTime;
			if (currentTime > StayDuration)
			{
				StartNextMove();
			}

			if (Vector3.Distance(EndPoint, transform.position) > TargetPositionTolerance)
			{
				Move();
			}
		}

		// CameraPath
		private void StartNextMove()
		{
			currentTime = 0;
			currentListIndex++;

			if (currentListIndex >= DemoObjects.Count)
			{
				currentListIndex = 0;
			}
			EndPoint = DemoObjects[currentListIndex].position + Offset;
		}

		private void Move()
		{
			transform.position = Vector3.Lerp(transform.position, EndPoint, MoveSpeed);
		}
	}
}