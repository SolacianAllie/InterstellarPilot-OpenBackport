using Pixelfactor.IP.Common.Factions;

namespace Pixelfactor.IP.Common
{
	public static class FactionStrategyExtensions
	{
		public static bool IsPeaceful(this FactionStrategy factionStrategy)
		{
			if ((uint)(factionStrategy - 1) <= 1u || factionStrategy == FactionStrategy.BountyHunt || factionStrategy == FactionStrategy.Escort)
			{
				return false;
			}
			return true;
		}
	}
}
