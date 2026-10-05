using Pixelfactor.IP.Common.FleetOrders;
using Pixelfactor.IP.Engine.AI.ActiveOrders;

namespace Pixelfactor.IP.Engine.Fleets.FleetOrders
{
	public class AutonomousRoamLocationsOrder : FleetOrder
	{
		public override FleetOrderType OrderType => FleetOrderType.AutonomousRoamLocationsObjective;

		protected override ActiveFleetOrder createActiveFleetOrder()
		{
			ActiveRoamLocationsOrder activeRoamLocationsOrder = gameObject.AddComponent<ActiveRoamLocationsOrder>();
			activeRoamLocationsOrder.AutonomousRoamLocationsObjective = this;
			return activeRoamLocationsOrder;
		}

		public override bool IsRepeatable()
		{
			return true;
		}
	}
}
