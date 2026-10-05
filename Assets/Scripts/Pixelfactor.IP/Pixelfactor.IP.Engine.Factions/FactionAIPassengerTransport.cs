using Pixelfactor.IP.Common.Factions;

namespace Pixelfactor.IP.Engine.Factions
{
	public class FactionAIPassengerTransport : FactionAIBase
	{
		public override FactionAIType AIType => FactionAIType.PassengerTransport;

		protected override void AssignOrdersToTimedOutFleet(Fleet fleet, Unit groupLeaderUnit)
		{
			if (fleet.FleetStrategy == FactionStrategy.PassengerTransport)
			{
				OrderExplore(fleet);
			}
			else
			{
				base.AssignOrdersToTimedOutFleet(fleet, groupLeaderUnit);
			}
		}
	}
}
