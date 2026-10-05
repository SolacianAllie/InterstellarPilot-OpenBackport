using System.Collections.Generic;
using Pixelfactor.IP.Engine.WorldPopulation;
using UnityEngine;

namespace Pixelfactor.IP.Engine.WorldSeeding.WorldSeedingLayers
{
	public class CreateBlueprintSectorsSeederSettings : MonoBehaviour
	{
		public Wormhole JumpGatePrefab;

		public float SectorMapPositionScaleFudge = 2f;

		public List<Sector> SectorPrefabs;

		public SectorLightingSettings SectorLightingSettings;
	}
}
