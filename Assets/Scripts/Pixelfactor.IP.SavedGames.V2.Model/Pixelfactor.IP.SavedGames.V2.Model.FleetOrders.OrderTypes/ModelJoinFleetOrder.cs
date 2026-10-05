using Pixelfactor.IP.Common.FleetOrders;

namespace Pixelfactor.IP.SavedGames.V2.Model.FleetOrders.OrderTypes
{
	public class ModelJoinFleetOrder : ModelFleetOrder
	{
		public ModelFleet TargetFleet { get; set; }

		public override FleetOrderType OrderType => FleetOrderType.JoinFleet;
	}
}
