using Pixelfactor.IP.Common.FleetOrders;
using Pixelfactor.IP.Engine.AI.ActiveOrders;

namespace Pixelfactor.IP.Engine.Fleets.FleetOrders
{
	public class RepairAtNearestStationOrder : RepairFleetOrder
	{
		public override FleetOrderType OrderType => FleetOrderType.RepairAtNearest;

		protected override ActiveFleetOrder createActiveFleetOrder()
		{
			ActiveRepairAtNearestStationOrder activeRepairAtNearestStationOrder = gameObject.AddComponent<ActiveRepairAtNearestStationOrder>();
			activeRepairAtNearestStationOrder.RepairGroupObjective = this;
			return activeRepairAtNearestStationOrder;
		}
	}
}
