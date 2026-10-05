using System.Collections.Generic;
using System.Linq;
using Pixelfactor.IP.Common;
using UnityEngine;

namespace Pixelfactor.IP.Engine.WorldSeeding.WorldSeedingLayers
{
	[RequireComponent(typeof(WorldSeederLayer))]
	public class WorldTraderCargoSeeder : MonoBehaviour
	{
		public WorldTraderCargoSeederSettings WorldTraderCargoSeederSettings;

		public void SeedLayer(WorldBase world)
		{
			if (LogWrapper.LogMsgs)
			{
				LogWrapper.Log("Seeding trader cargo...", this, 1);
			}
			WorldTraderCargoSeederSettings = world.Seeder.Settings.WorldTraderCargoSeederSettings;
			Seed(world, WorldTraderCargoSeederSettings);
		}

		public void Seed(WorldBase world, WorldTraderCargoSeederSettings settings)
		{
			foreach (CargoTrader trader in world.Engine.Traders)
			{
				TrySpawnCargo(trader.UnitComponents, settings);
			}
		}

		public static bool TrySpawnCargo(UnitComponentHolder unitComponents, WorldTraderCargoSeederSettings settings)
		{
			if (unitComponents != null)
			{
				if (unitComponents.ConstructionState == ConstructionState.Constructed)
				{
					if (unitComponents.CargoBayComponent != null)
					{
						AddCargoBasedOnUnitTrader(unitComponents, settings);
						return true;
					}
					Debug.LogWarning($"could not spawn cargo for unit: {unitComponents} because the unit has no cargo bay component attached", unitComponents);
				}
			}
			else
			{
				Debug.LogWarning($"could not spawn cargo. NullReference");
			}
			return false;
		}

		private static bool IsOreFoundInSector(CargoClass cargoClass, Sector sector)
		{
			List<Unit> unitsByType = sector.GetUnitsByType(UnitType.AsteroidCluster);
			if (unitsByType != null && unitsByType.Count > 0)
			{
				foreach (Unit item in unitsByType)
				{
					foreach (Asteroid asteroidUnitPrefab in item.GetComponent<AsteroidCluster>().AsteroidType.AsteroidUnitPrefabs)
					{
						if (asteroidUnitPrefab.AsteroidClass.AsteroidYieldItems.Any((AsteroidYieldItem e) => e.CargoClass == cargoClass))
						{
							return true;
						}
					}
				}
			}
			List<Unit> unitsByType2 = sector.GetUnitsByType(UnitType.Asteroid);
			if (unitsByType2 != null && unitsByType2.Count > 0)
			{
				foreach (Unit item2 in unitsByType2)
				{
					if (item2.Asteroid.AsteroidClass.AsteroidYieldItems.Any((AsteroidYieldItem e) => e.CargoClass == cargoClass))
					{
						return true;
					}
				}
			}
			return false;
		}

		private static void AddCargoBasedOnUnitTrader(UnitComponentHolder unitComponentHolder, WorldTraderCargoSeederSettings settings)
		{
			Unit unit = unitComponentHolder.Unit;
			CargoTrader component = unit.GetComponent<CargoTrader>();
			if (!(component != null))
			{
				return;
			}
			foreach (KeyValuePair<int, CargoTraderStockLevels> stockLevel in component.GetStockLevels())
			{
				CargoClass cargoClassById = unit.Engine.GetCargoClassById(stockLevel.Key);
				float num = (float)stockLevel.Value.HighStockMax * cargoClassById.WorldSeedQuantityFudge;
				float num2 = settings.DefaultCargoPowerMultiplier;
				if (component.IsConsumer)
				{
					num2 = settings.ConsumerCargoPowerMultiplier;
				}
				if (unitComponentHolder.FactoryComponent != null && unitComponentHolder.FactoryComponent.GetOutputQuantity(cargoClassById) > 0)
				{
					if (Random.value < settings.ProbabilityOfSurplusCargo)
					{
						num *= settings.SurplusCargoMultiplier;
					}
					num2 = settings.ProducerCargoPowerMultiplier;
				}
				if (cargoClassById.IsOre)
				{
					if (!IsOreFoundInSector(cargoClassById, unit.Sector))
					{
						num2 *= 16f;
					}
				}
				else if (cargoClassById.ConstituentCargoClasses.Count > 0)
				{
					foreach (CargoClass constituentCargoClass in cargoClassById.ConstituentCargoClasses)
					{
						if (constituentCargoClass != null && constituentCargoClass.IsOre && !IsOreFoundInSector(constituentCargoClass, unit.Sector))
						{
							num2 *= 16f;
							break;
						}
					}
				}
				float value = cargoClassById.Rarity * settings.CargoRarityMultiplier;
				num *= 1f - Mathf.Clamp(value, 0f, 0.9f);
				int preferredQuantity = Mathf.Min((int)num, unitComponentHolder.CargoBayComponent.GetFreeSpaceFor(cargoClassById));
				AddCargo(unitComponentHolder, cargoClassById, preferredQuantity, settings, num2 * settings.GeneralCargoPowerMultiplier);
			}
		}

		private static void AddCargo(UnitComponentHolder unitComponentHolder, CargoClass cargoClass, int preferredQuantity, WorldTraderCargoSeederSettings settings, float powerMultiplier = 1f)
		{
			int num = (int)(settings.InitialTraderCargoMaxStockMultiplier * (float)preferredQuantity);
			int num2 = (int)(settings.InitialTraderCargoMinStockMultiplier * (float)preferredQuantity);
			float value = Random.value;
			if (value < 0.05f)
			{
				powerMultiplier *= 0.2f;
			}
			else if (value > 0.9f)
			{
				powerMultiplier *= 4f;
			}
			int amount = (int)Mathf.Lerp(num2, num, Mathf.Pow(p: Mathf.Lerp(settings.InitialTraderCargoPowerMax, settings.InitialTraderCargoPowerMin, Mathf.Clamp01(settings.InitialTraderCargoPower01)) * powerMultiplier, f: Random.value));
			unitComponentHolder.CargoBayComponent.AddToCargoIfFits(cargoClass, amount);
		}
	}
}
