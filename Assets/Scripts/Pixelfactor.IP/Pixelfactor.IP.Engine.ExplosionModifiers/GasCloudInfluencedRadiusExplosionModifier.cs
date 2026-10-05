using Pixelfactor.IP.Engine.Core.Units;
using Pixelfactor.IP.Engine.GasClouds;
using UnityEngine;

namespace Pixelfactor.IP.Engine.ExplosionModifiers
{
	public class GasCloudInfluencedRadiusExplosionModifier : ExplosionModifier
	{
		public float MaxInfluence01 = 0.75f;

		public override float GetMaxDistance(Sector sector, Vector3 sectorPosition, float distance)
		{
			UnitGasCloud gasCloudAtWorldPosition = GasCloudHelper.GetGasCloudAtWorldPosition(sector, sector.ToWorldPosition(sectorPosition));
			if (gasCloudAtWorldPosition != null)
			{
				float num = 1f - gasCloudAtWorldPosition.GasCloudClass.DetectionRangeMultiplier;
				distance *= 1f - num * MaxInfluence01;
			}
			return distance;
		}
	}
}
