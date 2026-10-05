using Pixelfactor.IP.Engine.WorldGeneration;
using Pixelfactor.IP.Engine.WorldGeneration.Models;
using Pixelfactor.IP.Engine.WorldPopulation;
using Pixelfactor.Unity.Utils;
using UnityEngine;

namespace Pixelfactor.IP.Engine.WorldSeeding.WorldSeedingLayers.SkirmishUniverseGenerator
{
	[RequireComponent(typeof(WorldSeederLayer))]
	public class SkirmishUniverseGeneratorSeeder : MonoBehaviour
	{
		public SkirmishUniverseGeneratorSeederSettings SkirmishUniverseGeneratorSeederSettings;

		public void SeedLayer(WorldBase world)
		{
			if (LogWrapper.LogMsgs)
			{
				LogWrapper.Log("Seeding scenario options...", this, 1);
			}
			SkirmishUniverseGeneratorSeederSettings = world.Seeder.Settings.SkirmishUniverseGeneratorSeederSettings;
			WorldBlueprint worldBlueprint = new WorldBlueprint();
			SectorBlueprint sectorBlueprint = new SectorBlueprint();
			sectorBlueprint.Name = WorldBlueprintSectorGenerator.GetRandomSectorName(world.SeederRandom);
			sectorBlueprint.SecurityLevel = 0.5f;
			sectorBlueprint.SectorType = GetRandomSectorType();
			if (sectorBlueprint.HasAsteroids)
			{
				sectorBlueprint.AsteroidType = world.Seeder.Settings.WorldGeneratorSettings.AsteroidTypes.GetRandom();
			}
			worldBlueprint.SectorNodes.Add(sectorBlueprint);
			CreateBlueprintSectorsSeederSettings createBlueprintSectorsSeederSettings = world.Seeder.Settings.CreateBlueprintSectorsSeederSettings;
			SectorCreator.CreateSectorsAndWormholes(null, worldBlueprint.SectorNodes, EngineASX.Instance, createBlueprintSectorsSeederSettings.SectorPrefabs, createBlueprintSectorsSeederSettings.JumpGatePrefab, createBlueprintSectorsSeederSettings.SectorMapPositionScaleFudge, createBlueprintSectorsSeederSettings.SectorLightingSettings);
		}

		private SectorType GetRandomSectorType()
		{
			SectorType sectorType = SectorType.Unspecified;
			if (EngineASX.Instance.World.SeederRandom.NextFloat() < SkirmishUniverseGeneratorSeederSettings.ChanceOfPlanetSector)
			{
				sectorType |= SectorType.Planet;
			}
			if (EngineASX.Instance.World.SeederRandom.NextFloat() < SkirmishUniverseGeneratorSeederSettings.ChanceOfAsteroidSector)
			{
				sectorType |= SectorType.Asteroid;
			}
			return sectorType;
		}
	}
}
