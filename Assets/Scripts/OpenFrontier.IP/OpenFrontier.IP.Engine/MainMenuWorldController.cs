using System.Linq;
using OpenFrontier.IP.Common.Factions;
using OpenFrontier.IP.Engine.Factions;
using OpenFrontier.IP.Testing.Spawning;
using UnityEngine;

namespace OpenFrontier.IP.Engine
{
	/// <summary>
	/// Open Frontier: re-rolls the main menu's star system on EVERY menu
	/// visit so the backdrop is a fresh randomly generated system each
	/// time, custom-universe style. Detects the menu scenario the same way
	/// EngineMusicPlayerController does (World.ScenarioInfo matches
	/// GameController.MainMenuScenario), then:
	///   1. re-seeds the sector and regenerates its backdrop (nebulae +
	///      starfield params re-rolled via the stock pipeline)
	///   2. rolls a new star color via the seeded HSV generator and writes
	///      it into the sector's DirectionLightColor (drives light, sun
	///      billboard, tint and the reflection probe's anchor sun)
	///   3. jitters the sky exposure
	///   4. POPULATES the system like a light custom universe: civilian
	///      stations + trader traffic, miner fleets working the clusters,
	///      a security patrol, and a chance of a hostile bandit horde -
	///      faction AI assigns strategies and hostilities play out
	///      organically, so the spectating camera always has life to watch.
	/// ActiveSectorData.OnSectorChanged applies the visuals (backdrop
	/// regen + sun tint refresh + probe recapture).
	/// Lives on the persistent GameController.
	/// </summary>
	public class MainMenuWorldController : MonoBehaviour
	{
		private ScenarioInfo appliedScenario;

		private void Update()
		{
			if (!EngineASX.LoadedAndReady || EngineASX.Instance.World == null || GameController.Instance == null)
			{
				return;
			}
			ScenarioInfo scenarioInfo = EngineASX.Instance.World.ScenarioInfo;
			if (scenarioInfo != appliedScenario)
			{
				appliedScenario = scenarioInfo;
				if (IsMainMenuScenario(scenarioInfo))
				{
					RollMenuSystem();
				}
			}
		}

		private static bool IsMainMenuScenario(ScenarioInfo scenarioInfo)
		{
			ScenarioInfo mainMenuScenario = GameController.Instance.MainMenuScenario;
			return scenarioInfo != null && mainMenuScenario != null && (scenarioInfo == mainMenuScenario || scenarioInfo.UniqueId == mainMenuScenario.UniqueId);
		}

		private static void RollMenuSystem()
		{
			Sector sector = EngineASX.Instance.ActiveSector;
			if (sector == null || EngineASX.Instance.ActiveSectorData == null)
			{
				return;
			}
			int num = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
			System.Random random = new System.Random(num);
			// Fresh backdrop (nebulae/starfield re-rolled from the new seed)
			// and a fresh star color from the seeded generator.
			sector.RandomSeed = num;
			sector.DirectionLightColor = StarColorGenerator.ForSector(num);
			sector.SkyExposure = Mathf.Lerp(0.7f, 1.3f, (float)random.NextDouble());
			// Applies everything: regenerates the backdrop, refreshes the
			// sun tint, recaptures the reflection probe.
			EngineASX.Instance.ActiveSectorData.OnSectorChanged();
			PopulateMenuSystem(sector);
			Debug.Log("[MainMenuWorldController] rolled menu system, seed " + num);
		}

		// Light custom-universe population: everything the local AI needs
		// to exist and work inside this one system.
		private static void PopulateMenuSystem(Sector sector)
		{
			if (EngineASX.Instance.FactionSpawner == null || EngineASX.Instance.FactionSpawner.Settings == null)
			{
				return;
			}
			// 2-3 civilian stations: trade hubs and task targets.
			Faction civilian = CreateAIFaction(FactionType.Trader);
			var list = GameController.Instance.LoadedUnitClasses.Where((UnitClass e) => e.IsUsable && e.UnitType == UnitType.Station && e.SeedInSandbox && e.StationPurpose != StationPurpose.SectorControl).ToList();
			int num = UnityEngine.Random.Range(2, 4);
			for (int i = 0; i < num && list.Count > 0; i++)
			{
				SpawnUtils.SpawnUnit(list[UnityEngine.Random.Range(0, list.Count)].UnitPrefab, sector, sector.GetRandomSectorPositionWithinGateDistance(0.75f), civilian);
			}
			// Trader traffic between the stations.
			if (civilian != null)
			{
				int num2 = UnityEngine.Random.Range(2, 4);
				for (int j = 0; j < num2; j++)
				{
					SpawnUtils.SpawnFleetWithSmallUnits(civilian, sector, sector.GetRandomSectorPositionWithinGateDistance(0.75f), UnityEngine.Random.Range(3, 6));
				}
			}
			// Miners working the clusters.
			Faction faction = CreateAIFaction(FactionType.Miner);
			if (faction != null)
			{
				int num3 = UnityEngine.Random.Range(1, 3);
				for (int k = 0; k < num3; k++)
				{
					SpawnUtils.SpawnFleetWithLargerMiningUnits(faction, sector, sector.GetRandomSectorPositionWithinGateDistance(0.75f), UnityEngine.Random.Range(3, 6));
				}
			}
			// A security patrol.
			Faction faction2 = CreateAIFaction(FactionType.Security);
			if (faction2 != null)
			{
				SpawnUtils.SpawnFleetWithLargerUnits(faction2, sector, sector.GetRandomSectorPositionWithinGateDistance(0.75f), UnityEngine.Random.Range(4, 7));
			}
			// 40% chance of a hostile bandit horde - HostileWithAll, so the
			// locals spot and engage them on their own.
			if (UnityEngine.Random.value < 0.4f)
			{
				SpawnUtils.SpawnBanditHordesAtSectorPosition(sector, sector.GetRandomSectorPositionWithinGateDistance(0.75f), 1, 2);
			}
		}

		private static Faction CreateAIFaction(FactionType factionType)
		{
			FactionSpawnerSpawnType factionSpawnerSpawnType = EngineASX.Instance.FactionSpawner.Settings.FactionTypes.FirstOrDefault((FactionSpawnerSpawnType e) => e.TypeInfo.FactionType == factionType && !e.IsFreelancer);
			if (factionSpawnerSpawnType == null)
			{
				return null;
			}
			return FactionSpawner.CreateFactionAndAIAndAssignName(factionSpawnerSpawnType);
		}
	}
}
