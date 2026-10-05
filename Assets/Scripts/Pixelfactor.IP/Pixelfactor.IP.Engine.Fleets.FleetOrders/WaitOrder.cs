using Pixelfactor.IP.Common.FleetOrders;
using Pixelfactor.IP.Engine.AI.ActiveOrders;

namespace Pixelfactor.IP.Engine.Fleets.FleetOrders
{
	public class WaitOrder : FleetOrder
	{
		public float WaitTime;

		public override FleetOrderType OrderType => FleetOrderType.Wait;

		protected override ActiveFleetOrder createActiveFleetOrder()
		{
			ActiveWaitOrder activeWaitOrder = gameObject.AddComponent<ActiveWaitOrder>();
			activeWaitOrder.WaitObjective = this;
			return activeWaitOrder;
		}

		public override string GetDescription()
		{
			return "Wait";
		}

		public override bool IsRepeatable()
		{
			return true;
		}
	}
}
