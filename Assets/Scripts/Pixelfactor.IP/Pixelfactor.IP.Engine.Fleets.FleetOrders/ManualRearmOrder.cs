using Pixelfactor.IP.Common.FleetOrders;
using Pixelfactor.IP.Engine.AI.ActiveOrders;
using Pixelfactor.IP.Engine.Factions;
using Pixelfactor.IP.Engine.Fleets.ActiveOrders;

namespace Pixelfactor.IP.Engine.Fleets.FleetOrders
{
	public class ManualRearmOrder : RearmOrder
	{
		public Unit SpecificRearmLocation;

		public override FleetOrderType OrderType => FleetOrderType.ManualRearm;

		protected override ActiveFleetOrder createActiveFleetOrder()
		{
			ActiveManualRearmOrder activeManualRearmOrder = gameObject.AddComponent<ActiveManualRearmOrder>();
			activeManualRearmOrder.RearmOrder = (activeManualRearmOrder.ManualRearmOrder = this);
			return activeManualRearmOrder;
		}

		public override string GetDescriptionForFaction(Faction faction, Sector currentSector)
		{
			if (faction != null && SpecificRearmLocation != null && SpecificRearmLocation.Sector != null)
			{
				SpecificRearmLocation.GetFriendlyName();
				return "Rearm at " + UnitNamer.GetFriendlyNameInBracketsAndFactionShortNameAndLastKnownSector(faction, currentSector, SpecificRearmLocation);
			}
			return "Rearm";
		}
	}
}
