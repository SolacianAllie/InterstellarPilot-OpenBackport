using UnityEngine;

namespace Pixelfactor.IP.Engine.WorldSeeding.WorldSeedingLayers
{
	public class WorldSeederLayer : MonoBehaviour
	{
		public void Seed(WorldBase world)
		{
			SendMessage("SeedLayer", world, SendMessageOptions.RequireReceiver);
		}
	}
}
