using System;
using Pixelfactor.IP.Engine.CombatRatings;
using Pixelfactor.IP.Engine.CustomUnitVariants;

namespace Pixelfactor.IP.Engine
{
	[Serializable]
	public class SkirmishTeamParamsItem
	{
		public UnitClass UnitClass;

		public CustomUnitVariant CustomUnitVariant;

		public int SaleCost
		{
			get
			{
				if (CustomUnitVariant != null)
				{
					return CustomUnitVariantHelper.CalculateMoneyValue(CustomUnitVariant);
				}
				return UnitClass.SaleCost;
			}
		}

		public float GetCombatRating()
		{
			if (CustomUnitVariant == null)
			{
				return UnitClass.CombatRating;
			}
			return CombatRatingHelper.CalculateCustomVariantCombatRating(CustomUnitVariant);
		}
	}
}
