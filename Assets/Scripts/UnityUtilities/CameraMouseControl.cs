using Assets.Cinemachine.Base.Runtime.Behaviours;
using UnityEngine;

namespace Assets.Scripts.UnityUtilities
{
	public class CameraMouseControl : MonoBehaviour
	{
		private Vector3 _lastPosition;
		private const int CameraPriorityDelta = 2;

		public float MouseSensitivity = 0.008f;

		public CinemachineVirtualCamera PannableCamera;
		public CinemachineVirtualCamera FollowPlayerCamera;

		// Use this for initialization
		void Start()
		{
		}

		// Update is called once per frame
		void Update()
		{
			if (Input.GetMouseButtonDown(2))
			{
				_lastPosition = Input.mousePosition;
				PannableCamera.Priority += CameraPriorityDelta;
				PannableCamera.transform.position = FollowPlayerCamera.transform.position;
			}

			if (Input.GetMouseButton(2))
			{
				var delta = Input.mousePosition - _lastPosition;
				float adjustedSensitivity = MouseSensitivity * Camera.main.orthographicSize;
				PannableCamera.transform.Translate(-delta.x * adjustedSensitivity, -delta.y * adjustedSensitivity, 0);
				_lastPosition = Input.mousePosition;
			}

			if (Input.GetMouseButtonUp(2))
			{
				PannableCamera.Priority -= CameraPriorityDelta;
			}
		}
	}
}