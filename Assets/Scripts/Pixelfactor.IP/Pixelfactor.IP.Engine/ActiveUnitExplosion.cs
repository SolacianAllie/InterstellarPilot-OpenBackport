using UnityEngine;

namespace Pixelfactor.IP.Engine
{
	public class ActiveUnitExplosion : MonoBehaviour
	{
		public Unit DestroyedUnit;

		private void Update()
		{
			if ((bool)DestroyedUnit)
			{
				transform.position = DestroyedUnit.transform.position;
			}
		}
	}
}
