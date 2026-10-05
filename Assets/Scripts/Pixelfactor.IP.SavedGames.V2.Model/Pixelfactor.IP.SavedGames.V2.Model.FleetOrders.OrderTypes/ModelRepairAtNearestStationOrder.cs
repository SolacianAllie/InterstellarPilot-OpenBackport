using Pixelfactor.IP.Common.FleetOrders;

namespace Pixelfactor.IP.SavedGames.V2.Model.FleetOrders.OrderTypes
{
	public class ModelRepairAtNearestStationOrder : ModelFleetOrder
	{
		public InsufficientCreditsMode InsufficientCreditsMode { get; set; }

		public override FleetOrderType OrderType => FleetOrderType.RepairAtNearest;
	}
}
