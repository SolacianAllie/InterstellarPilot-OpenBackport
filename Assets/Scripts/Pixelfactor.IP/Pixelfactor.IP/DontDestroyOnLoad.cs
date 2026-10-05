using UnityEngine;

namespace Pixelfactor.IP
{
	public class DontDestroyOnLoad : MonoBehaviour
	{
		private void Awake()
		{
			Object.DontDestroyOnLoad(gameObject);
		}
	}
}
