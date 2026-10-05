using UnityEngine;

namespace Pixelfactor.IP.UI.Screens
{
	public class DamageFlashControllerBase : MonoBehaviour
	{
		public virtual bool HasStarted => false;

		public virtual void StartFlash()
		{
		}

		public virtual void StopFlash()
		{
		}
	}
}
