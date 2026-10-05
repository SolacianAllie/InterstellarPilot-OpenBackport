using System.Collections.Generic;
using Pixelfactor.IP.Common.FleetOrders;
using Pixelfactor.IP.Engine.AI.ActiveOrders;

namespace Pixelfactor.IP.Engine.Fleets.FleetOrders
{
	public class AutonomousTradeOrder : TradeOrder
	{
		public bool TradeOnlySpecificCargoTypes;

		public List<CargoClass> TradeSpecificCargoTypes = new List<CargoClass>();

		public override FleetOrderType OrderType => FleetOrderType.AutonomousTrade;

		public override bool CanSpendCredits => true;

		public override float AIDefaultMaxDuration => 0f;

		public override string GetDescription()
		{
			return "Trade";
		}

		protected override ActiveFleetOrder createActiveFleetOrder()
		{
			ActiveAutonomousTradeOrder activeAutonomousTradeOrder = gameObject.AddComponent<ActiveAutonomousTradeOrder>();
			activeAutonomousTradeOrder.TradeObjective = this;
			activeAutonomousTradeOrder.AutonomousTradeObjective = this;
			return activeAutonomousTradeOrder;
		}

		public override bool IsRepeatable()
		{
			return true;
		}
	}
}
