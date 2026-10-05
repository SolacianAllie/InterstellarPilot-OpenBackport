using UnityEngine;

namespace Pixelfactor.IP
{
	public class DestroyObject : MonoBehaviour
	{
		private void Awake()
		{
			Object.Destroy(gameObject);
		}
	}
}
