using Pixelfactor.IP.Engine;
using UnityEngine;

namespace Pixelfactor.IP
{
	public class PositionAtCamera : MonoBehaviour
	{
		public void Update()
		{
			if (GameController.Instance != null && GameController.Instance.MainCamera != null)
			{
				transform.position = GameController.Instance.MainCamera.transform.position;
			}
		}
	}
}
