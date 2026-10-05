using Pixelfactor.IP.Common.FleetOrders;
using Pixelfactor.IP.Engine.AI.ActiveOrders;
using Pixelfactor.IP.Engine.Factions;
using Pixelfactor.IP.Engine.Fleets.ActiveOrders;

namespace Pixelfactor.IP.Engine.Fleets.FleetOrders
{
	public class RearmAtNearestOrder : RearmOrder
	{
		public override FleetOrderType OrderType => FleetOrderType.RearmAtNearest;

		protected override ActiveFleetOrder createActiveFleetOrder()
		{
			ActiveRearmAtNearestOrder activeRearmAtNearestOrder = gameObject.AddComponent<ActiveRearmAtNearestOrder>();
			activeRearmAtNearestOrder.RearmOrder = this;
			return activeRearmAtNearestOrder;
		}

		public override string GetDescriptionForFaction(Faction faction, Sector currentSector)
		{
			return "Rearm at nearest";
		}
	}
}
