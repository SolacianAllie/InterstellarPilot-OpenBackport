using System.Collections.Generic;
using Pixelfactor.IP.Common.Factions;

namespace Pixelfactor.IP.SavedGames.V2.Model.Factions.FactionAITypes
{
	public class ModelFactionAITrader : ModelFactionAI
	{
		public bool TradeOnlySpecificCargoTypes { get; set; }

		public List<ModelCargoClass> TradeSpecificCargoTypes { get; set; } = new List<ModelCargoClass>();

		public override FactionAIType AIType => FactionAIType.Trader;
	}
}
