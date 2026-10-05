using System.Collections.Generic;
using Pixelfactor.IP.Common.Factions;

namespace Pixelfactor.IP.Engine.Factions
{
	public class FactionAITrader : FactionAIBase
	{
		public bool TradeOnlySpecificCargoTypes;

		public List<CargoClass> TradeSpecificCargoTypes = new List<CargoClass>();

		public override FactionAIType AIType => FactionAIType.Trader;

		public override int GetPreferredCountOfUnitClass(UnitClass unitClass)
		{
			if (unitClass.StationPurpose == StationPurpose.Factory)
			{
				if (faction.IsFreelancer)
				{
					return 1;
				}
				return 8;
			}
			return base.GetPreferredCountOfUnitClass(unitClass);
		}
	}
}
