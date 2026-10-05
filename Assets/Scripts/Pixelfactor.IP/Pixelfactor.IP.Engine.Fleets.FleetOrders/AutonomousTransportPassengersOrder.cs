using Pixelfactor.IP.Common.FleetOrders;
using Pixelfactor.IP.Engine.AI.ActiveOrders;

namespace Pixelfactor.IP.Engine.Fleets.FleetOrders
{
	public class AutonomousTransportPassengersOrder : FleetOrder
	{
		public override FleetOrderType OrderType => FleetOrderType.AutonomousTransportPassengers;

		public override float AIDefaultMaxDuration => 0f;

		public override string GetDescription()
		{
			return "Transport Passengers";
		}

		protected override ActiveFleetOrder createActiveFleetOrder()
		{
			ActiveTransportPassengersOrder activeTransportPassengersOrder = gameObject.AddComponent<ActiveTransportPassengersOrder>();
			activeTransportPassengersOrder.AutonomousTransportPassengersObjective = this;
			return activeTransportPassengersOrder;
		}

		public override bool IsRepeatable()
		{
			return true;
		}
	}
}
