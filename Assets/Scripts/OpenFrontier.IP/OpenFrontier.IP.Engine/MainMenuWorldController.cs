using System.Collections.Generic;
using System.Linq;
using OpenFrontier.IP.Common.Factions;
using OpenFrontier.IP.Engine.Core.Units;
using OpenFrontier.IP.Engine.Factions;
using OpenFrontier.IP.Scenarios;
using OpenFrontier.IP.Testing.Spawning;
using OpenFrontier.Unity.Utils;
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
			// Replace the preset dressing before regenerating: stock
			// stations/ships/planet/shuttle/clusters go, everything comes
			// back randomly below.
			ClearPresetMenuContent();
			RandomizeAsteroidClusterTypes(sector);
			// Applies everything: regenerates the backdrop, refreshes the
			// sun tint, recaptures the reflection probe.
			EngineASX.Instance.ActiveSectorData.OnSectorChanged();
			SpawnGuaranteedPlanetWithMoon(sector);
			PopulateMenuSystem(sector);
			SpawnHeroShipAndSpectate(sector);
			Debug.Log("[MainMenuWorldController] rolled menu system, seed " + num);
		}

		// The trader faction that owns the stations and traffic; the hero
		// ship joins it so the faction AI hands it trade runs.
		private static Faction civilianFaction;

		// The MenuScreenScene's hand-placed world content - replaced by the
		// random composition below on every visit.
		private static readonly string[] PresetUnitNames = new string[6] { "unit_space_station_tef (1)", "UnitRefinery", "unit_shuttle_a", "UnitPlanetOrangeWithImpacts", "UnitAsteroidClusterTypeA", "UnitHypersleepPodFactory" };

		private static readonly string[] PresetRootPrefixes = new string[5] { "Bay_", "Dock", "AIGenericPilotController", "GenericNpc", "GenericGroupNoCloak" };

		private static void ClearPresetMenuContent()
		{
			List<Unit> list = new List<Unit>();
			EngineASX.Instance.EnumerateUnitsWithPredicate(delegate(Unit unit)
			{
				Unit rootUnit = unit.GetRootUnit();
				if (rootUnit != null && System.Array.IndexOf(PresetUnitNames, rootUnit.gameObject.name) >= 0)
				{
					list.Add(rootUnit);
				}
			}, (Unit e) => e.IsValidAndNotDestroyed);
			// Menu-only: the gas clouds go too - clearer belts and a
			// cleaner backdrop for the random system.
			List<Unit> list2 = new List<Unit>();
			EngineASX.Instance.EnumerateUnitsWithPredicate(delegate(Unit unit)
			{
				Unit rootUnit2 = unit.GetRootUnit();
				if (rootUnit2 != null && rootUnit2.GetComponent<UnitGasCloud>() != null)
				{
					list2.Add(rootUnit2);
				}
			}, (Unit e) => e.IsValidAndNotDestroyed);
			list.AddRange(list2);
			foreach (Unit item in list)
			{
				// Notify first (fleets and AI processors drop their
				// references), then clean up - raw Destroy leaves the
				// faction AI ticking over corpses and spams the log.
				EngineASX.Instance.RegisterDestroyedUnit(item);
				item.SafeDestroy();
			}
			foreach (GameObject gameObject in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
			{
				string name = gameObject.name;
				if (PresetRootPrefixes.Any((string prefix) => name.StartsWith(prefix)))
				{
					Unit component = gameObject.GetComponent<Unit>();
					if (component != null)
					{
						EngineASX.Instance.RegisterDestroyedUnit(component);
						component.SafeDestroy();
					}
					else
					{
						Object.Destroy(gameObject);
					}
				}
			}
		}

		private static void RandomizeAsteroidClusterTypes(Sector sector)
		{
			AsteroidType[] array = Resources.LoadAll<AsteroidType>("Prefabs/WorldData/AsteroidTypes");
			List<Unit> unitsByType = sector.GetUnitsByType(UnitType.AsteroidCluster);
			if (array.Length == 0 || unitsByType == null)
			{
				return;
			}
			foreach (Unit item in unitsByType)
			{
				AsteroidCluster component = item.GetComponent<AsteroidCluster>();
				if (component != null)
				{
					component.AsteroidType = array[UnityEngine.Random.Range(0, array.Length)];
				}
			}
		}

		// The menu ALWAYS has a planet, and it ALWAYS has a moon.
		// (Mirrors CreatePlanetsSeeder's direct instantiate + Init path -
		// SpawnUtils.SpawnUnit is for regular units and drops planets.)
		private static void SpawnGuaranteedPlanetWithMoon(Sector sector)
		{
			string[] planetNames = new string[10] { "ActiveUnitPlanetAlienAshyGreen", "ActiveUnitPlanetAlienPurple", "ActiveUnitPlanetDevoidOfLife", "ActiveUnitPlanetEarthLike", "ActiveUnitPlanetEarth", "ActiveUnitPlanetFrozenTundras", "ActiveUnitPlanetJupitersCousin", "ActiveUnitPlanetOrangeWithImpacts", "ActiveUnitPlanetVolcanica", "ActiveUnitPlanetWinterWorld" };
			Unit unitPrefab = UnityObjectHelper.Load<Unit>("Prefabs/WorldData/ActiveUnits/Environment/" + planetNames[UnityEngine.Random.Range(0, planetNames.Length)]);
			if (unitPrefab == null)
			{
				Debug.LogWarning("[MainMenuWorldController] planet prefab failed to load");
				return;
			}
			Unit unit = UnityObjectHelper.InstantiateAndGetComponent(unitPrefab);
			unit.transform.SetParent(sector.transform, worldPositionStays: true);
			unit.transform.localPosition = sector.GetRandomSectorPositionWithinGateDistance(0.5f);
			UnitPlanet component = unit.GetComponent<UnitPlanet>();
			if (component != null)
			{
				component.Rotation = new Vector3(UnityEngine.Random.Range(-30f, 30f), UnityEngine.Random.Range(0f, 360f), 0f);
			}
			unit.GetComponent<Unit>().Init();
			Unit unitPrefab2 = UnityObjectHelper.Load<Unit>("Prefabs/WorldData/ActiveUnits/Environment/ActiveUnitMoon" + UnityEngine.Random.Range(1, 5));
			if (unitPrefab2 == null)
			{
				return;
			}
			Unit unit2 = UnityObjectHelper.InstantiateAndGetComponent(unitPrefab2);
			Moon component2 = unit2.GetComponent<Moon>();
			if (component2 != null)
			{
				component2.OrbitingAroundUnit = unit;
				float y = UnityEngine.Random.value * 360f;
				float x = UnityEngine.Random.Range(-30f, 30f);
				component2.OffsetFromPlanet = Quaternion.Euler(x, y, 0f) * Vector3.forward * (unit.Radius * UnityEngine.Random.Range(2.5f, 4f));
				unit2.GetComponent<Unit>().Init(autoFindParents: false);
				unit2.Sector = sector;
			}
		}

		// The camera's opening view: a random ship from the whole pool, so
		// over time the menu shows off every ship class in the game. It
		// joins the trader faction as a fleet of one, so the faction AI
		// gives it the old shuttle's life: exploring the system and
		// periodically docking at stations.
		private static void SpawnHeroShipAndSpectate(Sector sector)
		{
			var list = GameController.Instance.LoadedUnitClasses.Where((UnitClass e) => e.IsUsable && e.UnitType == UnitType.Ship).ToList();
			if (list.Count == 0)
			{
				return;
			}
			Unit unit = SpawnUtils.SpawnUnit(list[UnityEngine.Random.Range(0, list.Count)].UnitPrefab, sector, sector.GetRandomSectorPositionWithinGateDistance(0.3f), civilianFaction);
			if (unit != null)
			{
				if (civilianFaction != null)
				{
					SpawnUtils.SpawnGenericFleetWithUnits(civilianFaction, new List<Unit> { unit });
				}
				if (EngineASX.Instance.World != null)
				{
					EngineASX.Instance.World.Engine.CameraStartSpectateUnit(unit, allowFlyby: true, allowViewport: false, allowOrbit: true);
				}
			}
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
			civilianFaction = CreateAIFaction(FactionType.Trader);
			Faction civilian = civilianFaction;
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
