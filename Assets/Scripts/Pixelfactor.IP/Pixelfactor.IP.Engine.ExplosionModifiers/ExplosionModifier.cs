using UnityEngine;

namespace Pixelfactor.IP.Engine.ExplosionModifiers
{
	public class ExplosionModifier : MonoBehaviour
	{
		public virtual float GetMaxDistance(Sector sector, Vector3 sectorPosition, float distance)
		{
			return distance;
		}
	}
}
