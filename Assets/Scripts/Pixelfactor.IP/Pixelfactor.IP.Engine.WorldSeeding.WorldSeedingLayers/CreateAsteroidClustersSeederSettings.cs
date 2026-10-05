using UnityEngine;

namespace Pixelfactor.IP.Engine.WorldSeeding.WorldSeedingLayers
{
	public class CreateAsteroidClustersSeederSettings : MonoBehaviour
	{
		public int MinAsteroidClusterCount = 3;

		public int MaxAsteroidClusterCount = 5;

		public float MinDistanceBetweenAsteroidClusters = 1700f;

		public float MinAsteroidClusterRadius = 2500f;

		public float MaxAsteroidClusterRadius = 4000f;

		public float AsteroidClusterRadiusPower = 1f;

		public float ProbabilityOfGeneratingGasCloud = 0.75f;
	}
}
