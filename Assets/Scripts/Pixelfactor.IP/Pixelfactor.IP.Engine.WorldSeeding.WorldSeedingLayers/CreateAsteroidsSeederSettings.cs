using UnityEngine;

namespace Pixelfactor.IP.Engine.WorldSeeding.WorldSeedingLayers
{
	public class CreateAsteroidsSeederSettings : MonoBehaviour
	{
		public int MinAsteroidCount = 3;

		public int MaxAsteroidCount = 5;

		public float MinDistanceBetweenAsteroids = 175f;

		public float AsteroidCountReferenceRadius = 1000f;
	}
}
