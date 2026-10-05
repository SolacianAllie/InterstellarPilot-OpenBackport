using UnityEngine;

namespace Pixelfactor.IP.Engine
{
	public class DestroyOnScreenTouch : MonoBehaviour
	{
		public void Update()
		{
			if (Input.GetMouseButton(0) || Input.touchCount > 0)
			{
				Object.Destroy(gameObject);
			}
		}
	}
}
