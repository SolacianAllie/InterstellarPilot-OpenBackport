using UnityEngine;

namespace Pixelfactor.IP.Engine.WorldSeeding.WorldSeedingLayers
{
	[RequireComponent(typeof(WorldSeederLayer))]
	public class FactionRecacheValidTraderTargetSeeder : MonoBehaviour
	{
		public void SeedLayer(WorldBase world)
		{
			WorldSeeder.PopulateFactionValidTraderTargets();
		}
	}
}
