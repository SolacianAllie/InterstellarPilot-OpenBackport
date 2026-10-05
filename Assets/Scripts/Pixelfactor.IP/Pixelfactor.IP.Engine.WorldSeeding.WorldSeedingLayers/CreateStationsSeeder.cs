using System.Collections.Generic;
using System.Linq;
using Pixelfactor.IP.Common.Factions;
using Pixelfactor.IP.Engine.Factions;
using Pixelfactor.IP.Engine.Factions.StationBuilding;
using Pixelfactor.IP.Engine.WorldPopulation;
using UnityEngine;

namespace Pixelfactor.IP.Engine.WorldSeeding.WorldSeedingLayers
{
	[RequireComponent(typeof(WorldSeederLayer))]
	public class CreateStationsSeeder : MonoBehaviour
	{
		public CreateStationsSeederSettings CreateStationsSeederSettings;

		public List<FactionType> SpecificFactionTypes = new List<FactionType>();

		public List<FactionType> ExcludeSpecificFactionTypes = new List<FactionType>();

		public void SeedLayer(WorldBase world)
		{
			if (LogWrapper.LogMsgs)
			{
				LogWrapper.Log("Seeding stations...", this, 1);
			}
			CreateStationsSeederSettings = world.Seeder.Settings.CreateStationsSeederSettings;
			SeedStations(CreateStationsSeederSettings);
			world.CacheAllFactionValidTraders();
		}

		public void SeedStations(CreateStationsSeederSettings settings)
		{
			foreach (Faction faction in EngineASX.Instance.Factions)
			{
				if (!(faction.FactionAI != null) || (SpecificFactionTypes.Any() && !SpecificFactionTypes.Contains(faction.FactionType)) || (ExcludeSpecificFactionTypes.Any() && ExcludeSpecificFactionTypes.Contains(faction.FactionType)) || faction.AISettings.PreferSingleShip)
				{
					continue;
				}
				int num = Mathf.RoundToInt((float)faction.Credits * GetMaxCreditsSpendOnStationsMultiplier(faction, settings));
				for (WorldStationSeeder.StationBuild nextStationBuild = faction.FactionAI.StationBuilder.GetNextStationBuild(num - faction.CreditsReserve, considerShipRatio: false); nextStationBuild != null; nextStationBuild = faction.FactionAI.StationBuilder.GetNextStationBuild(num - faction.CreditsReserve, considerShipRatio: false))
				{
					faction.FactionAI.SetLastBuiltUnitTimeToCurrent();
					bool underConstruction = nextStationBuild.UnitClass.StationPurpose != StationPurpose.Refinery && nextStationBuild.UnitClass.StationPurpose != StationPurpose.Shipyard && nextStationBuild.UnitClass.StationPurpose != StationPurpose.TradeStation && Random.value < 0.05f;
					Unit unit = FactionStationBuilder.BuildStation(nextStationBuild, cargoLoadout: true, underConstruction);
					num -= nextStationBuild.UnitClass.SaleCost;
					faction.Credits -= nextStationBuild.UnitClass.SaleCost;
					if (unit.IsUnderConstruction)
					{
						unit.Components.ConstructionProgress = Random.Range(0.05f, 0.75f);
						unit.Components.SetHealthForConstructionProgress();
						faction.FactionAI.OnNewStationConstructionStarted(unit);
					}
					else
					{
						faction.FactionAI.OnNewStationFullyConstructed(unit);
					}
					faction.FindAndPickHomeSectorIfNull();
				}
			}
		}

		private static float GetMaxCreditsSpendOnStationsMultiplier(Faction faction, CreateStationsSeederSettings settings)
		{
			if (faction.SpawnType == null)
			{
				return 0.5f;
			}
			return faction.SpawnType.MaxSpendOfSeededStations;
		}
	}
}
