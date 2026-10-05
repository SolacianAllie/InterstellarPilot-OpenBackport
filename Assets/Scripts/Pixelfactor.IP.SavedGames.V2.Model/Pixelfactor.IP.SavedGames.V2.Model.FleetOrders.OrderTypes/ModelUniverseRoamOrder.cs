using Pixelfactor.IP.Common.FleetOrders;

namespace Pixelfactor.IP.SavedGames.V2.Model.FleetOrders.OrderTypes
{
	public class ModelUniverseRoamOrder : ModelFleetOrder
	{
		public override FleetOrderType OrderType => FleetOrderType.AutonomousRoamLocationsObjective;
	}
}
