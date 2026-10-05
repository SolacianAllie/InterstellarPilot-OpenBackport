using System.Collections.Generic;
using Pixelfactor.IP.Common.Factions;
using Pixelfactor.IP.Engine;
using Pixelfactor.IP.Engine.CombatRatings;
using Pixelfactor.IP.Engine.CustomUnitVariants;

namespace Pixelfactor.IP.UI.Screens.Skirmish
{
	public static class SkirmishHelper
	{
		public static IEnumerable<UnitClass> GetAvailableVanillaShips()
		{
			UnitClass[] loadedUnitClasses = GameController.Instance.LoadedUnitClasses;
			foreach (UnitClass unitClass in loadedUnitClasses)
			{
				if (unitClass.IsPilottable && unitClass.IsUsable && unitClass.IsArmed && (unitClass.GetEffectivenessAtStrategy(FactionStrategy.War) > 0.1f || unitClass.GetEffectivenessAtStrategy(FactionStrategy.Scout) > 0.1f) && !unitClass.IsBarebonesShip)
				{
					yield return unitClass;
				}
			}
		}

		public static List<SkirmishTeamParamsItem> GetAvailableShips(bool includeCustomVariants, IEnumerable<CustomUnitVariant> loadedCustomUnitVariants)
		{
			List<SkirmishTeamParamsItem> list = new List<SkirmishTeamParamsItem>();
			foreach (UnitClass availableVanillaShip in GetAvailableVanillaShips())
			{
				list.Add(new SkirmishTeamParamsItem
				{
					UnitClass = availableVanillaShip
				});
			}
			if (includeCustomVariants && loadedCustomUnitVariants != null)
			{
				foreach (CustomUnitVariant loadedCustomUnitVariant in loadedCustomUnitVariants)
				{
					if (CombatRatingHelper.CalculateCustomVariantCombatRating(loadedCustomUnitVariant) > 0f)
					{
						list.Add(new SkirmishTeamParamsItem
						{
							UnitClass = loadedCustomUnitVariant.UnitClass,
							CustomUnitVariant = loadedCustomUnitVariant
						});
					}
				}
			}
			return list;
		}

		public static List<SkirmishTeamParamsItem> GetAvailableShipsIncludingCustomVariants(IEnumerable<CustomUnitVariant> loadedCustomUnitVariants)
		{
			List<SkirmishTeamParamsItem> list = new List<SkirmishTeamParamsItem>();
			foreach (UnitClass availableVanillaShip in GetAvailableVanillaShips())
			{
				list.Add(new SkirmishTeamParamsItem
				{
					UnitClass = availableVanillaShip
				});
			}
			foreach (CustomUnitVariant loadedCustomUnitVariant in loadedCustomUnitVariants)
			{
				list.Add(new SkirmishTeamParamsItem
				{
					UnitClass = loadedCustomUnitVariant.UnitClass,
					CustomUnitVariant = loadedCustomUnitVariant
				});
			}
			return list;
		}
	}
}
