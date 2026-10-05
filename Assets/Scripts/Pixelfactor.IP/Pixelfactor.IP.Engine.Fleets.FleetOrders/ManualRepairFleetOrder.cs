using Pixelfactor.IP.Common.FleetOrders;
using Pixelfactor.IP.Engine.AI.ActiveOrders;

namespace Pixelfactor.IP.Engine.Fleets.FleetOrders
{
	public class ManualRepairFleetOrder : RepairFleetOrder
	{
		public Unit SpecificRepairLocation;

		public override FleetOrderType OrderType => FleetOrderType.ManualRepair;

		protected override ActiveFleetOrder createActiveFleetOrder()
		{
			ActiveManualRepairFleetOrder activeManualRepairFleetOrder = gameObject.AddComponent<ActiveManualRepairFleetOrder>();
			activeManualRepairFleetOrder.RepairGroupObjective = (activeManualRepairFleetOrder.ManualRepairGroupObjective = this);
			return activeManualRepairFleetOrder;
		}

		public override string GetDescription()
		{
			return "Repair";
		}

		public override bool IsRepeatable()
		{
			return true;
		}
	}
}
