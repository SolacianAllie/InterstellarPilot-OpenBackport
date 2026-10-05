using Pixelfactor.IP.Common.FleetOrders;
using Pixelfactor.IP.Engine.AI.ActiveOrders;
using Pixelfactor.IP.Engine.Fleets.ActiveObjectives;

namespace Pixelfactor.IP.Engine.Fleets.FleetOrders
{
	public class UndockOrder : FleetOrder
	{
		public override FleetOrderType OrderType => FleetOrderType.Undock;

		public override string GetDescription()
		{
			return "Undock";
		}

		protected override ActiveFleetOrder createActiveFleetOrder()
		{
			return gameObject.AddComponent<ActiveUndockOrder>();
		}
	}
}
