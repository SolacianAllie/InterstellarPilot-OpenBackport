using Pixelfactor.IP.Common.Factions;
using Pixelfactor.Unity.Utils;

namespace Pixelfactor.IP.Engine.Factions
{
	public struct FactionAIStrategyItem : IWeighted
	{
		public FactionStrategy Strategy { get; set; }

		public float Weight { get; set; }

		public bool CanAssignToFleet { get; set; }

		public FactionAIStrategyItem(FactionStrategy strategy, float weight, bool canAssignToFleet)
		{
			this = default;
			Strategy = strategy;
			Weight = weight;
			CanAssignToFleet = canAssignToFleet;
		}
	}
}
