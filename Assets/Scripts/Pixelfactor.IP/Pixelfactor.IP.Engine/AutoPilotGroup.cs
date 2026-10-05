using UnityEngine;

namespace Pixelfactor.IP.Engine
{
	[RequireComponent(typeof(Fleet))]
	public class AutoPilotGroup : MonoBehaviour
	{
		private Fleet fleet;

		private void Awake()
		{
			fleet = GetComponent<Fleet>();
		}
	}
}
