using System.Collections.Generic;
using Pixelfactor.IP.Engine;
using Pixelfactor.Unity.Utils;

namespace Pixelfactor.IP.Scenarios.RandomEvents
{
	public class AsteroidYieldEvent : RandomUniverseEvent
	{
		public override void Generate(bool silent = false)
		{
			base.Generate();
			Asteroid asteroid = FindRandomAsteroid();
			if (asteroid != null)
			{
				asteroid.YieldCargo(null, null, applyForces: false, applyEffects: false, setExpiryTime: false, 0.2f);
			}
		}

		private Asteroid FindRandomAsteroid()
		{
			for (int i = 0; i < 4; i++)
			{
				List<Unit> unitsByType = World.Engine.Sectors.GetRandom().GetUnitsByType(UnitType.Asteroid);
				if (unitsByType != null && unitsByType.Count > 0)
				{
					return unitsByType.GetRandom().Asteroid;
				}
			}
			return null;
		}
	}
}
