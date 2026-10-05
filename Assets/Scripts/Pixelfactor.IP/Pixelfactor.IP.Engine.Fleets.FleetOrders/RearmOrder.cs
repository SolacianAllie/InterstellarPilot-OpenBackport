using Pixelfactor.IP.Common.FleetOrders;
using Pixelfactor.IP.Engine.AI.ActiveOrders;
using Pixelfactor.IP.Engine.Fleets.ActiveOrders;

namespace Pixelfactor.IP.Engine.Fleets.FleetOrders
{
	public class RearmOrder : FleetOrder
	{
		public InsufficientCreditsMode InsufficientCreditsMode;

		public float EquipmentUsage = 0.5f;

		public override FleetOrderType OrderType => FleetOrderType.Rearm;

		public override bool CanSpendCredits => true;

		public override string GetDescription()
		{
			return "Rearm";
		}

		protected override ActiveFleetOrder createActiveFleetOrder()
		{
			ActiveRearmOrder activeRearmOrder = gameObject.AddComponent<ActiveRearmOrder>();
			activeRearmOrder.RearmOrder = this;
			return activeRearmOrder;
		}

		public override bool IsRepeatable()
		{
			return true;
		}
	}
}
