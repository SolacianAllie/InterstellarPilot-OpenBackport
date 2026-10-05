using Pixelfactor.IP.Engine;
using UnityEngine;

namespace Pixelfactor.IP.Misc
{
	public class FaceCamera : MonoBehaviour
	{
		private void Update()
		{
			if (GameController.Instance != null && GameController.Instance.MainCamera != null)
			{
				transform.rotation = GetRotation(transform.position, GameController.Instance.MainCamera.transform.position);
			}
		}

		public static Quaternion GetRotation(Vector3 ourPosition, Vector3 cameraPosition)
		{
			return Quaternion.LookRotation(Vector3.Normalize(ourPosition - cameraPosition));
		}
	}
}
